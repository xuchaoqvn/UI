using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleFramework.UI
{
    /// <summary>
    /// 图片弹出面板
    /// </summary>
    public class PicturePopForm : BasePopForm
    {
        #region Field
        /// <summary>
        /// 图片显示
        /// </summary>
        [SerializeField]
        [Header("图片显示")]
        private RawImage m_Picture;

        /// <summary>
        /// 提示文本
        /// </summary>
        [SerializeField]
        [Header("提示文本")]
        private TextMeshProUGUI m_TextMeshProUGUI;

        /// <summary>
        /// 图片地址
        /// </summary>
        private string m_PicturePath = default;

        /// <summary>
        /// 图片
        /// </summary>
        private Texture m_Texture;
        #endregion

        #region Property
        /// <summary>
        /// 设置图片地址
        /// </summary>
        /// <value>图片地址</value>
        public string PicturePath
        {
            set => this.m_PicturePath = value;
        }


        /// <summary>
        /// 设置图片
        /// </summary>
        /// <value>图片</value>
        public Texture Texture
        {
            set => this.m_Texture = value;
        }
        #endregion

        #region Function
        /// <summary>
        /// 注册
        /// </summary>
        protected override void Register()
        {
            base.Register();

            if (!string.IsNullOrEmpty(this.m_PicturePath))
            {
                Texture2D texture2D = new Texture2D(0, 0);
                texture2D.LoadImage(File.ReadAllBytes(this.m_PicturePath));

                this.m_Picture.texture = texture2D;
                this.m_Picture.gameObject.SetActive(true);
                this.SetPictureNativeSize(texture2D);
                this.m_TextMeshProUGUI.text = string.Empty;
                this.m_TextMeshProUGUI.gameObject.SetActive(false);
            }
            else if (this.m_Texture != null)
            {
                this.m_Picture.texture = this.m_Texture;
                this.m_Picture.gameObject.SetActive(true);
                this.SetPictureNativeSize(this.m_Texture);
                this.m_TextMeshProUGUI.text = string.Empty;
                this.m_TextMeshProUGUI.gameObject.SetActive(false);
            }
            else
            {
                this.m_Picture.texture = null;
                this.m_Picture.gameObject.SetActive(false);
                this.m_TextMeshProUGUI.text = "无效图片！！！";
                this.m_TextMeshProUGUI.fontSize = 50.0f;
                this.m_TextMeshProUGUI.horizontalAlignment = HorizontalAlignmentOptions.Center;
                this.m_TextMeshProUGUI.verticalAlignment = VerticalAlignmentOptions.Middle;
                RectTransform rectTransform = this.m_TextMeshProUGUI.rectTransform;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                this.m_TextMeshProUGUI.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            base.UnRegister();

            if (!string.IsNullOrEmpty(this.m_PicturePath))
                Destroy(this.m_Picture.texture);

            this.m_Texture = null;
            this.m_Picture.texture = null;
            this.m_TextMeshProUGUI.text = string.Empty;
        }

        /// <summary>
        /// 设置图片合适的大小
        /// </summary>
        /// <param name="texture">图片</param>
        private void SetPictureNativeSize(Texture texture)
        {
            Vector2 maxArea = this.m_MaskLayer.rectTransform.rect.size * 0.85f;

            RectTransform rectTransform = this.m_Picture.rectTransform;
            float aspect = texture.width / (float)texture.height;
            Vector2 sizeDelta = new Vector2(texture.width, texture.height);
            float height = maxArea.x / aspect;
            if (height > maxArea.y)
                sizeDelta = new Vector2(maxArea.y * aspect, maxArea.y);
            else
                sizeDelta = new Vector2(maxArea.x, maxArea.x / aspect);
            rectTransform.sizeDelta = sizeDelta;

            rectTransform.anchorMax = rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.offsetMin = sizeDelta * -0.5f;
            rectTransform.offsetMax = sizeDelta * 0.5f;
        }
        #endregion
    }
}