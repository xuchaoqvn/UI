using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SimpleFramework.UI
{
    /// <summary>
    /// 消息面板
    /// </summary>
    public class MessageForm : BaseForm
    {
        #region Field
        /// <summary>
        /// 消息物体池
        /// </summary>
        [SerializeField]
        [Header("消息物体池")]
        private RectTransform m_Pool;

        /// <summary>
        /// 消息父物体
        /// </summary>
        [SerializeField]
        [Header("消息父物体")]
        private RectTransform m_MessageParent;

        /// <summary>
        /// 消息停留时间
        /// </summary>
        [SerializeField]
        [Header("消息停留时间")]
        [Range(0.5f, 3.0f)]
        private float m_StayTime = 1.0f;

        /// <summary>
        /// 消息
        /// </summary>
        private Queue<string> m_Messages;

        /// <summary>
        /// 计时器
        /// </summary>
        private float m_Timer;
        #endregion

        private void Update()
        {
            if (this.m_MessageParent.childCount <= 0)
                return;

            this.m_Timer += Time.deltaTime;
            if (this.m_Timer < this.m_StayTime)
                return;

            this.m_Timer = 0.0f;
            this.UpdateMessageItem();
        }

        #region Function
        /// <summary>
        /// 注册
        /// </summary>
        protected override void Register()
        {
            this.m_Messages = new Queue<string>(5);
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            for (int i = 0; i < this.m_MessageParent.childCount; i++)
            {
                int index = i;
                Transform child = this.m_MessageParent.GetChild(index);
                TextMeshProUGUI textMeshProUGUI = child.GetComponentInChildren<TextMeshProUGUI>();
                Button button = child.GetComponent<Button>();
                textMeshProUGUI.text = string.Empty;
                button.onClick.RemoveAllListeners();
                child.SetParent(this.m_Pool);
                child.gameObject.SetActive(false);
                child.gameObject.name = $"Pool Item {this.m_Pool}";
            }

            this.m_Messages.Clear();
            this.m_Messages = null;
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message">消息</param>
        public void ShowMessage(string message)
        {
            base.Show();

            int messageItemCount = this.m_MessageParent.childCount;
            if (messageItemCount < 5)
            {
                Transform child = this.m_Pool.GetChild(0);
                TextMeshProUGUI textMeshProUGUI = child.GetComponentInChildren<TextMeshProUGUI>();
                Button button = child.GetComponent<Button>();
                textMeshProUGUI.text = message;
                child.SetParent(this.m_MessageParent);
                child.gameObject.SetActive(true);
                child.gameObject.name = $"Message Item {messageItemCount}";
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => this.OnMessageItemClick(button));
            }
            else
                this.m_Messages.Enqueue(message);
        }

        /// <summary>
        /// 更新消息物体
        /// </summary>
        private void UpdateMessageItem(int index = 0)
        {
            Transform child = this.m_MessageParent.GetChild(index);
            TextMeshProUGUI textMeshProUGUI = child.GetComponentInChildren<TextMeshProUGUI>();
            Button button = child.GetComponent<Button>();
            if (this.m_Messages.Count > 0)
            {
                textMeshProUGUI.text = this.m_Messages.Dequeue();
                child.SetAsLastSibling();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => this.OnMessageItemClick(button));
            }
            else
            {
                textMeshProUGUI.text = string.Empty;
                button.onClick.RemoveAllListeners();
                child.SetParent(this.m_Pool);
                child.gameObject.SetActive(false);
                child.gameObject.name = $"Pool Item {this.m_Pool}";
            }
        }

        /// <summary>
        /// 当消息物体被点击时
        /// </summary>
        /// <param name="button">按钮</param>
        private void OnMessageItemClick(Button button)
        {
            int index = button.transform.GetSiblingIndex();
            if (index == 0)
                this.m_Timer = 0.0f;

            this.UpdateMessageItem(index);
        }
        #endregion
    }
}