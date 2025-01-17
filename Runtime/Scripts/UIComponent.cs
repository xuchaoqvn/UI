using UnityEngine;

namespace SimpleFramework.UI
{
    /// <summary>
    /// UI组件
    /// </summary>
    public class UIComponent : BaseUIComponent
    {
        private static UIComponent S_Instance;

        internal static UIComponent Instance
        {
            get
            {
                if (UIComponent.S_Instance == null)
                    UIComponent.S_Instance = GameObject.FindObjectOfType<UIComponent>();
                return UIComponent.S_Instance;
            }
        }
    }
}