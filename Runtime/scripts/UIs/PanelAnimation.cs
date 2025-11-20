using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Coalballcat.Services.UIs
{

    public abstract class PanelAnimation : MonoBehaviour
    {
        public abstract UniTask PlayOpenAnimation();

        public abstract UniTask PlayCloseAnimation();
    }
}