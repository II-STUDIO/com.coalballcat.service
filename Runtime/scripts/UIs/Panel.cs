using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Coalballcat.Services.UIs
{
    public class Panel : MonoBehaviour
    {
        [SerializeField] private PanelAnimation anim;

        public bool isOpen { get; private set; }

        public virtual void Open()
        {
            OpenAsync().Forget();
        }

        public virtual void Close()
        {
            CloseAsync().Forget();
        }

        public virtual async UniTask OpenAsync()
        {
            gameObject.SetActive(true);

            if (anim)
            {
                await anim.PlayOpenAnimation();
            }

            isOpen = true;

            OnOpenComplete();
        }

        public virtual async UniTask CloseAsync()
        {
            if (anim)
            {
                await anim.PlayCloseAnimation();
            }

            gameObject.SetActive(false);

            isOpen = false;

            OnCloseComplete();
        }

        protected virtual void OnOpenComplete() { }

        protected virtual void OnCloseComplete() { }
    }
}