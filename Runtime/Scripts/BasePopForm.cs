using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SimpleFramework.UI
{
    /// <summary>
    /// 弹出面板基类
    /// </summary>
    public abstract class BasePopForm : BaseForm
    {
        #region Function
        /// <summary>
        /// 屏蔽交互层
        /// </summary>
        [SerializeField]
        [Header("屏蔽交互层")]
        protected Graphic m_MaskLayer;

        /// <summary>
        /// 内容层
        /// </summary>
        [SerializeField]
        [Header("内容层")]
        protected RectTransform m_ContentLayer;

        /// <summary>
        /// 内容弹出动画？
        /// </summary>
        [SerializeField]
        [Header("内容弹出动画？")]
        protected bool m_Tween;

        /// <summary>
        /// 内容弹出动画曲线
        /// </summary>
        [SerializeField]
        [Header("内容弹出动画曲线")]
        protected AnimationCurve m_TweenCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        /// <summary>
        /// 退出键
        /// </summary>
        [SerializeField]
        [Header("退出键")]
        protected Button m_Exit;
        #endregion

        #region Function
        /// <summary>
        /// 注册
        /// </summary>
        protected override void Register()
        {
            RectTransform rectTransform = this.m_MaskLayer.rectTransform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            this.m_MaskLayer.raycastTarget = true;

            this.m_Exit.onClick.AddListener(this.Hide);

            if (this.m_Tween)
                this.StartCoroutine(this.TweenContent());
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            this.m_Exit.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 内容层弹出动画
        /// </summary>
        /// <returns>迭代器</returns>
        private IEnumerator TweenContent()
        {
            WaitForEndOfFrame endFrame = new WaitForEndOfFrame();

            float timer = 0.0f;
            float duration = 0.35f;
            this.m_ContentLayer.localScale = Vector3.zero;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float ratio = timer / duration;
                float scale = this.m_TweenCurve.Evaluate(ratio);
                this.m_ContentLayer.localScale = Vector3.one * scale;

                yield return endFrame;
            }

            this.m_ContentLayer.localScale = Vector3.one;
        }
        #endregion
    }
}