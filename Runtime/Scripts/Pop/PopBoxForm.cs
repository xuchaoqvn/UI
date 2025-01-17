using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleFramework.UI
{
    public class PopBoxForm : BasePopForm
    {
        #region Field
        /// <summary>
        /// 标题
        /// </summary>
        [SerializeField]
        [Header("标题")]
        private TextMeshProUGUI m_Title;

        /// <summary>
        /// 标题
        /// </summary>
        [SerializeField]
        [Header("标题")]
        private TextMeshProUGUI m_Content;

        /// <summary>
        /// 按钮预制体
        /// </summary>
        [SerializeField]
        [Header("按钮预制体")]
        private GameObject m_ButtonPrefab;

        /// <summary>
        /// 按钮父物体
        /// </summary>
        [SerializeField]
        [Header("按钮父物体")]
        private RectTransform m_ButtonParent;

        /// <summary>
        /// 回调
        /// </summary>
        private Action<int> m_Callback;

        /// <summary>
        /// 按钮
        /// </summary>
        private Button[] m_Buttons;

        /// <summary>
        /// 索引
        /// </summary>
        private int m_Index;
        #endregion

        #region Property
        /// <summary>
        /// 设置标题
        /// </summary>
        /// <value>标题</value>
        public string Title
        {
            set => this.m_Title.text = value;
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <value>内容</value>
        public string Content
        {
            set => this.m_Content.text = value;
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <value>标题</value>
        public string[] ButtonText
        {
            set => this.UpdateButton(value);
        }

        /// <summary>
        /// 设置回调
        /// </summary>
        /// <value>回调</value>
        public Action<int> Callback
        {
            set => this.m_Callback = value;
        }
        #endregion

        #region Function
        /// <summary>
        /// 注册
        /// </summary>
        protected override void Register()
        {
            base.Register();

            this.m_Index = -1;
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            base.UnRegister();

            for (int i = 0; i < this.m_ButtonParent.childCount; i++)
                Destroy(this.m_ButtonParent.GetChild(i).gameObject);

            this.m_Callback?.Invoke(this.m_Index);
            this.m_Callback = null;
        }

        /// <summary>
        /// 更新按钮
        /// </summary>
        private void UpdateButton(string[] buttonTexts)
        {
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                int index = i;

                GameObject item = GameObject.Instantiate<GameObject>(this.m_ButtonPrefab, this.m_ButtonParent);
                item.name = index.ToString();
                item.gameObject.SetActive(true);
                TextMeshProUGUI textMeshProUGUI = item.GetComponentInChildren<TextMeshProUGUI>();
                textMeshProUGUI.text = buttonTexts[index];

                float width = textMeshProUGUI.preferredWidth + 20.0f;
                if (width < 200.0f)
                    width = 200.0f;
                RectTransform rectTransform = item.transform as RectTransform;
                rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);

                Button button = item.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    this.m_Index = index;
                    this.Hide();
                });
            }
        }
        #endregion
    }
}