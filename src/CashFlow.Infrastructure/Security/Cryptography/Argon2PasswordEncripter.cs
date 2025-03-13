using CashFlow.Domain.Security.Cryptography;
using Konscious.Security.Cryptography;
using System;
using System.Text;

namespace CashFlow.Infrastructure.Security.Cryptography
{
    internal class Argon2PasswordEncripter : IPasswordEncripter
    {
        public string Encrypt(string password)
        {
            // Use Argon2 to create hash
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            using var argon2 = new Argon2id(passwordBytes);

            // Config argon parameters
            argon2.DegreeOfParallelism = 8;  
            argon2.MemorySize = 65536;       
            argon2.Iterations = 4;           

            var hash = argon2.GetBytes(32); // Size hash in bytes
            return Convert.ToBase64String(hash); 
        }

        public bool Verify(string password, string passwordHash)
        {
            var storedHashBytes = Convert.FromBase64String(passwordHash);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using var argon2 = new Argon2id(passwordBytes);
            argon2.DegreeOfParallelism = 8;
            argon2.MemorySize = 65536;
            argon2.Iterations = 4;

            // Generates hash to received password and compares to stored hash
            var hashedPassword = argon2.GetBytes(32);
            return CompareByteArrays(hashedPassword, storedHashBytes);
        }

        private bool CompareByteArrays(byte[] first, byte[] second)
        {
            if (first.Length != second.Length) return false;
            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i]) return false;
            }
            return true;
        }
    }
}
