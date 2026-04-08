using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace EncryptionDiary.Shared.Helper
{
    public class CryptoServices
    {
        private const int TagSize = 16;
        private const int nonceSize = 12;
        // Generate a random key of specified size
        public static byte[] GenerateKey(int keySizeInBytes)
        {
            return RandomNumberGenerator.GetBytes(keySizeInBytes);
        }

        // Encrypt data with a key, returns nonce, ciphertext, tag
        public static (byte[] nonce, byte[] ciphertext, byte[] tag) EncryptAES_GCM(byte[] key, byte[] plaintext, byte[]? aad = null)
        {
            byte[] nonce = RandomNumberGenerator.GetBytes(nonceSize);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[TagSize];

            using var gcm = new AesGcm(key, TagSize);
            gcm.Encrypt(nonce, plaintext, ciphertext, tag, aad);

            return (nonce, ciphertext, tag);
        }

        // Decrypt data with a key, nonce, and tag
        public static byte[] DecryptAES_GCM(byte[] key, byte[] nonce, byte[] ciphertext, byte[] tag, byte[]? aad = null)
        {
            byte[] plaintext = new byte[ciphertext.Length];
            using var gcm = new AesGcm(key, TagSize);
            gcm.Decrypt(nonce, ciphertext, tag, plaintext, aad);
            return plaintext;
        }


    }
}
