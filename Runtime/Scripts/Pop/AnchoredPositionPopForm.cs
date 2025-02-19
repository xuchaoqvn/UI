using TMPro;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleFramework.UI
{
    /// <summary>
    /// UGUI位置弹出面板
    /// </summary>
    public class AnchoredPositionPopForm : BasePopForm
    {
        #region Field
        /// <summary>
        /// 宽度输入
        /// </summary>
        [SerializeField]
        [Header("宽度输入")]
        private TMP_InputField m_WidthInput;

        /// <summary>
        /// 高度输入
        /// </summary>
        [SerializeField]
        [Header("高度输入")]
        private TMP_InputField m_HeightInput;

        /// <summary>
        /// 锚点按钮
        /// </summary>
        [SerializeField]
        [Header("锚点按钮")]
        private Button[] m_AnchorButtons;

        /// <summary>
        /// RectTransform组件
        /// </summary>
        [SerializeField]
        [Header("RectTransform组件")]
        private RectTransform m_Rect;

        /// <summary>
        /// 宽度
        /// </summary>
        [SerializeField]
        [Header("宽度")]
        private RectTransform m_Width;

        /// <summary>
        /// 高度
        /// </summary>
        [SerializeField]
        [Header("高度")]
        private RectTransform m_Height;

        /// <summary>
        /// 水平位置线
        /// </summary>
        private RectTransform m_X;

        /// <summary>
        /// 垂直位置线
        /// </summary>
        private RectTransform m_Y;

        /// <summary>
        /// 水平位置
        /// </summary>
        private TextMeshProUGUI m_AnchoredPositionX;

        /// <summary>
        /// 垂直位置
        /// </summary>
        private TextMeshProUGUI m_AnchoredPositionY;

        /// <summary>
        /// 回调
        /// </summary>
        private Action<Vector2> m_OutputAnchoredPosition;
        #endregion

        #region Property
        /// <summary>
        /// 设置输入位置
        /// </summary>
        /// <value>输入位置</value>
        public Vector2 InputAnchoredPosition
        {
            set => this.m_Rect.anchoredPosition = value;
        }

        /// <summary>
        /// 设置输出位置
        /// </summary>
        /// <value>输出位置</value>
        public Action<Vector2> OutputAnchoredPosition
        {
            set => this.m_OutputAnchoredPosition = value;
        }
        #endregion

        #region Function
        /// <summary>
        /// 注册
        /// </summary>
        protected override void Register()
        {
            base.Register();

            Vector2 boxSize = this.m_Rect.rect.size;
            this.m_WidthInput.text = boxSize.x.ToString();
            this.m_HeightInput.text = boxSize.y.ToString();

            this.m_Width.sizeDelta = new Vector2(Screen.width, 5.0f);
            this.m_Width.anchoredPosition = new Vector2(0.0f, -30.0f);
            this.m_Width.GetComponentInChildren<TextMeshProUGUI>().text = Screen.width.ToString();
            this.m_Height.sizeDelta = new Vector2(5.0f, Screen.height);
            this.m_Height.anchoredPosition = new Vector2(-30.0f, 0.0f);
            this.m_Height.GetComponentInChildren<TextMeshProUGUI>().text = Screen.height.ToString();

            this.m_X = this.m_Rect.GetChild(0).GetComponent<RectTransform>();
            this.m_Y = this.m_Rect.GetChild(1).GetComponent<RectTransform>();
            this.m_AnchoredPositionX = this.m_X.GetComponentInChildren<TextMeshProUGUI>();
            this.m_AnchoredPositionY = this.m_Y.GetComponentInChildren<TextMeshProUGUI>();

            this.RefrashAnchoredPosition(new Vector2(Screen.width, Screen.height), Vector2.zero);

            EventTrigger eventTrigger = this.m_Rect.GetComponent<EventTrigger>();
            eventTrigger.triggers.Clear();
            EventTrigger.Entry drag = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Drag
            };
            drag.callback.AddListener(this.OnRectDrag);
            eventTrigger.triggers.Add(drag);

            this.m_WidthInput.onEndEdit.AddListener(this.OnWidthInputEndIdit);
            this.m_HeightInput.onEndEdit.AddListener(this.OnHeightInputEndIdit);
            for (int i = 0; i < this.m_AnchorButtons.Length; i++)
            {
                int index = i;
                Button button = this.m_AnchorButtons[index];
                button.onClick.AddListener(() => this.OnAnchorClick(index));
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            base.UnRegister();

            EventTrigger eventTrigger = this.m_Rect.GetComponent<EventTrigger>();
            eventTrigger.triggers.Clear();

            this.m_WidthInput.onEndEdit.RemoveListener(this.OnWidthInputEndIdit);
            this.m_HeightInput.onEndEdit.RemoveListener(this.OnHeightInputEndIdit);
            for (int i = 0; i < this.m_AnchorButtons.Length; i++)
            {
                int index = i;
                Button button = this.m_AnchorButtons[index];
                button.onClick.RemoveAllListeners();
            }
            this.m_WidthInput.text = string.Empty;
            this.m_HeightInput.text = string.Empty;

            this.m_OutputAnchoredPosition?.Invoke(this.m_Rect.anchoredPosition);
        }

        /// <summary>
        /// 当RectTransform组件被拖拽时
        /// </summary>
        /// <param name="arg0">事件参数</param>
        private void OnRectDrag(BaseEventData arg0) => this.RefrashAnchoredPosition(new Vector2(Screen.width, Screen.height), (arg0 as PointerEventData).delta);

        /// <summary>
        /// 当宽度输入框结束编辑时
        /// </summary>
        /// <param name="arg0">内容</param>
        private void OnWidthInputEndIdit(string arg0)
        {
            string widthText = arg0;
            string heightText = this.m_HeightInput.text;
            if (string.IsNullOrEmpty(widthText) || string.IsNullOrEmpty(heightText))
                return;

            float width = float.Parse(widthText);
            float height = float.Parse(heightText);
            this.m_Rect.sizeDelta = new Vector2(width, height);
            this.RefrashAnchoredPosition(new Vector2(Screen.width, Screen.height), Vector2.zero);
        }

        /// <summary>
        /// 当高度输入框结束编辑时
        /// </summary>
        /// <param name="arg0">内容</param>
        private void OnHeightInputEndIdit(string arg0)
        {
            string widthText = this.m_WidthInput.text;
            string heightText = arg0;
            if (string.IsNullOrEmpty(widthText) || string.IsNullOrEmpty(heightText))
                return;

            float width = float.Parse(widthText);
            float height = float.Parse(heightText);
            this.m_Rect.sizeDelta = new Vector2(width, height);
            this.RefrashAnchoredPosition(new Vector2(Screen.width, Screen.height), Vector2.zero);
        }

        private void OnAnchorClick(int index)
        {
            Vector2 anchoredPosition = this.m_Rect.anchoredPosition;
            Vector2 halfSize = this.m_Rect.rect.size * 0.5f;
            Vector2 areaSize = new Vector2(Screen.width, Screen.height);
            Vector2 anchor = (this.m_Rect.anchorMin + this.m_Rect.anchorMax) * 0.5f * areaSize;
            Vector2 min = -anchor;
            Vector2 max = areaSize - anchor;

            float width = 100.0f;
            float height = 100.0f;
            if (index == 0)
            {
                width = min.x + halfSize.x;
                height = max.y - halfSize.y;
            }
            else if (index == 1)
            {
                width = min.x + halfSize.x;
                height = max.y * 0.5f;
            }
            else if (index == 2)
            {
                width = min.x + halfSize.x;
                height = min.y + halfSize.y;
            }
            else if (index == 3)
            {
                width = max.x * 0.5f;
                height = max.y - halfSize.y;
            }
            else if (index == 4)
            {
                width = max.x * 0.5f;
                height = max.y * 0.5f;
            }
            else if (index == 5)
            {
                width = max.x * 0.5f;
                height = min.y + halfSize.y;
            }
            else if (index == 6)
            {
                width = max.x - halfSize.x;
                height = max.y - halfSize.y;
            }
            else if (index == 7)
            {
                width = max.x - halfSize.x;
                height = max.y * 0.5f;
            }
            else if (index == 8)
            {
                width = max.x - halfSize.x;
                height = min.y + halfSize.y;
            }
            anchoredPosition = new Vector2(width, height);
            this.m_Rect.anchoredPosition = anchoredPosition;
            this.RefrashAnchoredPosition(areaSize, Vector2.zero);
        }

        /// <summary>
        /// 刷新位置
        /// </summary>
        /// <param name="delta">位移</param>
        private void RefrashAnchoredPosition(Vector2 areaSize, Vector2 delta)
        {
            float scaleRatio = 0.85f;
            Vector2 halfSize = this.m_Rect.rect.size * 0.5f;
            Vector2 anchoredPosition = this.m_Rect.anchoredPosition + delta / scaleRatio;
            Vector2 anchor = (this.m_Rect.anchorMin + this.m_Rect.anchorMax) * 0.5f * areaSize;
            Vector2 min = -anchor;
            Vector2 max = areaSize - anchor;
            anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, min.x + halfSize.x, max.x - halfSize.x);
            anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, min.y + halfSize.y, max.y - halfSize.y);
            this.m_Rect.anchoredPosition = anchoredPosition;
            Vector2 originalPosition = anchor + anchoredPosition;

            this.m_X.sizeDelta = new Vector2(originalPosition.x - halfSize.x, 5);
            this.m_Y.sizeDelta = new Vector2(5, originalPosition.y - halfSize.y);
            this.m_X.anchoredPosition = new Vector2(this.m_X.sizeDelta.x * -0.5f, 0.0f);
            this.m_Y.anchoredPosition = new Vector2(0.0f, this.m_Y.sizeDelta.y * -0.5f);
            this.m_AnchoredPositionX.text = originalPosition.x.ToString();
            this.m_AnchoredPositionY.text = originalPosition.y.ToString();
        }
        #endregion
    }
}