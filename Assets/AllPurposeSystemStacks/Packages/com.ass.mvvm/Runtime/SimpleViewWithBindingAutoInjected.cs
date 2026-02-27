using R3;
using UnityEngine;
using VContainer;

namespace ASS.Mvvm.Runtime
{
    public abstract class SimpleViewWithBindingAutoInjected<TViewModel> : MonoBehaviour
    {
        protected TViewModel m_ViewModel;

        protected CompositeDisposable m_bindings = new();

        [Inject]
        protected virtual void Construct(TViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        protected abstract void Binding(TViewModel viewModel);

        protected virtual void Start()
        {
            Binding(m_ViewModel);
        }

        protected virtual void OnDestroy()
        {
            m_bindings.Dispose();
        }
    }
}