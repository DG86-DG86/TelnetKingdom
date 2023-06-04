using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelnetKingdoms.Data;
using TelnetKingdoms.InputInterface;

namespace TelnetKingdoms.UserAuthentication
{
    internal class CreateUserInputInterface : InputInterface.InputInterface
    {
        const string NEW_NAME_KEY = nameof(CreateUserInputInterface) + ".new_name";
        const string CONFIRMED_NAME_KEY = nameof(CreateUserInputInterface) + ".confirmed_name";
        const string GET_PASSWORD_KEY = nameof(CreateUserInputInterface) + ".get_password";
        const string NEW_PASSWORD_KEY = nameof(CreateUserInputInterface) + ".new_password";

        public void ExecuteCommand(UserContext context)
        {
            // handle confirming new name
            if (context.Data.ContainsKey(NEW_NAME_KEY))
                ConfirmName(context);
            else if (context.Data.ContainsKey(GET_PASSWORD_KEY))
                GetPassword(context);
            else if (context.Data.ContainsKey(NEW_PASSWORD_KEY))
                ConfirmPassword(context);
            else
                EnterNewName(context);
        }

        public string GetPrompt(UserContext context)
        {
            if (context.Data.ContainsKey(NEW_NAME_KEY))
                return "\n\rRe-enter your name to confirm.\n\r>";
            else if (context.Data.ContainsKey(GET_PASSWORD_KEY))
                return "\n\rEnter a password (with no spaces or quotes.)\n\r>";
            else if (context.Data.ContainsKey(NEW_PASSWORD_KEY))
                return "\n\rRe-enter your password to confirm.\n\r>";
            else
                return "\n\rEnter your user name (with no spaces or quotes.)\n\r>";
        }

        private void EnterNewName(UserContext context)
        {
            // command is user name
            context.Data[NEW_NAME_KEY] = context.Command.Command;
        }

        private void ConfirmName(UserContext context)
        {
            // make sure it matches, or go back a step
            if (context.Command.Command != (string)context.Data[NEW_NAME_KEY])
            {
                context.Data.Remove(NEW_NAME_KEY);
                Program.s.sendMessageToClient(context.Client, "\n\rNames do not match.\n\r");

                return;
            }

            // remove new name and set data
            context.Data[CONFIRMED_NAME_KEY] = (string)context.Data[NEW_NAME_KEY];
            context.Data.Remove(NEW_NAME_KEY);
            context.Data.Add(GET_PASSWORD_KEY, true);
        }

        private void GetPassword(UserContext context)
        {
            context.Data[NEW_PASSWORD_KEY] = context.Command.Command;
            context.Data.Remove(GET_PASSWORD_KEY);
        }

        private void ConfirmPassword(UserContext context)
        {
            // make sure passwords match or go back a step
            if (context.Command.Command != (string)context.Data[NEW_PASSWORD_KEY])
            {
                context.Data.Remove (NEW_PASSWORD_KEY);
                Program.s.sendMessageToClient(context.Client, "\n\rPasswords do not match.\n\r");

                return;
            }

            // create a new user
            context.User = DataCore.CreateNewInstance<User>();
            context.User.Name = (string)context.Data[CONFIRMED_NAME_KEY];
            context.User.EncryptToPassword((string)context.Data[NEW_PASSWORD_KEY]);

            // remove context data for this interface
            context.Data.Remove(NEW_PASSWORD_KEY);
            context.Data.Remove(CONFIRMED_NAME_KEY);

            // pop back to login interface
            context.InputInterfaceStack.Pop();
        }
    }
}
