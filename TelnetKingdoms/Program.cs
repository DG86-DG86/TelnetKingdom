using System.Data;
using System.Net;
using TelnetKingdoms.Data;
using TelnetKingdoms.InputInterface;
using TelnetKingdoms.UserAuthentication;
using TelnetServer;

namespace TelnetKingdoms
{
    class Program
    {
        public static Server s;

        private static Dictionary<Client, UserContext> _user_list = new Dictionary<Client, UserContext>();

        static void Main(string[] args)
        {
            DataCore.Deserialize("data.xml");

            s = new Server(IPAddress.Any);
            s.ClientConnected += clientConnected;
            s.ClientDisconnected += clientDisconnected;
            s.ConnectionBlocked += connectionBlocked;
            s.MessageReceived += messageReceived;
            s.start();

            Console.WriteLine(DataCore.GetAll<User>().Count() + " users loaded.\n\r");

            Console.WriteLine("SERVER STARTED: " + DateTime.Now);

            char read = '\0';

            do
            {
                if (read == 'b')
                {
                    s.sendMessageToAll(Console.ReadLine());
                }
            } while ((read = Console.ReadKey(true).KeyChar) != 'q');

            s.stop();

            DataCore.Serialize("data.xml");
        }

        private static void clientConnected(Client c)
        {
            Console.WriteLine("CONNECTED: " + c);

            // drop any old connection
            if (_user_list.ContainsKey(c))
                _user_list.Remove(c);

            // create a context for this connection
            UserContext uc = new UserContext() { Client = c };
            _user_list.Add(c, uc);
            uc.InputInterfaceStack.Push(new UnauthInputInterface());

            // send prompt
            s.sendMessageToClient(c, uc.InputInterfaceStack.Peek().GetPrompt(uc));
        }

        private static void clientDisconnected(Client c)
        {
            Console.WriteLine("DISCONNECTED: " + c);

            if (_user_list.ContainsKey(c))
                _user_list.Remove(c);
        }

        private static void connectionBlocked(IPEndPoint ep)
        {
            Console.WriteLine(string.Format("BLOCKED: {0}:{1} at {2}", ep.Address, ep.Port, DateTime.Now));
        }

        private static void messageReceived(Client c, string message)
        {
            if (!_user_list.TryGetValue(c, out var context))
            {
                s.sendMessageToClient(c, "SERVER DATA ERROR.");
                s.kickClient(c);
                return;
            }

            // update context
            context.Command = new UserCommand(message);

            // execute the command
            context.InputInterfaceStack.Peek().ExecuteCommand(context);

            // send a prompt
            s.sendMessageToClient(c, context.InputInterfaceStack.Peek().GetPrompt(context));
        }
    }
}