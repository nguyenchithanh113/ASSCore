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
            string nugetConfigPath = Path.Combine(Application.dataPath, "NuGet.config");
            string packagesPath = Path.Combine(Application.dataPath, "packages.config");
            
            /*string nugetConfigTempPath = Path.Combine(Application.dataPath, "Packages/ASS/Core/Resources/NuGet.config");
            string packagesTempPath = Path.Combine(Application.dataPath, "Packages/ASS/Core/Resources/packages.config");*/

            bool requireResolve = false;
            
            if (!File.Exists(nugetConfigPath))
            {
                string nugetConfigContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""nuget.org"" value=""https://api.nuget.org/v3/index.json"" enableCredentialProvider=""false"" />
  </packageSources>
  <disabledPackageSources />
  <activePackageSource>
    <add key=""All"" value=""(Aggregate source)"" />
  </activePackageSource>
  <config>
    <add key=""packageInstallLocation"" value=""CustomWithinAssets"" />
    <add key=""repositoryPath"" value=""./Packages"" />
    <add key=""PackagesConfigDirectoryPath"" value=""."" />
    <add key=""slimRestore"" value=""true"" />
    <add key=""PreferNetStandardOverNetFramework"" value=""true"" />
  </config>
</configuration>";
                File.WriteAllText(nugetConfigPath, nugetConfigContent, new UTF8Encoding());
                requireResolve = true;
            }

            if (!File.Exists(packagesPath))
            {
                string packagesContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""Microsoft.Bcl.AsyncInterfaces"" version=""6.0.0"" />
  <package id=""Microsoft.Bcl.TimeProvider"" version=""8.0.0"" />
  <package id=""R3"" version=""1.3.0"" manuallyInstalled=""true"" />
  <package id=""System.ComponentModel.Annotations"" version=""5.0.0"" />
  <package id=""System.Threading.Channels"" version=""8.0.0"" />
</packages>";
                File.WriteAllText(packagesPath, packagesContent, new UTF8Encoding());
                requireResolve = true;
            }
            
            if(requireResolve) AssetDatabase.Refresh();
            
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            string jsonText = File.ReadAllText(manifestPath);

            JObject manifestJson = JObject.Parse(jsonText);
            JObject dependencies = (JObject)manifestJson["dependencies"];
            
            requireResolve |= AddPackageDependency("com.bazyleu.unistate", "https://github.com/bazyleu/UniState.git?path=Assets/UniState", dependencies);
            requireResolve |= AddPackageDependency("com.cysharp.unitask", "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask", dependencies);
            requireResolve |= AddPackageDependency("jp.hadashikick.vcontainer", "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.17.0", dependencies);
            requireResolve |= AddPackageDependency("com.github-glitchenzo.nugetforunity", "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity", dependencies);
            requireResolve |= AddPackageDependency("com.cysharp.r3", "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity", dependencies);

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
