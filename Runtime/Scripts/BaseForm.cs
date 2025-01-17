using UnityEngine;
using System.Collections;

namespace SimpleFramework.UI
{
    /// <summary>
    /// 基础面板
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseForm : MonoBehaviour
    {
        /// <summary>
        /// 面板状态
        /// </summary>
        public enum FormState
        {
            /// <summary>
            /// 关闭
            /// </summary>
            Hide,

            /// <summary>
            /// 打开中
            /// </summary>
            Showing,

            /// <summary>
            /// 打开
            /// </summary>
            Show,

            /// <summary>
            /// 关闭中
            /// </summary>
            Hideing
        }

        #region Field
        /// <summary>
        /// 画布组
        /// </summary>
        protected CanvasGroup m_CanvasGroup = default;

        /// <summary>
        /// 面板状态
        /// </summary>
        private FormState m_FormState = FormState.Hide;

        /// <summary>
        /// 淡入淡出
        /// </summary>
        [SerializeField]
        [Header("淡入淡出")]
        protected bool m_Fade = false;
        #endregion

        #region Property
        /// <summary>
        /// 获取画布组
        /// </summary>
        public CanvasGroup CanvasGroup => this.m_CanvasGroup;

        /// <summary>
        /// 获取面板状态
        /// </summary>
        public FormState State => this.m_FormState;
        #endregion

        #region Function
        private void OnEnable()
        {
            this.m_CanvasGroup = this.GetComponent<CanvasGroup>();
            this.Register();
        }

        private void OnDisable()
        {
            this.m_CanvasGroup = null;
            this.UnRegister();
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (this.m_FormState == FormState.Showing || this.m_FormState == FormState.Show)
                return;

            this.m_FormState = FormState.Showing;
            this.gameObject.SetActive(true);
            if (this.m_Fade)
                this.StartCoroutine(this.FadeIn());
            else
            {
                this.m_CanvasGroup.alpha = 1.0f;
                this.m_CanvasGroup.interactable = true;
                this.m_CanvasGroup.blocksRaycasts = true;
                this.m_FormState = FormState.Show;
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            if (this.m_FormState == FormState.Hideing || this.m_FormState == FormState.Hide)
                return;

            this.m_FormState = FormState.Hideing;
            if (this.m_Fade)
                this.StartCoroutine(this.FadeOut());
            else
            {
                this.m_CanvasGroup.alpha = 0.0f;
                this.m_CanvasGroup.interactable = false;
                this.m_CanvasGroup.blocksRaycasts = false;

                this.gameObject.SetActive(false);
                this.m_FormState = FormState.Hide;
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        protected abstract void Register();

        /// <summary>
        /// 取消注册
        /// </summary>
        protected abstract void UnRegister();

        /// <summary>
        /// 淡入
        /// </summary>
        /// <param name="fadeIn">淡入时间</param>
        /// <returns></returns>
        protected IEnumerator FadeIn(float fadeIn = 0.5f)
        {
            this.m_CanvasGroup.alpha = 0.0f;
            this.m_CanvasGroup.interactable = false;
            this.m_CanvasGroup.blocksRaycasts = false;
            float timer = 0.0f;
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            while (this.m_CanvasGroup.alpha < 1.0f)
            {
                timer += Time.deltaTime;
                this.m_CanvasGroup.alpha = timer / fadeIn;
                yield return waitForEndOfFrame;
            }

            this.m_CanvasGroup.alpha = 1.0f;
            this.m_CanvasGroup.interactable = true;
            this.m_CanvasGroup.blocksRaycasts = true;

            this.m_FormState = FormState.Show;
        }

        /// <summary>
        /// 淡出
        /// </summary>
        /// <param name="fadeOut">淡出时间</param>
        /// <returns></returns>
        protected IEnumerator FadeOut(float fadeOut = 0.5f)
        {
            this.m_CanvasGroup.alpha = 1.0f;
            this.m_CanvasGroup.interactable = false;
            this.m_CanvasGroup.blocksRaycasts = false;
            float timer = 0.0f;
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            while (this.m_CanvasGroup.alpha > 0.0f)
            {
                timer += Time.deltaTime;
                this.m_CanvasGroup.alpha = 1.0f - timer / fadeOut;
                yield return waitForEndOfFrame;
            }

            this.m_CanvasGroup.alpha = 0.0f;
            this.gameObject.SetActive(false);

            this.m_FormState = FormState.Hide;
        }

        /// <summary>
        /// 左进
        /// </summary>
        /// <param name="enterTime">进入时间</param>
        /// <returns></returns>
        protected IEnumerator LeftEnter(float enterTime = 0.5f)
        {
            float timer = 0.0f;
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            RectTransform rectTransform = this.transform as RectTransform;
            while (timer < enterTime)
            {
                timer += Time.deltaTime;
                //rectTransform.offsetMin = 
                yield return waitForEndOfFrame;
            }
        }

        /// <summary>
        /// 右出
        /// </summary>
        /// <param name="leaveTime">退出时间</param>
        /// <returns></returns>
        protected IEnumerator RigthLeave(float leaveTime = 0.5f)
        {
            float timer = 0.0f;
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            RectTransform rectTransform = this.transform as RectTransform;
            while (timer < leaveTime)
            {
                timer += Time.deltaTime;
                //rectTransform.offsetMin = 
                yield return waitForEndOfFrame;
            }
        }
        #endregion
    }
}