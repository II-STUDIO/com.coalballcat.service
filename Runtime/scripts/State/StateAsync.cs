using Cysharp.Threading.Tasks;

namespace Coalballcat.Services
{
    public abstract class StateAsync : IStateAsync
    {
        public abstract UniTask EnterAsync();

        public abstract UniTask ExitAsync();
    }
}