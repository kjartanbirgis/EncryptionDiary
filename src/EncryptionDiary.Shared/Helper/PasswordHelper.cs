using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace EncryptionDiary.Shared.Helper
{
    public static class PasswordHelper
    {
        public static byte[] ClientHash(string password, string username)
        {
            var PasswordBytes = Encoding.UTF8.GetBytes(password);
            var usernameBytes = Encoding.UTF8.GetBytes(username);


            using var pbkdf2 = new Rfc2898DeriveBytes(PasswordBytes, usernameBytes, 100_000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32); //32 is for SHA256 (8*32 = 256)
        }

        public static byte[] GenerateSalt(int size = 32)
        {
            return RandomNumberGenerator.GetBytes(size);
        }

        public static byte[] HashPassword(byte[] clientHash, byte[] salt,int iterations,string? pepper = null)
        {
            byte[] input;
            if (pepper != null)
            {
                var pepperBytes = Encoding.UTF8.GetBytes(pepper);
                input =new byte[clientHash.Length + pepperBytes.Length];
                clientHash.CopyTo(input, 0);
                pepperBytes.CopyTo(input, clientHash.Length);
            }
            else
            {
                input = clientHash;
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(input,salt,iterations,HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32); //32 is for SHA256 (8*32 = 256)
        }
    }
}
