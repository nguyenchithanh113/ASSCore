using R3;
using UnityEngine;

namespace ASS.Mvvm.Runtime
{
    public abstract class SimpleViewWithBinding<TViewModel> : MonoBehaviour
    {
        protected CompositeDisposable m_bindings = new();
        
        protected abstract void Binding(TViewModel viewModel);
        
        protected virtual void OnDestroy()
        {
            m_bindings.Dispose();
        }
    }
}