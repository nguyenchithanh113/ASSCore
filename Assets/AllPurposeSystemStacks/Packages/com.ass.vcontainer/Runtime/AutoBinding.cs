using System.Collections.Generic;
using ASS.VContainer.Core.Enum;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ASS.VContainer.Runtime
{
    public class AutoBinding : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> m_monoObjects;
        [SerializeField] private BindingType m_bindingType; 

        public void Binding(IContainerBuilder builder)
        {
            foreach (var monoBehaviour in m_monoObjects)
            {
                switch (m_bindingType)
                {
                    case BindingType.AsSelf:
                        builder.RegisterInstance(monoBehaviour).As(monoBehaviour.GetType());
                        break;
                    case BindingType.AsSelfAndImplementedInterfaces:
                        builder.RegisterInstance(monoBehaviour).As(monoBehaviour.GetType()).AsImplementedInterfaces();
                        break;
                }
            }
        }
    }
}