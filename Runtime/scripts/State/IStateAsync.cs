using Cysharp.Threading.Tasks;

namespace Coalballcat.Services
{
    public interface IStateAsync : IState
    {
        UniTask EnterAsync();
        UniTask ExitAsync();
    }
}