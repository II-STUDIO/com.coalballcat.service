using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Like PlayerPrefs of UnityEngine but marge for eaizy to use.
    /// </summary>
    public static class Pre
    {
        public static string FindStringData(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return "";
            return PlayerPrefs.GetString(key);
        }

        public static void SaveStringData(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static bool FindBoolData(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return false;

            if (PlayerPrefs.GetString(key) == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SaveBoolData(string key, bool value)
        {
            if (value)
            {
                PlayerPrefs.SetString(key, "true");
            }
            else
            {
                PlayerPrefs.SetString(key, "false");
            }
        }

        public static int FindIntData(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return 0;

            return PlayerPrefs.GetInt(key, 0);
        }

        public static void SaveIntData(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static float FindFloatData(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return 0;

            return PlayerPrefs.GetFloat(key, 0);
        }

        public static void SaveFloatData(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static bool Has(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteData(string key)
        {
            if (Has(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
    }
}
