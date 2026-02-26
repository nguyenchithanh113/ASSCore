using System.IO;
using System.Text;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace ASS.Core.Editor
{
    internal class CoreInstallerEditor : EditorWindow
    {
        public static void OpenEditor()
        {
            var editor = EditorWindow.GetWindow<CoreInstallerEditor>();
            editor.Show();

            Validate();
        }
        
        internal static void Validate()
        {
            string nugetConfigPath = Path.Combine(Application.dataPath, "NuGet.config");
            string packagesPath = Path.Combine(Application.dataPath, "packages.config");
            
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
            }

            AssetDatabase.Refresh();
                
            AddPackageDependency("com.bazyleu.unistate", "https://github.com/bazyleu/UniState.git?path=Assets/UniState");
            AddPackageDependency("com.cysharp.unitask", "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");
            AddPackageDependency("jp.hadashikick.vcontainer", "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.17.0");
                
            AddPackageDependency("com.github-glitchenzo.nugetforunity", "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity");
            // Force Unity to resolve packages
            UnityEditor.PackageManager.Client.Resolve();
                
            AddPackageDependency("com.cysharp.r3", "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity");
                
            UnityEditor.PackageManager.Client.Resolve();
                
            Debug.Log("Install Complete");
        }
        
        internal static void AddPackageDependency(string name, string url)
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            string jsonText = File.ReadAllText(manifestPath);

            JObject manifestJson = JObject.Parse(jsonText);
            JObject dependencies = (JObject)manifestJson["dependencies"];

            if (dependencies != null)
            {
                // Add or update the dependency
                dependencies[name] = url;

                // Write the updated JSON back to the file
                File.WriteAllText(manifestPath, manifestJson.ToString(Unity.Plastic.Newtonsoft.Json.Formatting.Indented));
                
                Debug.Log($"Added dependency: {name}@{url}");
            }
            else
            {
                Debug.LogError("Could not find 'dependencies' in manifest.json");
            }
        }
    }
}