using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Coalballcat.Services
{
    public class StateMachine<T> where T : Enum
    {
        public T CurrentType { get; private set; }
        public IState CurrentState { get; private set; }

        private readonly Dictionary<T, IState> _states = new();
        private bool _isTransitioning;

        public event Action<T> OnStateChanged;

        public void Register(T key, IState state)
        {
            _states[key] = state;
        }

        public IState GetState(T key)
        {
            return _states[key];
        }

        public async UniTask SetInitialStateAsync(T key)
        {
            CurrentType = key;
            CurrentState = _states[key];

            await ExecuteEnter(CurrentState);
        }

        public async UniTask ChangeStateAsync(T newState)
        {
            if (_isTransitioning || EqualityComparer<T>.Default.Equals(newState, CurrentType))
                return;

            _isTransitioning = true;

            if (CurrentState != null)
                await ExecuteExit(CurrentState);

            CurrentType = newState;
            CurrentState = _states[newState];

            await ExecuteEnter(CurrentState);

            OnStateChanged?.Invoke(newState);
            _isTransitioning = false;
        }

        private static async UniTask ExecuteEnter(IState state)
        {
            if (state is IStateAsync asyncState)
                await asyncState.EnterAsync();
            else if (state is IStateNormal stateNormal)
                stateNormal.Enter();
        }

        private static async UniTask ExecuteExit(IState state)
        {
            if (state is IStateAsync asyncState)
                await asyncState.ExitAsync();
            else if (state is IStateNormal stateNormal)
                stateNormal.Exit();
        }
    }
}
