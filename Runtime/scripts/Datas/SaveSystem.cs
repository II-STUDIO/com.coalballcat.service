using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Cross-platform (PC & mobile) file save/load that is tamper-evident.
    ///
    /// Files are written under <see cref="Application.persistentDataPath"/> — the only
    /// writable, per-user location that behaves the same on Windows/macOS/Linux,
    /// Android, and iOS.
    ///
    /// Every file is signed with an HMAC-SHA256, so any edit a player makes is detected
    /// on load (<see cref="TryLoad{T}"/> returns false). Pass an
    /// <c>encryptionPassword</c> to additionally AES-encrypt the contents so they can't
    /// be read either.
    ///
    /// SECURITY NOTE: On-device data can never be made fully tamper-proof — a determined
    /// user with a rooted device and a decompiler can recover keys. This raises the bar
    /// against casual save editing; it is not a substitute for server-side validation of
    /// anything that matters competitively or commercially.
    /// </summary>
    public static class SaveSystem
    {
        // Used to sign files when no per-call encryption password is supplied. Changing
        // this value invalidates every previously written unencrypted save.
        private const string DefaultSignatureKey = "Coalballcat.Services.SaveSystem.v1";

        // Serialized with Newtonsoft.Json (public fields).
        private struct Envelope
        {
            public string sig;   // Base64 HMAC-SHA256 of 'data'.
            public string data;  // JSON of the user payload.
        }

        /// <summary>Absolute path a given save file resolves to.</summary>
        public static string GetFullPath(string fileName)
            => Path.Combine(Application.persistentDataPath, fileName);

        public static bool Exists(string fileName)
            => File.Exists(GetFullPath(fileName));

        /// <summary>
        /// Serializes <paramref name="data"/>, signs it, optionally encrypts it, and writes
        /// it atomically (temp file + move) so a crash mid-write can't corrupt the save.
        /// Returns true on success.
        /// </summary>
        public static bool Save<T>(string fileName, T data, string encryptionPassword = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("[SaveSystem] Save: fileName is null or empty.");
                return false;
            }

            try
            {
                string json = JsonConvert.SerializeObject(data);
                string signingKey = string.IsNullOrEmpty(encryptionPassword) ? DefaultSignatureKey : encryptionPassword;

                var envelope = new Envelope { sig = ComputeSignature(json, signingKey), data = json };
                string payload = JsonConvert.SerializeObject(envelope);

                if (!string.IsNullOrEmpty(encryptionPassword))
                {
                    payload = Cryptor.Encrypt(payload, encryptionPassword);
                    if (string.IsNullOrEmpty(payload))
                    {
                        Debug.LogError($"[SaveSystem] Save: encryption failed for '{fileName}'.");
                        return false;
                    }
                }

                WriteAtomic(GetFullPath(fileName), payload);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Save failed for '{fileName}': {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads and verifies a save. Returns false (and <paramref name="data"/> = default)
        /// if the file is missing, corrupted, tampered with, or fails to decrypt.
        /// </summary>
        public static bool TryLoad<T>(string fileName, out T data, string encryptionPassword = null)
        {
            data = default;

            string path = GetFullPath(fileName);
            if (!File.Exists(path))
                return false;

            try
            {
                string payload = File.ReadAllText(path, Encoding.UTF8);

                if (!string.IsNullOrEmpty(encryptionPassword))
                {
                    payload = Cryptor.Decrypt(payload, encryptionPassword);
                    if (string.IsNullOrEmpty(payload))
                    {
                        Debug.LogWarning($"[SaveSystem] Load: '{fileName}' could not be decrypted (wrong password or tampered).");
                        return false;
                    }
                }

                var envelope = JsonConvert.DeserializeObject<Envelope>(payload);
                if (envelope.data == null)
                {
                    Debug.LogWarning($"[SaveSystem] Load: '{fileName}' is malformed.");
                    return false;
                }

                string signingKey = string.IsNullOrEmpty(encryptionPassword) ? DefaultSignatureKey : encryptionPassword;
                string expected = ComputeSignature(envelope.data, signingKey);
                if (!FixedTimeEquals(expected, envelope.sig))
                {
                    Debug.LogWarning($"[SaveSystem] Load: '{fileName}' failed integrity check — it was modified.");
                    return false;
                }

                data = JsonConvert.DeserializeObject<T>(envelope.data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Load failed for '{fileName}': {e.Message}");
                return false;
            }
        }

        /// <summary>Loads a save, returning <paramref name="fallback"/> if it can't be read.</summary>
        public static T Load<T>(string fileName, T fallback = default, string encryptionPassword = null)
            => TryLoad(fileName, out T data, encryptionPassword) ? data : fallback;

        /// <summary>Deletes a save file. Returns true if a file was removed.</summary>
        public static bool Delete(string fileName)
        {
            try
            {
                string path = GetFullPath(fileName);
                if (!File.Exists(path))
                    return false;

                File.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Delete failed for '{fileName}': {e.Message}");
                return false;
            }
        }

        // ── Internal ──────────────────────────────────────────────────────────────

        private static void WriteAtomic(string path, string contents)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string tempPath = path + ".tmp";
            File.WriteAllText(tempPath, contents, new UTF8Encoding(false));

            // Replace the destination as atomically as the platform allows.
            if (File.Exists(path))
                File.Delete(path);
            File.Move(tempPath, path);
        }

        private static string ComputeSignature(string content, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hash);
        }

        // Constant-time-ish comparison to avoid leaking signature info via timing.
        private static bool FixedTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
