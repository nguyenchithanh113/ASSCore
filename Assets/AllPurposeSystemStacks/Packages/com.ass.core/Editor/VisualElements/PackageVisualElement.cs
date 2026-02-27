using ASS.Core.Editor.Extension;
using UnityEngine.UIElements;

namespace ASS.Core.Editor.VisualElements
{
    public class PackageVisualElement : VisualElement
    {
        //private Label m_NameLabel;
        private Toggle m_InstallToggle;

        public bool InstallToggle => m_InstallToggle.value;
        
        public PackageVisualElement(string name, bool installed)
        {
            //m_NameLabel = new Label(name).AddTo(this);
            m_InstallToggle = new Toggle(name)
            {
                value = installed
            }.AddTo(this);
        }
    }
}