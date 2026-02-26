using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace ASS.Core.Editor
{
    public static class ConfigurationManager
    {
        static ConfigurationManager()
        {
            string nugetConfigPath = Path.Combine(Application.dataPath, "NuGet.config");
            string packagesPath = Path.Combine(Application.dataPath, "packages.config");
            
            string nugetConfigTempPath = Path.Combine(Application.dataPath, "ASS/Core/Resources/NuGet.config");
            string packagesTempPath = Path.Combine(Application.dataPath, "ASS/Core/Resources/packages.config");

            bool requireInstallation = false;
            
            if (!File.Exists(nugetConfigPath))
            {
                File.Copy(nugetConfigTempPath, nugetConfigPath);
                requireInstallation = true;
            }

            if (!File.Exists(packagesPath))
            {
                File.Copy(packagesTempPath, packagesPath);
                requireInstallation = true;
            }

            if (requireInstallation)
            {
                AddPackageDependency("com.github-glitchenzo.nugetforunity", "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity");
                AddPackageDependency("com.cysharp.r3", "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity");
            }
        }
        
        public static void AddPackageDependency(string name, string url)
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
            
                // Force Unity to resolve packages
                UnityEditor.PackageManager.Client.Resolve();
                Debug.Log($"Added dependency: {name}@{url}");
            }
            else
            {
                Debug.LogError("Could not find 'dependencies' in manifest.json");
            }
        }
    }
}