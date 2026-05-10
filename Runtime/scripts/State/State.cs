using Cysharp.Threading.Tasks;

namespace Coalballcat.Services
{
    /// <summary>
    /// Base class for synchronous states.
    /// Override Tick() if this state needs per-frame logic.
    ///
    /// Example:
    /// <code>
    /// public class IdleState : State, IStateTick
    /// {
    ///     public override void Enter()  => animator.Play("Idle");
    ///     public override void Exit()   { }
    ///     public void Tick()            => CheckForInput();
    /// }
    /// </code>
    /// </summary>
    public abstract class State : IStateSync
    {
        public abstract void Enter();
        public abstract void Exit();
    }

    /// <summary>
    /// Base class for asynchronous states (loading, cutscenes, fade transitions).
    /// Can still implement IStateTick if the state runs logic while it's active.
    ///
    /// Example:
    /// <code>
    /// public class LoadingState : StateAsync, IStateTick
    /// {
    ///     public override async UniTask EnterAsync() => await LoadScene();
    ///     public override async UniTask ExitAsync()  => await FadeOut();
    ///     public void Tick()                         => UpdateProgressBar();
    /// }
    /// </code>
    /// </summary>
    public abstract class StateAsync : IStateAsync
    {
        public abstract UniTask EnterAsync();
        public abstract UniTask ExitAsync();
    }
}
