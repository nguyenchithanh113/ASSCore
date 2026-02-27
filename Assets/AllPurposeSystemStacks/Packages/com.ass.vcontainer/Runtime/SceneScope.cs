using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ASS.VContainer.Runtime
{
    public class SceneScope : LifetimeScope
    {
        [SerializeField] private List<GameObject> m_installers;
        
        protected override void Configure(IContainerBuilder builder)
        {
            /*foreach (var installer in m_monoInstallers)
            {
                installer.Install(builder);
            }*/
            
            foreach (var installerGO in m_installers)
            {
                if (installerGO.TryGetComponent(out IInstaller installer))
                {
                    installer.Install(builder);
                }
            }
            
            AutoBindingHandling(builder);
        }

        void AutoBindingHandling(IContainerBuilder builder)
        {
            var autoBindings = FindObjectsByType<AutoBinding>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            //HashSet<GameObject> autoBindingGameObjectSet = new HashSet<GameObject>();

            for (int i = 0; i < autoBindings.Length; i++)
            {
                var autoBinding = autoBindings[i];
                autoBinding.Binding(builder);
                //autoBindingGameObjectSet.Add(autoBinding.gameObject);
            }

            /*var autoInjects = FindObjectsByType<AutoInject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var autoInject in autoInjects)
            {
                autoBindingGameObjectSet.Add(autoInject.gameObject);
            }*/
            
            //autoInjectGameObjects.AddRange(autoBindingGameObjectSet);
        }
    }
}