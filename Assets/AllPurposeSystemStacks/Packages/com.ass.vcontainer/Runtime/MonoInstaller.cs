using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ASS.VContainer.Runtime
{
    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        public abstract void Install(IContainerBuilder builder);
    }
}