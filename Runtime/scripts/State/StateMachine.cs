using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Coalballcat.Services
{
    /// <summary>
    /// Generic state machine keyed by any Enum.
    /// Supports both synchronous (IStateSync) and asynchronous (IStateAsync) states,
    /// and optional per-frame ticking (IStateTick, IStateFixedTick, IStateLateTick).
    ///
    /// Usage in a MonoBehaviour:
    /// <code>
    /// enum PlayerStateType { Idle, Run, Jump }
    ///
    /// StateMachine&lt;PlayerStateType&gt; sm = new();
    /// sm.Register(PlayerStateType.Idle, new IdleState());
    /// sm.Register(PlayerStateType.Run,  new RunState());
    /// await sm.SetInitialStateAsync(PlayerStateType.Idle);
    ///
    /// void Update()      => sm.Tick();
    /// void FixedUpdate() => sm.FixedTick();
    /// </code>
    /// </summary>
    public class StateMachine<T> where T : Enum
    {
        public T      CurrentType  { get; private set; }
        public T      PreviousType { get; private set; }
        public IState CurrentState { get; private set; }

        private readonly Dictionary<T, IState> _states = new();
        private bool _isTransitioning;
        private bool _isInitialized;

        // (previousState, newState)
        public event Action<T, T> OnStateChanged;

        // ── Registration ──────────────────────────────────────────────────────

        public void Register(T key, IState state)
        {
            if (state == null)
            {
                Debug.LogError($"[StateMachine] Tried to register null state for key '{key}'.");
                return;
            }
            _states[key] = state;
        }

        public bool HasState(T key) => _states.ContainsKey(key);

        /// <summary>Returns the state cast to TState, or null if not found or wrong type.</summary>
        public TState GetState<TState>(T key) where TState : class, IState
        {
            _states.TryGetValue(key, out IState state);
            return state as TState;
        }

        /// <summary>Returns the raw IState for a key.</summary>
        public IState GetState(T key)
        {
            _states.TryGetValue(key, out IState state);
            return state;
        }

        // ── Initialization ────────────────────────────────────────────────────

        /// <summary>Set the starting state. Does not fire OnStateChanged.</summary>
        public async UniTask SetInitialStateAsync(T key)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[StateMachine] SetInitialStateAsync called more than once.");
                return;
            }

            if (!_states.TryGetValue(key, out IState state))
            {
                Debug.LogError($"[StateMachine] State '{key}' is not registered.");
                return;
            }

            _isInitialized = true;
            CurrentType    = key;
            CurrentState   = state;

            await ExecuteEnter(CurrentState);
        }

        /// <summary>Synchronous version for machines with only IStateSync states.</summary>
        public void SetInitialState(T key)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[StateMachine] SetInitialState called more than once.");
                return;
            }

            if (!_states.TryGetValue(key, out IState state))
            {
                Debug.LogError($"[StateMachine] State '{key}' is not registered.");
                return;
            }

            _isInitialized = true;
            CurrentType    = key;
            CurrentState   = state;

            if (state is IStateSync sync)
                sync.Enter();
        }

        // ── Transitions ───────────────────────────────────────────────────────

        public async UniTask ChangeStateAsync(T newState)
        {
            if (_isTransitioning)
                return;

            if (EqualityComparer<T>.Default.Equals(newState, CurrentType))
                return;

            if (!_states.TryGetValue(newState, out IState nextState))
            {
                Debug.LogError($"[StateMachine] State '{newState}' is not registered.");
                return;
            }

            _isTransitioning = true;

            // FIX: try/finally so _isTransitioning is ALWAYS cleared,
            // even if Enter/Exit throws an exception.
            try
            {
                T previousType = CurrentType;

                if (CurrentState != null)
                    await ExecuteExit(CurrentState);

                PreviousType = CurrentType;
                CurrentType  = newState;
                CurrentState = nextState;

                await ExecuteEnter(CurrentState);

                OnStateChanged?.Invoke(previousType, newState);
            }
            catch (Exception e)
            {
                Debug.LogError($"[StateMachine] Exception during transition to '{newState}': {e}");
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        /// <summary>Synchronous version for machines with only IStateSync states.</summary>
        public void ChangeState(T newState)
        {
            if (_isTransitioning)
                return;

            if (EqualityComparer<T>.Default.Equals(newState, CurrentType))
                return;

            if (!_states.TryGetValue(newState, out IState nextState))
            {
                Debug.LogError($"[StateMachine] State '{newState}' is not registered.");
                return;
            }

            _isTransitioning = true;

            try
            {
                T previousType = CurrentType;

                if (CurrentState is IStateSync currentSync)
                    currentSync.Exit();

                PreviousType = CurrentType;
                CurrentType  = newState;
                CurrentState = nextState;

                if (nextState is IStateSync nextSync)
                    nextSync.Enter();

                OnStateChanged?.Invoke(previousType, newState);
            }
            catch (Exception e)
            {
                Debug.LogError($"[StateMachine] Exception during transition to '{newState}': {e}");
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        // ── Tick — call from MonoBehaviour Update / FixedUpdate / LateUpdate ──

        /// <summary>Call this from MonoBehaviour.Update().</summary>
        public void Tick()
        {
            if (CurrentState is IStateTick tick)
                tick.Tick();
        }

        /// <summary>Call this from MonoBehaviour.FixedUpdate().</summary>
        public void FixedTick()
        {
            if (CurrentState is IStateFixedTick fixedTick)
                fixedTick.FixedTick();
        }

        /// <summary>Call this from MonoBehaviour.LateUpdate().</summary>
        public void LateTick()
        {
            if (CurrentState is IStateLateTick lateTick)
                lateTick.LateTick();
        }

        // ── Internal ──────────────────────────────────────────────────────────

        private static async UniTask ExecuteEnter(IState state)
        {
            if (state is IStateAsync asyncState)
                await asyncState.EnterAsync();
            else if (state is IStateSync syncState)
                syncState.Enter();
        }

        private static async UniTask ExecuteExit(IState state)
        {
            if (state is IStateAsync asyncState)
                await asyncState.ExitAsync();
            else if (state is IStateSync syncState)
                syncState.Exit();
        }
    }
}
