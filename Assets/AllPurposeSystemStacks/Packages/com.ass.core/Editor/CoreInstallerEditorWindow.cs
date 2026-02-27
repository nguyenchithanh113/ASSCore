using System.IO;
using System.Text;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ASS.Core.Editor
{
    public class CoreInstallerEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("ASS/Core/CoreInstallerEditorWindow")]
        public static void ShowExample()
        {
            Validate();
            
            CoreInstallerEditorWindow wnd = GetWindow<CoreInstallerEditorWindow>();
            wnd.titleContent = new GUIContent("CoreInstallerEditorWindow");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
        }
        
        internal static void Validate()
        {
            AddPackageDependency("com.bazyleu.unistate", "https://github.com/bazyleu/UniState.git?path=Assets/UniState");
            AddPackageDependency("com.cysharp.unitask", "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");
            AddPackageDependency("jp.hadashikick.vcontainer", "https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer#1.17.0");
                
            AddPackageDependency("com.cysharp.r3", "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity");
                
            // Force Unity to resolve packages
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
