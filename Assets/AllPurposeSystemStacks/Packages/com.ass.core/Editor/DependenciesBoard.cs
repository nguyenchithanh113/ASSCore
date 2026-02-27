using System.Collections.Generic;
using ASS.Core.Editor.Extension;
using ASS.Core.Editor.VisualElements;
using UnityEngine.UIElements;

namespace ASS.Core.Editor
{
    public class DependenciesBoard
    {
        private VisualElement m_Root;
        public VisualElement Root => m_Root;

        private List<PackageVisualElement> m_Packages = new();

        public List<PackageVisualElement> PackageVisualElements => m_Packages;
        
        internal DependenciesBoard(VisualElement rootVisualElement, PackagesModel packagesModel)
        {
            m_Root = rootVisualElement;

            for (int i = 0; i < packagesModel.Packages.Length; i++)
            {
                var packModel = packagesModel.Packages[i];

                PackageVisualElement packageVisualElement =
                    new PackageVisualElement(packModel.name, packModel.owned).AddTo(m_Root);
                
                m_Packages.Add(packageVisualElement);
            }
        }
    }
}