using TMPro;
using System;
using UnityEngine;

namespace SimpleFramework.UI
{
    /// <summary>
    /// 文本弹出面板
    /// </summary>
    public class TextPopForm : BasePopForm
    {
        #region Field
        /// <summary>
        /// 输入框
        /// </summary>
        [SerializeField]
        [Header("输入框")]
        private TMP_InputField m_Input;

        /// <summary>
        /// 回调
        /// </summary>
        private Action<string> m_OutputText;
        #endregion

        #region Property
        /// <summary>
        /// 设置输入文本
        /// </summary>
        /// <value>输入文本</value>
        public string InputText
        {
            set => this.m_Input.text = value;
        }

        /// <summary>
        /// 设置是否只读
        /// </summary>
        /// <value>是否只读</value>
        public bool ReadOnly
        {
            set => this.m_Input.readOnly = value;
        }

        /// <summary>
        /// 设置输出文本
        /// </summary>
        /// <value>输出文本</value>
        public Action<string> OutputText
        {
            set => this.m_OutputText = value;
        }
        #endregion

        #region Function
        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            base.UnRegister();

            if (!this.m_Input.readOnly)
            {
                this.m_OutputText?.Invoke(this.m_Input.text);
                this.m_OutputText = null;
            }
        }
        #endregion
    }
}