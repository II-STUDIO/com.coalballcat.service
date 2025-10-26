#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coalballcat.ServicesEditor
{
    public static class AssetFounder
    {
        public static void FoundExistOrCreateOneSO<T>(string directory, string assetName, out T output) where T : ScriptableObject
        {
            string typeName = typeof(T).Name;

            var guids = AssetDatabase.FindAssets($"t:{typeName}");

            output = null;

            if (guids.Length > 0)
                output = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(T)) as T;

            if (output != null)
            {
                return;
            }
            else
            {
                output = ScriptableObject.CreateInstance<T>();

                string path = directory + assetName;

                string targetDirectory = $"{Application.dataPath}/{directory}/";
                string resultPath = $"Assets/{path}";

                if (!Directory.Exists(targetDirectory))
                    Directory.CreateDirectory(targetDirectory);

                AssetDatabase.Refresh();

                AssetDatabase.CreateAsset(output, resultPath);
                AssetDatabase.SaveAssets();
            }
        }

        public static void FoundExistOrCreateSO<T>(string path, out T output) where T : ScriptableObject
        {
            output = ScriptableObject.CreateInstance<T>();

            AssetDatabase.Refresh();

            AssetDatabase.CreateAsset(output, path);
            AssetDatabase.SaveAssets();
        }

        public static T[] FoundAllAssetTypeAtPath<T>(string path) where T : Object
        {
            string typeName = typeof(T).Name;

            string[] guids = AssetDatabase.FindAssets($"t:{typeName}", new[] { path });
            T[] assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }

            return assets;
        }
    }
}
#endif