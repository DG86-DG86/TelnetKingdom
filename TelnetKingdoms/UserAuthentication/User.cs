using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TelnetKingdoms.Data;

namespace TelnetKingdoms.UserAuthentication
{
    public class User : DataTrackedObject
    {
        [DataTrackedColumn] public string Name { get => GetColumnValue<string>(nameof(Name)); set => SetColumnValue(nameof(Name), value); }
        [DataTrackedColumn] public string Password { get => GetColumnValue<string>(nameof(Password)); set => SetColumnValue(nameof(Password), value); }
        [DataTrackedColumn] public DateTime CreationDate { get => GetColumnValue<DateTime>(nameof(CreationDate)); set => SetColumnValue(nameof(CreationDate), value); }

        public User(DataRow row) : base(row) { }

        static public string HashString(string s, byte[] salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            // copy both into a single buffer
            byte[] buffer = new byte[bytes.Length + salt.Length];
            System.Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
            System.Buffer.BlockCopy(salt, 0, buffer, bytes.Length, salt.Length);

            // hash the buffer
            using (var sha = SHA1.Create())
                bytes = sha.ComputeHash(buffer);

            // write the salt to the end of the buffer
            buffer = new byte[bytes.Length + salt.Length];
            System.Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
            System.Buffer.BlockCopy(salt, 0, buffer, bytes.Length, salt.Length);

            // return as a b64 string
            return Convert.ToBase64String(buffer);
        }

        public void EncryptToPassword(string password)
        {
            byte[] salt = new byte[32];

            // generate salt
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            Password = HashString(password, salt);
        }

        public bool CompareToPassword(string comp)
        {
            // read salt from password
            byte[] bytes = Convert.FromBase64String(Password);
            byte[] salt = new byte[32];
            System.Buffer.BlockCopy(bytes, bytes.Length - salt.Length, salt, 0, salt.Length);

            return HashString(comp, salt) == Password;
        }
    }
}
