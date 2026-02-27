using UnityEngine.UIElements;

namespace ASS.Core.Editor.Extension
{
    public static class VisualElementExtension
    {
        public static T AddTo<T>(this T visualElement, VisualElement parentVisualElement) where T : VisualElement
        {
            parentVisualElement.Add(visualElement);
            return visualElement;
        }
    }
}