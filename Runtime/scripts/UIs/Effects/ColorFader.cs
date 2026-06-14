using System;
using System.Collections;
using UnityEngine;

namespace Coalballcat.Services.UI
{
    /// <summary>
    /// Coroutine helpers for fading a <see cref="CanvasGroup"/>'s alpha.
    /// Start them with <c>StartCoroutine(...)</c> from any MonoBehaviour.
    /// </summary>
    public static class ColorFader
    {
        /// <summary>Fade <paramref name="group"/> from its current alpha to <paramref name="targetAlpha"/>.</summary>
        /// <param name="useUnscaledTime">Use unscaled time so the fade runs while the game is paused.</param>
        /// <param name="onComplete">Optional callback invoked when the fade finishes.</param>
        public static IEnumerator Fade(CanvasGroup group, float targetAlpha, float duration,
            bool useUnscaledTime = false, Action onComplete = null)
        {
            if (group == null)
                yield break;

            targetAlpha = Mathf.Clamp01(targetAlpha);

            if (duration <= 0f)
            {
                group.alpha = targetAlpha;
                onComplete?.Invoke();
                yield break;
            }

            float startAlpha = group.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }

            group.alpha = targetAlpha;
            onComplete?.Invoke();
        }

        /// <summary>Fade in to a target alpha (default fully opaque).</summary>
        public static IEnumerator FadeIn(CanvasGroup group, float duration, float targetAlpha = 1f,
            bool useUnscaledTime = false, Action onComplete = null)
            => Fade(group, targetAlpha, duration, useUnscaledTime, onComplete);

        /// <summary>Fade out to a target alpha (default fully transparent).</summary>
        public static IEnumerator FadeOut(CanvasGroup group, float duration, float targetAlpha = 0f,
            bool useUnscaledTime = false, Action onComplete = null)
            => Fade(group, targetAlpha, duration, useUnscaledTime, onComplete);
    }
}
