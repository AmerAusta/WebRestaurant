using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Helper
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }

        public static bool IsSha256Base64Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length != 44)
                return false;

            try
            {
                Convert.FromBase64String(password);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
