using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace ASS.Core.Editor
{
    internal class PackagesModel
    {
        public class PackageInfo
        {
            public string name;
            public string url;

            public PackageInfo(string name, string url)
            {
                this.name = name;
                this.url = url;
            }
        }
        
        public class PackageModel
        {
            public string name;
            public bool owned;
        }

        private JObject m_ManinestJson;
        private JObject m_Dependencies;

        private Dictionary<string, string> m_BasePackageDict = new()
        {
            {"com.ass.mvvm",
                "https://github.com/nguyenchithanh113/All-purposeSystemStacks.git?path=Assets/ASS/Packages/com.ass.mvvm"}
        };
        
        public PackageModel[] Packages { get; private set; }
        
        public PackagesModel()
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            string jsonText = File.ReadAllText(manifestPath);

            m_ManinestJson = JObject.Parse(jsonText);
            m_Dependencies = (JObject)m_ManinestJson["dependencies"];

            var packageKeys = m_BasePackageDict.Keys.ToArray();

            Packages = new PackageModel[packageKeys.Length];

            for (int i = 0; i < packageKeys.Length; i++)
            {
                Packages[i] = new PackageModel()
                {
                    name = packageKeys[i],
                    owned = m_Dependencies.ContainsKey(packageKeys[i])
                };
            }
        }
    }
}