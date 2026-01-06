using System;

[Serializable]
public struct Timer
{
    public float Duration;
    public float Remaining;
    public bool IsRunning;
    public bool IsPaused;
    public bool isRepeadted;

    private Action _onComplete;
    private Action<float> _onUpdate;

    // Start a timer with callbacks
    public void Start(float duration, Action onComplete = null, Action<float> onUpdate = null)
    {
        Duration = duration;
        Remaining = duration;
        IsRunning = true;
        IsPaused = false;
        _onComplete = onComplete;
        _onUpdate = onUpdate;
    }

    // Must be called from your Update loop
    public void Tick(float deltaTime)
    {
        if (!IsRunning || IsPaused)
            return;

        Remaining -= deltaTime;
        if (_onUpdate != null)
            _onUpdate(Remaining);

        if (Remaining <= 0f)
        {
            IsRunning = false;
            float oldRemaining = Remaining;
            Remaining = 0f;
            _onUpdate?.Invoke(0f);
            _onComplete?.Invoke();
        }
    }

    public void Stop()
    {
        IsRunning = false;
        IsPaused = false;
        Remaining = 0;
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;

    public void Restart()
    {
        Remaining = Duration;
        IsRunning = true;
        IsPaused = false;
    }

    public bool IsFinished => IsRunning == false && Remaining <= 0f;
}
