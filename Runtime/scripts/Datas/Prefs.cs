using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Thin convenience wrapper over <see cref="PlayerPrefs"/> with consistent
    /// has-key handling, default values, and a bool helper.
    /// </summary>
    public static class Prefs
    {
        public static string FindStringData(string key, string defaultValue = "")
            => PlayerPrefs.GetString(key, defaultValue);

        public static void SaveStringData(string key, string value)
            => PlayerPrefs.SetString(key, value);

        public static bool FindBoolData(string key, bool defaultValue = false)
            => PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) != 0 : defaultValue;

        public static void SaveBoolData(string key, bool value)
            => PlayerPrefs.SetInt(key, value ? 1 : 0);

        public static int FindIntData(string key, int defaultValue = 0)
            => PlayerPrefs.GetInt(key, defaultValue);

        public static void SaveIntData(string key, int value)
            => PlayerPrefs.SetInt(key, value);

        public static float FindFloatData(string key, float defaultValue = 0f)
            => PlayerPrefs.GetFloat(key, defaultValue);

        public static void SaveFloatData(string key, float value)
            => PlayerPrefs.SetFloat(key, value);

        public static bool Has(string key)
            => PlayerPrefs.HasKey(key);

        public static void DeleteData(string key)
        {
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }

        /// <summary>Removes ALL stored PlayerPrefs keys. Use with care.</summary>
        public static void DeleteAll()
            => PlayerPrefs.DeleteAll();

        /// <summary>Flushes pending changes to disk. Call after a batch of writes.</summary>
        public static void Save()
            => PlayerPrefs.Save();
    }
}
