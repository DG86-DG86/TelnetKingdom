using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelnetKingdoms.InputInterface
{
    public class UserCommand
    {
        public string Command { get; init; }
        public List<string> Arguments { get; init; }

        public UserCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                Command = string.Empty;
                Arguments = new List<string>(0);
                return;
            }

            var split = SplitText(command);
            Command = split[0];
            Arguments = split.Skip(1).ToList();
        }

        private List<string> SplitText(string text)
        {
            List<string> result = new List<string>();

            StringBuilder accumulation = new StringBuilder();
            bool quoted = false;

            void FinishAccumulation()
            {
                result.Add(accumulation.ToString());
                accumulation.Clear();
            }

            foreach (char c in text)
            {
                if (c == ' ' && !quoted)
                    FinishAccumulation();
                else if (c == '"')
                {
                    if (quoted)
                        FinishAccumulation();

                    quoted = !quoted;
                }
                else
                    accumulation.Append(c);
            }

            // final accumulation in buffer
            if (accumulation.Length > 0)
                FinishAccumulation();

            return result;
        }
    }
}
