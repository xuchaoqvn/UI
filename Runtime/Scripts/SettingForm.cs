using TMPro;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleFramework.Config;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace SimpleFramework.UI
{
    /// <summary>
    /// 设置面板
    /// </summary>
    public class SettingForm : BaseForm
    {
        #region Field
        /// <summary>
        /// 拖拽区域
        /// </summary>
        [SerializeField]
        [Header("拖拽区域")]
        private EventTrigger m_DragBox;

        /// <summary>
        /// 侧边栏
        /// </summary>
        [SerializeField]
        [Header("侧边栏")]
        private RectTransform m_Sidebar;

        /// <summary>
        /// 内容区
        /// </summary>
        [SerializeField]
        [Header("内容区")]
        private RectTransform m_Content;

        /// <summary>
        /// 底部导航栏
        /// </summary>
        [SerializeField]
        [Header("底部导航栏")]
        private RectTransform m_Footer;

        /// <summary>
        /// 侧边栏按钮预制体
        /// </summary>
        [SerializeField]
        [Header("侧边栏按钮预制体")]
        private GameObject m_SliberUnitPrefab;

        /// <summary>
        /// 内容区预制体
        /// </summary>
        [SerializeField]
        [Header("内容区预制体")]
        private GameObject m_ContentUnitPrefab;

        /// <summary>
        /// 配置项预制体
        /// </summary>
        [SerializeField]
        [Header("配置项预制体")]
        private GameObject[] m_ItemPrefabs;

        /// <summary>
        /// 侧边栏
        /// </summary>
        private Button[] m_SidebarUnits;

        /// <summary>
        /// 内容区域
        /// </summary>
        private RectTransform[] m_ContentBoxs;

        /// <summary>
        /// 拖拽偏移
        /// </summary>
        private Vector2 m_DragOffSet;

        /// <summary>
        /// 配置
        /// </summary>
        private IConfig m_Config;

        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool m_Init = false;
        #endregion

        #region Property
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <value>配置</value>
        public IConfig Config
        {
            set => this.m_Config = value;
        }
        #endregion

        #region Function
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">配置</param>
        public void Init(IConfig config)
        {
            if (this.m_Init)
                return;

            IConfigUnit[] configUnits = config.ConfigUnits;
            this.m_SidebarUnits = new Button[configUnits.Length];
            this.m_ContentBoxs = new RectTransform[configUnits.Length];
            for (int i = 0; i < configUnits.Length; i++)
            {
                int unitIndex = i;
                IConfigUnit unit = configUnits[unitIndex];
                string unitName = unit.Name;

                //侧边栏
                {
                    GameObject sliberUnit = GameObject.Instantiate<GameObject>(this.m_SliberUnitPrefab, this.m_Sidebar);
                    RectTransform sliberUnitRectTransform = sliberUnit.transform as RectTransform;
                    sliberUnitRectTransform.sizeDelta = new Vector2(180.0f, 40.0f);
                    Button sliberUnitButton = sliberUnit.GetComponent<Button>();
                    TextMeshProUGUI sliberUnitText = sliberUnit.GetComponentInChildren<TextMeshProUGUI>();
                    sliberUnitButton.onClick.RemoveAllListeners();
                    sliberUnitButton.onClick.AddListener(() => this.OnSidebarItemClick(unitIndex));
                    sliberUnitText.text = unitName;
                    this.m_SidebarUnits[unitIndex] = sliberUnitButton;
                }

                //内容区
                GameObject unitContent = GameObject.Instantiate<GameObject>(this.m_ContentUnitPrefab, this.m_Content);
                unitContent.name = unitName;
                RectTransform unitRectTransform = unitContent.transform as RectTransform;
                {
                    unitRectTransform.anchorMin = Vector2.zero;
                    unitRectTransform.anchorMax = Vector2.one;
                    unitRectTransform.offsetMin = Vector2.zero;
                    unitRectTransform.offsetMax = Vector2.zero;
                    unitRectTransform.localRotation = Quaternion.identity;
                    unitRectTransform.localScale = Vector3.one;
                    this.m_ContentBoxs[unitIndex] = unitRectTransform;
                }

                RectTransform itemParent = unitRectTransform.GetComponent<ScrollRect>().content;
                VerticalLayoutGroup verticalLayoutGroup = itemParent.GetComponent<VerticalLayoutGroup>();
                float height = verticalLayoutGroup.padding.top + verticalLayoutGroup.spacing * (unit.Items.Length - 1) + verticalLayoutGroup.padding.bottom;

                //配置项
                IConfigItem[] items = unit.Items;
                for (int j = 0; j < items.Length; j++)
                {
                    int itemIndex = j;
                    IConfigItem item = items[itemIndex];
                    Type type = item.Type;
                    UIType uiType = item.UIType;

                    GameObject itemGameObject = GameObject.Instantiate<GameObject>(this.m_ItemPrefabs[(int)uiType], itemParent);
                    TextMeshProUGUI itemName = itemGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    itemGameObject.name = item.Name;
                    itemName.text = item.Name;
                    itemName.rectTransform.sizeDelta = new Vector2(itemName.preferredWidth, itemName.rectTransform.sizeDelta.y);
                    switch (uiType)
                    {
                        case UIType.AnchoredPosition:
                            {
                                TextMeshProUGUI text = itemGameObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                Button button = itemGameObject.transform.GetChild(2).GetComponent<Button>();

                                text.text = item.ValueString;
                                button.onClick.RemoveAllListeners();
                                button.onClick.AddListener(() => 
                                {
                                    text.text.TryParse(out Vector2 anchoredPosition);
                                    UIComponent.Instance.PopAnchoredPosition(anchoredPosition, outAnchoredPosition => text.text = outAnchoredPosition.ToString());
                                });
                            }
                            break;
                        case UIType.Text:
                            {
                                TextMeshProUGUI text = itemGameObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                Button button = itemGameObject.transform.GetChild(2).GetComponent<Button>();

                                text.text = item.ValueString;
                                button.onClick.RemoveAllListeners();
                                button.onClick.AddListener(() => UIComponent.Instance.PopText(text.text, false, outText => text.text = outText));
                            }
                            break;
                        case UIType.Toggle:
                            {
                                Toggle tooggle = itemGameObject.transform.GetChild(1).GetComponent<Toggle>();
                                tooggle.isOn = (item as BaseConfigItem<bool>).Value;
                            }
                            break;
                        case UIType.Slider:
                            {
                                Slider slider = itemGameObject.transform.GetChild(1).GetComponent<Slider>();

                                float value = default;
                                float minValue = default;
                                float maxValue = default;
                                bool wholeNumbers = default;
                                //浮点类型
                                if (type == typeof(float))
                                {
                                    MinMaxConfigItem<float> itemSingle = item as MinMaxConfigItem<float>;
                                    wholeNumbers = false;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }
                                else if (type == typeof(double))
                                {
                                    MinMaxConfigItem<double> itemSingle = item as MinMaxConfigItem<double>;
                                    wholeNumbers = false;
                                    minValue = (float)itemSingle.MinValue;
                                    maxValue = (float)itemSingle.MaxValue;
                                    value = (float)itemSingle.Value;
                                }
                                else if (type == typeof(decimal))
                                {
                                    MinMaxConfigItem<decimal> itemSingle = item as MinMaxConfigItem<decimal>;
                                    wholeNumbers = false;
                                    minValue = (float)itemSingle.MinValue;
                                    maxValue = (float)itemSingle.MaxValue;
                                    value = (float)itemSingle.Value;
                                }
                                //整型类型
                                else if (type == typeof(byte))
                                {
                                    MinMaxConfigItem<byte> itemSingle = item as MinMaxConfigItem<byte>;
                                    wholeNumbers = true;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }
                                else if (type == typeof(sbyte))
                                {
                                    MinMaxConfigItem<sbyte> itemSingle = item as MinMaxConfigItem<sbyte>;
                                    wholeNumbers = true;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }
                                else if (type == typeof(short))
                                {
                                    MinMaxConfigItem<short> itemSingle = item as MinMaxConfigItem<short>;
                                    wholeNumbers = true;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }
                                else if (type == typeof(ushort))
                                {
                                    MinMaxConfigItem<ushort> itemSingle = item as MinMaxConfigItem<ushort>;
                                    wholeNumbers = true;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }
                                else if (type == typeof(int))
                                {
                                    MinMaxConfigItem<int> itemSingle = item as MinMaxConfigItem<int>;
                                    wholeNumbers = true;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }
                                else if (type == typeof(uint))
                                {
                                    MinMaxConfigItem<uint> itemSingle = item as MinMaxConfigItem<uint>;
                                    wholeNumbers = true;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }
                                else if (type == typeof(long))
                                {
                                    MinMaxConfigItem<long> itemSingle = item as MinMaxConfigItem<long>;
                                    wholeNumbers = true;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }
                                else if (type == typeof(ulong))
                                {
                                    MinMaxConfigItem<ulong> itemSingle = item as MinMaxConfigItem<ulong>;
                                    wholeNumbers = true;
                                    minValue = itemSingle.MinValue;
                                    maxValue = itemSingle.MaxValue;
                                    value = itemSingle.Value;
                                }

                                slider.wholeNumbers = wholeNumbers;
                                slider.minValue = minValue;
                                slider.maxValue = maxValue;
                                slider.value = value;
                                //滑动条数值改变事件
                                TextMeshProUGUI sliderTextMeshProUGUI = slider.handleRect.GetComponentInChildren<TextMeshProUGUI>();
                                slider.onValueChanged.RemoveAllListeners();
                                slider.onValueChanged.AddListener(args =>
                                {
                                    if (wholeNumbers)
                                        sliderTextMeshProUGUI.text = args.ToString();
                                    else
                                        sliderTextMeshProUGUI.text = args.ToString("0.00");
                                });
                                slider.onValueChanged.Invoke(value);
                            }
                            break;
                        case UIType.Dropdown:
                            {
                                IOptionsConfigItem optionsConfigItem = item as IOptionsConfigItem;
                                TMP_Dropdown tmp_Dropdown = itemGameObject.transform.GetChild(1).GetComponent<TMP_Dropdown>();
                                tmp_Dropdown.options.Clear();
                                List<TMP_Dropdown.OptionData> optionDatas = tmp_Dropdown.options;
                                for (int k = 0; k < optionsConfigItem.Options.Length; k++)
                                {
                                    string option = optionsConfigItem.Options[k];
                                    optionDatas.Add(new TMP_Dropdown.OptionData(option));
                                }
                                tmp_Dropdown.value = optionsConfigItem.Index;
                            }
                            break;
                        case UIType.File:
                            {
                                TextMeshProUGUI text = itemGameObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                Button file = itemGameObject.transform.GetChild(2).GetComponent<Button>();

                                text.text = item.ValueString;
                                file.onClick.RemoveAllListeners();
                                file.onClick.AddListener(() =>
                                {
                                    string[] filePaths = SFB.StandaloneFileBrowser.OpenFilePanel("选择文件", Path.GetDirectoryName(text.text), string.Empty, false);
                                    if (filePaths == null || filePaths.Length <= 0)
                                        return;
                                    text.text = filePaths[0];
                                });
                            }
                            break;
                        case UIType.Folder:
                            {
                                TextMeshProUGUI text = itemGameObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                Button button = itemGameObject.transform.GetChild(2).GetComponent<Button>();

                                text.text = item.ValueString;
                                button.onClick.RemoveAllListeners();
                                button.onClick.AddListener(() =>
                                {
                                    string[] folderPaths = SFB.StandaloneFileBrowser.OpenFolderPanel("选择文件夹", text.text, false);
                                    if (folderPaths == null || folderPaths.Length <= 0)
                                        return;
                                    text.text = folderPaths[0];
                                });
                            }
                            break;
                        case UIType.Picture:
                            {
                                TextMeshProUGUI text = itemGameObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                Button file = itemGameObject.transform.GetChild(2).GetComponent<Button>();
                                Button detail = itemGameObject.transform.GetChild(3).GetComponent<Button>();

                                text.text = item.ValueString;
                                file.onClick.RemoveAllListeners();
                                detail.onClick.RemoveAllListeners();
                                file.onClick.AddListener(() =>
                                {
                                    string[] filePaths = SFB.StandaloneFileBrowser.OpenFilePanel("选择图片文件", Path.GetDirectoryName(text.text), new SFB.ExtensionFilter[1] { new SFB.ExtensionFilter("图片", "jpg", "png") }, false);
                                    if (filePaths == null || filePaths.Length <= 0)
                                        return;
                                    text.text = filePaths[0];
                                });
                                detail.onClick.AddListener(() => UIComponent.Instance.PopPicture(text.text));
                            }
                            break;
                        case UIType.Video:
                            {
                                TextMeshProUGUI text = itemGameObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                Button file = itemGameObject.transform.GetChild(2).GetComponent<Button>();
                                Button detail = itemGameObject.transform.GetChild(3).GetComponent<Button>();

                                text.text = item.ValueString;
                                file.onClick.RemoveAllListeners();
                                detail.onClick.RemoveAllListeners();
                                file.onClick.AddListener(() =>
                                {
                                    string[] filePaths = SFB.StandaloneFileBrowser.OpenFilePanel("选择视频文件", Path.GetDirectoryName(text.text), "mp4", false);
                                    if (filePaths == null || filePaths.Length <= 0)
                                        return;
                                    text.text = filePaths[0];
                                });
                                detail.onClick.AddListener(() => UIComponent.Instance.PopVideo(text.text));
                            }
                            break;
                        case UIType.Audio:
                            {
                                TextMeshProUGUI text = itemGameObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                Button file = itemGameObject.transform.GetChild(2).GetComponent<Button>();
                                Button detail = itemGameObject.transform.GetChild(3).GetComponent<Button>();

                                text.text = item.ValueString;
                                file.onClick.RemoveAllListeners();
                                detail.onClick.RemoveAllListeners();
                                file.onClick.AddListener(() =>
                                {
                                    string[] filePaths = SFB.StandaloneFileBrowser.OpenFilePanel("选择音频文件", Path.GetDirectoryName(text.text), "mp3", false);
                                    if (filePaths == null || filePaths.Length <= 0)
                                        return;
                                    text.text = filePaths[0];
                                });
                                detail.onClick.AddListener(() => UIComponent.Instance.PopAudio(text.text));
                            }
                            break;
                        default:
                            break;
                    }

                    height += (itemGameObject.transform as RectTransform).rect.size.y;
                }

                height = Mathf.Max(height, unitRectTransform.rect.size.y);
                itemParent.offsetMin = new Vector2(0.0f, -height);
                itemParent.offsetMax = Vector2.zero;
            }

            this.m_Init = true;
        }

        /// <summary>
        /// 注册
        /// </summary>
        protected override void Register()
        {
            this.Init(this.m_Config);

            //侧边栏按钮事件注册
            for (int i = 0; i < this.m_SidebarUnits.Length; i++)
            {
                int index = i;
                Button button = this.m_SidebarUnits[index];
                button.onClick.AddListener(() => this.OnSidebarItemClick(index));
            }

            //拖拽区域
            this.m_DragBox.triggers.Clear();
            EventTrigger.Entry down = new EventTrigger.Entry();
            down.eventID = EventTriggerType.PointerDown;
            down.callback.AddListener(this.OnDragBoxPointDown);
            EventTrigger.Entry drag = new EventTrigger.Entry();
            drag.eventID = EventTriggerType.Drag;
            drag.callback.AddListener(this.OnDragBoxDrag);
            EventTrigger.Entry up = new EventTrigger.Entry();
            up.eventID = EventTriggerType.PointerUp;
            up.callback.AddListener(this.OnDragBoxPointUp);
            this.m_DragBox.triggers.Add(down);
            this.m_DragBox.triggers.Add(drag);
            this.m_DragBox.triggers.Add(up);
            this.m_DragOffSet = Vector2.zero;
            (this.transform as RectTransform).anchoredPosition = Vector2.zero;

            //底边栏事件
            Button[] footerButtons = this.m_Footer.GetComponentsInChildren<Button>();
            footerButtons[0].onClick.AddListener(this.OnDefaultClick);
            footerButtons[1].onClick.AddListener(this.OnSaveClick);
            footerButtons[2].onClick.AddListener(this.Hide);

            this.OnSidebarItemClick(0);
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            //侧边栏取消注册按钮事件
            for (int i = 0; i < this.m_SidebarUnits.Length; i++)
            {
                int index = i;
                Button button = this.m_SidebarUnits[index];
                button.onClick.RemoveAllListeners();
            }

            //拖拽区域
            this.m_DragBox.triggers.Clear();
            this.m_DragOffSet = Vector2.zero;
            (this.transform as RectTransform).anchoredPosition = Vector2.zero;

            //底边栏事件
            Array.ForEach(this.m_Footer.GetComponentsInChildren<Button>(), button => button.onClick.RemoveAllListeners());
        }

        /// <summary>
        /// 当侧边栏按钮被点击时
        /// </summary>
        /// <param name="index">按钮索引</param>
        private void OnSidebarItemClick(int index)
        {
            Color selected = Color.yellow;
            Color noSelected = new Color(135.0f / 255.0f, 135.0f / 255.0f, 135.0f / 255.0f, 1.0f);
            for (int i = 0; i < this.m_SidebarUnits.Length; i++)
            {
                Image sidebarItem = this.m_SidebarUnits[i].targetGraphic as Image;
                if (i == index)
                {
                    sidebarItem.color = selected;
                    this.m_ContentBoxs[i].gameObject.SetActive(true);
                }
                else
                {
                    sidebarItem.color = noSelected;
                    this.m_ContentBoxs[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 当可拖拽区域点按下时
        /// </summary>
        /// <param name="arg0">触摸点数据</param>
        private void OnDragBoxPointDown(BaseEventData arg0)
        {
            PointerEventData pointerEventData = arg0 as PointerEventData;

            RectTransform rectTransform = this.m_DragBox.transform.parent as RectTransform;
            this.m_DragOffSet = rectTransform.anchoredPosition - pointerEventData.pressPosition;
        }

        /// <summary>
        /// 当可拖拽区域点拖拽时
        /// </summary>
        /// <param name="arg0">触摸点数据</param>
        private void OnDragBoxDrag(BaseEventData arg0)
        {
            PointerEventData pointerEventData = arg0 as PointerEventData;

            RectTransform rectTransform = this.m_DragBox.transform.parent as RectTransform;
            rectTransform.anchoredPosition = pointerEventData.position + this.m_DragOffSet;
        }

        /// <summary>
        /// 当可拖拽区域点抬起时
        /// </summary>
        /// <param name="arg0">触摸点数据</param>
        private void OnDragBoxPointUp(BaseEventData arg0) => this.m_DragOffSet = Vector2.zero;

        /// <summary>
        /// 当默认值按钮被点击时
        /// </summary>
        protected virtual void OnDefaultClick()
        {
            IConfigUnit[] configUnits = this.m_Config.ConfigUnits;
            for (int i = 0; i < configUnits.Length; i++)
            {
                int unitIndex = i;
                IConfigUnit unit = configUnits[unitIndex];
                IConfigItem[] items = unit.Items;
                RectTransform content = this.m_ContentBoxs[unitIndex];
                for (int j = 0; j < items.Length; j++)
                {
                    int itemIndex = j;
                    IConfigItem item = items[itemIndex];
                    Transform itemParentTransform = content.GetComponent<ScrollRect>().content.GetChild(itemIndex);
                    UIType uiType = item.UIType;
                    switch (uiType)
                    {
                        case UIType.Toggle:
                            {
                                Toggle tooggle = itemParentTransform.GetChild(1).GetComponent<Toggle>();
                                bool isOn = tooggle.isOn;
                                bool.TryParse(item.DefaultValueString, out isOn);
                                tooggle.isOn = isOn;
                            }
                            break;
                        case UIType.Slider:
                            {
                                Slider slider = itemParentTransform.GetChild(1).GetComponent<Slider>();
                                float value = slider.value;
                                float.TryParse(item.DefaultValueString, out value);
                                slider.value = value;
                            }
                            break;
                        case UIType.Dropdown:
                            {
                                IOptionsConfigItem optionsConfigItem = item as IOptionsConfigItem;
                                TMP_Dropdown tmp_Dropdown = itemParentTransform.GetChild(1).GetComponent<TMP_Dropdown>();
                                tmp_Dropdown.value = optionsConfigItem.DefaultIndex;
                            }
                            break;
                        case UIType.AnchoredPosition:
                        case UIType.Text:
                        case UIType.File:
                        case UIType.Folder:
                        case UIType.Picture:
                        case UIType.Video:
                        case UIType.Audio:
                            {
                                TextMeshProUGUI text = itemParentTransform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                text.text = item.DefaultValueString;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 当保存按钮被点击时
        /// </summary>
        protected virtual void OnSaveClick()
        {
            IConfigUnit[] configUnits = this.m_Config.ConfigUnits;
            for (int i = 0; i < configUnits.Length; i++)
            {
                int unitIndex = i;
                IConfigUnit unit = configUnits[unitIndex];
                IConfigItem[] items = unit.Items;
                RectTransform content = this.m_ContentBoxs[unitIndex];
                for (int j = 0; j < items.Length; j++)
                {
                    int itemIndex = j;
                    IConfigItem item = items[itemIndex];
                    Transform itemParentTransform = content.GetComponent<ScrollRect>().content.GetChild(itemIndex);
                    UIType uiType = item.UIType;
                    switch (uiType)
                    {
                        case UIType.Toggle:
                            {
                                Toggle tooggle = itemParentTransform.GetChild(1).GetComponent<Toggle>();
                                item.ValueString = tooggle.isOn.ToString();
                            }
                            break;
                        case UIType.Slider:
                            {
                                Slider slider = itemParentTransform.GetChild(1).GetComponent<Slider>();
                                item.ValueString = slider.value.ToString();
                            }
                            break;
                        case UIType.Dropdown:
                            {
                                TMP_Dropdown tmp_Dropdown = itemParentTransform.GetChild(1).GetComponent<TMP_Dropdown>();
                                (item as IOptionsConfigItem).Index = tmp_Dropdown.value;
                            }
                            break;
                        case UIType.AnchoredPosition:
                        case UIType.Text:
                        case UIType.File:
                        case UIType.Folder:
                        case UIType.Picture:
                        case UIType.Video:
                        case UIType.Audio:
                            {
                                TextMeshProUGUI text = itemParentTransform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
                                item.ValueString = text.text;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            this.m_Config.Save();
        }

        /// <summary>
        /// 当退出按钮被点击时
        /// </summary>
        protected virtual void OnExitClick() => this.Hide();
        #endregion
    }
}