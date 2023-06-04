using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelnetKingdoms.InputInterface
{
    public interface InputInterface
    {
        public string GetPrompt(UserContext context);
        public void ExecuteCommand(UserContext context);
    }
}
