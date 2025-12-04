namespace Coalballcat.Services
{
    public abstract class State : IStateNormal
    {
        public abstract void Enter();

        public abstract void Exit();
    }
}