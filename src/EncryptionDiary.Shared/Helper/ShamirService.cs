using EncryptionDiary.Shared.Models;
using SecretSharingDotNet.Cryptography;
using SecretSharingDotNet.Cryptography.ShamirsSecretSharing;
using SecretSharingDotNet.Math;
using System.Numerics;


namespace EncryptionDiary.Shared.Helper
{
    public static class ShamirService
    {
        private static List<string> GenerateShares(byte[] secret, int totalShares, int threshold)
        {
            var splitter = new SecretSplitter<BigInteger>();
            var shares = splitter.MakeShares(threshold, totalShares, secret);
            return shares.Select(s => s.ToString()).ToList();
        }

        public static byte[] ReconstructSecret(List<string> shareStrings)
        {
            var gcd = new ExtendedEuclideanAlgorithm<BigInteger>();
            var combiner = new SecretReconstructor<BigInteger>(gcd);
            
            Shares<BigInteger> list =  shareStrings.ToArray();

            return combiner.Reconstruction(list).ToByteArray();

        }
        public static List<string> GenerateShares(int totalShares, int theshold, byte[] hashedPassword, Key key)
        {
            var dycryptedKey = CryptoServices.DecryptAES_GCM(hashedPassword, key.KeyNonce, key.EncKey, key.KeyTag);
            return GenerateShares(dycryptedKey, totalShares, theshold);
        }
    }
}

