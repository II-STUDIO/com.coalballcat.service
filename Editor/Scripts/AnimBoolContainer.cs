#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

namespace Coalballcat.ServicesEditor
{
    public class AnimBoolContainer<TKey>
    {
        private Dictionary<TKey, AnimBool> dict = new();

        public void Add(TKey key, bool startValue)
        {
            if (dict.ContainsKey(key))
                return;

            dict.Add(key, new AnimBool(startValue));
        }

        public AnimBool Get(TKey key, bool startValue)
        {
            if (dict.TryGetValue(key, out var anim))
            {
                return anim;
            }

            anim = new AnimBool(startValue);

            dict.Add(key, anim);
            return anim;
        }

        public void Remove(TKey key)
        {
            if (!dict.ContainsKey(key))
                return;

            dict.Remove(key);
        }
    }
}
#endif