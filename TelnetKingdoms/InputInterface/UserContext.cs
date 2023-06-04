using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelnetKingdoms.UserAuthentication;
using TelnetServer;

namespace TelnetKingdoms.InputInterface
{
    public class UserContext
    {
        public User User { get; set; } = null;
        public Client Client { get; set; } = null;
        public Dictionary<string, object> Data { get; set;} = new Dictionary<string, object>();
        public UserCommand Command { get; set; } = null;
        public Stack<InputInterface> InputInterfaceStack { get; set; } = new Stack<InputInterface>();
    }
}
