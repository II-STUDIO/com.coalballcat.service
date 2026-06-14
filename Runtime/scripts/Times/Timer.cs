using System;

namespace Coalballcat.Services
{
    /// <summary>
    /// Lightweight, allocation-free countdown timer. Drive it by calling
    /// <see cref="Tick"/> from your own Update loop.
    /// </summary>
    [Serializable]
    public struct Timer
    {
        public float Duration;
        public float Remaining;
        public bool IsRunning;
        public bool IsPaused;
        public bool IsRepeated;

        private Action _onComplete;
        private Action<float> _onUpdate;

        /// <summary>Start (or restart) the timer with optional callbacks.</summary>
        public void Start(float duration, Action onComplete = null, Action<float> onUpdate = null, bool repeat = false)
        {
            Duration = duration;
            Remaining = duration;
            IsRunning = true;
            IsPaused = false;
            IsRepeated = repeat;
            _onComplete = onComplete;
            _onUpdate = onUpdate;
        }

        /// <summary>Advance the timer. Call once per frame with <c>Time.deltaTime</c>.</summary>
        public void Tick(float deltaTime)
        {
            if (!IsRunning || IsPaused)
                return;

            Remaining -= deltaTime;

            if (Remaining > 0f)
            {
                _onUpdate?.Invoke(Remaining);
                return;
            }

            _onUpdate?.Invoke(0f);
            _onComplete?.Invoke();

            if (IsRepeated && Duration > 0f)
            {
                // Carry the overshoot so repeating timers don't drift.
                Remaining += Duration;
            }
            else
            {
                Remaining = 0f;
                IsRunning = false;
            }
        }

        public void Stop()
        {
            IsRunning = false;
            IsPaused = false;
            Remaining = 0f;
        }

        public void Pause() => IsPaused = true;
        public void Resume() => IsPaused = false;

        public void Restart()
        {
            Remaining = Duration;
            IsRunning = true;
            IsPaused = false;
        }

        public readonly bool IsFinished => !IsRunning && Remaining <= 0f;

        /// <summary>Progress from 0 (just started) to 1 (complete).</summary>
        public readonly float Progress => Duration > 0f ? 1f - (Remaining / Duration) : 1f;
    }
}
