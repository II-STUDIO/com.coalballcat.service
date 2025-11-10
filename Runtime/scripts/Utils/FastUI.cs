using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Coalballcat.Services
{
    public static class FastUI
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddCleanListener(this Button button, UnityAction action)
        {
            var onClick = button.onClick;
            onClick.RemoveAllListeners();
            onClick.AddListener(action);
        }
    }
}