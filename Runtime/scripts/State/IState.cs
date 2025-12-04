using UnityEngine;

namespace Coalballcat.Services
{
    public interface IStateNormal : IState
    {
        void Enter();
        void Exit();
    }

    public interface IState
    {

    }
}