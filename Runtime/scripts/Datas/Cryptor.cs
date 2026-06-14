using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Lightweight AES-based string encryption helper for local save data.
    /// Layout of the Base64 payload: [16-byte salt][16-byte IV][ciphertext].
    ///
    /// This is convenience obfuscation for on-device data, NOT a substitute for
    /// server-side validation of anything security-critical.
    /// </summary>
    public static class Cryptor
    {
        private const int KeySizeBits = 256;
        private const int SaltSizeBytes = 16;
        private const int IvSizeBytes = 16; // AES block size is always 128 bits.
        private const int DefaultIterations = 10_000;

        public static string Encrypt(string plainText, string passPhrase, int derivationIterations = DefaultIterations)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                Debug.LogError("[Cryptor] Encrypt: 'plainText' cannot be null or empty.");
                return string.Empty;
            }

            if (string.IsNullOrEmpty(passPhrase))
            {
                Debug.LogError("[Cryptor] Encrypt: 'passPhrase' cannot be null or empty.");
                return string.Empty;
            }

            try
            {
                byte[] salt = GenerateRandomBytes(SaltSizeBytes);
                byte[] iv = GenerateRandomBytes(IvSizeBytes);
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                using var keyDerivation = new Rfc2898DeriveBytes(
                    passPhrase, salt, derivationIterations, HashAlgorithmName.SHA256);
                byte[] key = keyDerivation.GetBytes(KeySizeBits / 8);

                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor(key, iv);
                using var memoryStream = new MemoryStream();

                // Prepend salt + IV so they're available for decryption.
                memoryStream.Write(salt, 0, salt.Length);
                memoryStream.Write(iv, 0, iv.Length);

                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch (Exception e)
            {
                Debug.LogError($"[Cryptor] Encrypt failed: {e.Message}");
                return string.Empty;
            }
        }

        public static string Decrypt(string cipherText, string passPhrase, int derivationIterations = DefaultIterations)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                Debug.LogError("[Cryptor] Decrypt: 'cipherText' cannot be null or empty.");
                return string.Empty;
            }

            if (string.IsNullOrEmpty(passPhrase))
            {
                Debug.LogError("[Cryptor] Decrypt: 'passPhrase' cannot be null or empty.");
                return string.Empty;
            }

            try
            {
                byte[] payload = Convert.FromBase64String(cipherText);
                if (payload.Length <= SaltSizeBytes + IvSizeBytes)
                {
                    Debug.LogError("[Cryptor] Decrypt: payload is too short or corrupted.");
                    return string.Empty;
                }

                byte[] salt = new byte[SaltSizeBytes];
                byte[] iv = new byte[IvSizeBytes];
                Buffer.BlockCopy(payload, 0, salt, 0, SaltSizeBytes);
                Buffer.BlockCopy(payload, SaltSizeBytes, iv, 0, IvSizeBytes);

                int cipherOffset = SaltSizeBytes + IvSizeBytes;
                int cipherLength = payload.Length - cipherOffset;

                using var keyDerivation = new Rfc2898DeriveBytes(
                    passPhrase, salt, derivationIterations, HashAlgorithmName.SHA256);
                byte[] key = keyDerivation.GetBytes(KeySizeBits / 8);

                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor(key, iv);
                using var memoryStream = new MemoryStream(payload, cipherOffset, cipherLength);
                using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream, Encoding.UTF8);

                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Cryptor] Decrypt failed (wrong pass phrase or corrupted data?): {e.Message}");
                return string.Empty;
            }
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return bytes;
        }
    }
}
