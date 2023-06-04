using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelnetKingdoms.Data;
using TelnetKingdoms.InputInterface;
using TelnetServer;

namespace TelnetKingdoms.UserAuthentication
{
    internal class UnauthInputInterface : TelnetKingdoms.InputInterface.InputInterface
    {
        public void ExecuteCommand(UserContext context)
        {
            // handle new user request
            if (context.Command.Command == "new")
            {
                context.InputInterfaceStack.Push(new CreateUserInputInterface());
                return;
            }

            // handle incorrect number of arguments
            if (context.Command.Arguments.Count != 1)
            {
                Program.s.sendMessageToClient(context.Client, "\n\rIncorrect entry: please enter your name, a space, and then your password.\n\r");
                return;
            }

            // command is user name, first argument is password
            User u = DataCore.Query<User>(x => x.Name == context.Command.Command).FirstOrDefault();

            // handle "user not found"
            if (u == null)
            {
                Program.s.sendMessageToClient(context.Client, "\n\rNo user found with that name.\n\r");
                return;
            }

            // handle "incorrect password"
            if (!u.CompareToPassword(context.Command.Arguments[0]))
            {
                Program.s.sendMessageToClient(context.Client, "\n\rPasswords do not match.\n\r>");
                return;
            }

            // TODO: successful authentication
            Program.s.sendMessageToClient(context.Client, "\n\r\n\rTODO: Successful login.\n\r>");
        }

        public string GetPrompt(UserContext context)
        {
            return "\n\rEnter your name and password, or 'new' to register as a new user\n\r>";
        }
    }
}
