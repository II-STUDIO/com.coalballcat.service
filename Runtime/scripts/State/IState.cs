using Cysharp.Threading.Tasks;

namespace Coalballcat.Services
{
    // ── Base marker ───────────────────────────────────────────────────────────

    /// <summary>
    /// Marker interface. All states implement this.
    /// StateMachine stores them as IState in its dictionary.
    /// </summary>
    public interface IState { }

    // ── Enter / Exit variants ─────────────────────────────────────────────────

    /// <summary>Synchronous state — Enter and Exit run in the same frame.</summary>
    public interface IStateSync : IState
    {
        void Enter();
        void Exit();
    }

    /// <summary>Asynchronous state — transitions can span multiple frames (loading, cutscenes, etc.).</summary>
    public interface IStateAsync : IState
    {
        UniTask EnterAsync();
        UniTask ExitAsync();
    }

    // ── Tick variants — opt-in, mix with either Enter/Exit style ──────────────

    /// <summary>
    /// Add to any state that needs a per-frame update.
    /// The owning MonoBehaviour calls stateMachine.Tick() from Update().
    /// Compatible with both IStateSync and IStateAsync.
    /// </summary>
    public interface IStateTick
    {
        void Tick();
    }

    /// <summary>Physics update. Call stateMachine.FixedTick() from FixedUpdate().</summary>
    public interface IStateFixedTick
    {
        void FixedTick();
    }

    /// <summary>Late update. Call stateMachine.LateTick() from LateUpdate().</summary>
    public interface IStateLateTick
    {
        void LateTick();
    }
}
