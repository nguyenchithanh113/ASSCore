using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace ASS.Core.Editor
{
    public class CoreInstallerEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("ASS/Core/Core Installer")]
        public static void ShowExample()
        {
            Validate();
            
            CoreInstallerEditorWindow wnd = GetWindow<CoreInstallerEditorWindow>();
            wnd.titleContent = new GUIContent("Core Installer");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement tree = m_VisualTreeAsset.Instantiate();
            root.Add(tree);

            PackagesModel packagesModel = new PackagesModel();

            var dbView = root.Query("DependenciesBoard").First();
            DependenciesBoard dependenciesBoard = new DependenciesBoard(
                dbView, packagesModel);

            Button updatePackagesButton = root.Query<Button>(className: "update-packages");
            updatePackagesButton.clicked += () =>
            {
                var views = dependenciesBoard.PackageVisualElements;

                PackagesModel.PackageModel[] packageModels = new PackagesModel.PackageModel[packagesModel.Packages.Length];

                for (int i = 0; i < views.Count; i++)
                {
                    PackagesModel.PackageModel model = new PackagesModel.PackageModel()
                    {
                        name = packagesModel.Packages[i].name,
                        url = packagesModel.Packages[i].url,
                        owned = views[i].InstallToggle
                    };
                    packageModels[i] = model;
                }
                
                UpdatePackages(packageModels);
            };
        }

        static void UpdatePackages(PackagesModel.PackageModel[] packageModels)
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            string jsonText = File.ReadAllText(manifestPath);

            JObject manifestJson = JObject.Parse(jsonText);
            JObject dependencies = (JObject)manifestJson["dependencies"];
            
            Assert.IsNotNull(dependencies, "dependencies is null");
            
            for (var i = 0; i < packageModels.Length; i++)
            {
                var packageModel = packageModels[i];

                if (packageModel.owned)
                {
                    dependencies[packageModel.name] = packageModel.url;
                }
                else
                {
                    dependencies.Remove(packageModel.name);
                }
            }
            
            File.WriteAllText(manifestPath, manifestJson.ToString(Unity.Plastic.Newtonsoft.Json.Formatting.Indented));
            AssetDatabase.Refresh();
            UnityEditor.PackageManager.Client.Resolve();
        }
        
        internal static void Validate()
        {
            bool requireResolve = false;
            
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            string jsonText = File.ReadAllText(manifestPath);

            JObject manifestJson = JObject.Parse(jsonText);
            JObject dependencies = (JObject)manifestJson["dependencies"];
            
            requireResolve |= AddPackageDependency("com.bazyleu.unistate", "https://github.com/bazyleu/UniState.git?path=Assets/UniState", dependencies);
            requireResolve |= AddPackageDependency("com.cysharp.unitask", "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask", dependencies);
            requireResolve |= AddPackageDependency("jp.hadashikick.vcontainer", "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.17.0", dependencies);
            requireResolve |= AddPackageDependency("com.github-glitchenzo.nugetforunity", "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity", dependencies);

            if (requireResolve)
            {
                File.WriteAllText(manifestPath, manifestJson.ToString(Unity.Plastic.Newtonsoft.Json.Formatting.Indented));
                AssetDatabase.Refresh();
                // Force Unity to resolve packages
                Client.Resolve();
            }
        }
        
        internal static bool AddPackageDependency(string name, string url, JObject dependencies)
        {
            if (!dependencies.ContainsKey(name))
            {
                dependencies[name] = url;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
