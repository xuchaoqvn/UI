using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SimpleFramework.UI
{
    /// <summary>
    /// UI组件
    /// </summary>
    [RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
    public abstract class BaseUIComponent : MonoBehaviour
    {
        #region Field
        /// <summary>
        /// 设置面板
        /// </summary>
        [SerializeField]
        [Header("设置面板")]
        protected SettingForm m_SettingForm;

        /// <summary>
        /// UGUI位置面板
        /// </summary>
        [SerializeField]
        [Header("UGUI位置面板")]
        protected AnchoredPositionPopForm m_AnchoredPositionPopForm;

        /// <summary>
        /// 文本弹出面板
        /// </summary>
        [SerializeField]
        [Header("文本弹出面板")]
        protected TextPopForm m_TextPopForm;

        /// <summary>
        /// 图片弹出面板
        /// </summary>
        [SerializeField]
        [Header("图片弹出面板")]
        protected PicturePopForm m_PicturePopForm;

        /// <summary>
        /// 视频弹出面板
        /// </summary>
        [SerializeField]
        [Header("视频弹出面板")]
        protected VideoPopForm m_VideoPopForm;

        /// <summary>
        /// 音频弹出面板
        /// </summary>
        [SerializeField]
        [Header("音频弹出面板")]
        protected AudioPopForm m_AudioPopForm;

        /// <summary>
        /// 消息面板
        /// </summary>
        [SerializeField]
        [Header("消息面板")]
        protected MessageForm m_MessageForm;

        /// <summary>
        /// 对话框
        /// </summary>
        [SerializeField]
        [Header("对话框")]
        protected PopBoxForm m_PopBoxForm;
        #endregion

        #region Function
        /// <summary>
        /// 显示设置面板
        /// </summary>
        public void ShowSettingForm() => this.m_SettingForm.Show();

        /// <summary>
        /// 显示弹出UGUI位置
        /// </summary>
        /// <param name="inputAnchoredPosition">UGUI位置</param>
        /// <param name="outAnchoredPosition">回调</param>
        public void PopAnchoredPosition(Vector2 inputAnchoredPosition, Action<Vector2> outAnchoredPosition = null)
        {
            this.m_AnchoredPositionPopForm.InputAnchoredPosition = inputAnchoredPosition;
            this.m_AnchoredPositionPopForm.OutputAnchoredPosition = outAnchoredPosition;
            this.m_AnchoredPositionPopForm.Show();
        }

        /// <summary>
        /// 显示弹出文本框
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="readOnly">只读</param>
        /// <param name="outText">回调</param>
        public void PopText(string text, bool readOnly, Action<string> outText = null)
        {
            this.m_TextPopForm.InputText = text;
            this.m_TextPopForm.ReadOnly = readOnly;
            this.m_TextPopForm.OutputText = outText;
            this.m_TextPopForm.Show();
        }

        /// <summary>
        /// 显示弹出图片框
        /// </summary>
        /// <param name="texture">图片</param>
        public void PopPicture(Texture texture)
        {
            this.m_PicturePopForm.Texture = texture;
            this.m_PicturePopForm.Show();
        }

        /// <summary>
        /// 显示弹出图片框
        /// </summary>
        /// <param name="texture">图片</param>
        public void PopPicture(string picturePath)
        {
            this.m_PicturePopForm.PicturePath = picturePath;
            this.m_PicturePopForm.Show();
        }

        /// <summary>
        /// 显示弹出视频框
        /// </summary>
        /// <param name="clip">视频片段</param>
        public void PopVideo(VideoClip clip)
        {
            this.m_VideoPopForm.Clip = clip;
            this.m_VideoPopForm.Show();
        }

        /// <summary>
        /// 显示弹出视频框
        /// </summary>
        /// <param name="videoPath">视频地址</param>
        public void PopVideo(string videoPath)
        {
            this.m_VideoPopForm.VideoPath = videoPath;
            this.m_VideoPopForm.Show();
        }

        /// <summary>
        /// 显示弹出音频框
        /// </summary>
        /// <param name="clip">音频片段</param>
        public void PopAudio(AudioClip clip)
        {
            this.m_AudioPopForm.Clip = clip;
            this.m_AudioPopForm.Show();
        }

        /// <summary>
        /// 显示弹出音频框
        /// </summary>
        /// <param name="audioPath">音频地址</param>
        public void PopAudio(string audioPath)
        {
            this.m_AudioPopForm.AudioPath = audioPath;
            this.m_AudioPopForm.Show();
        }

        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="message">消息</param>
        public void Message(string message) => this.m_MessageForm.ShowMessage(message);

        /// <summary>
        /// 显示弹出对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="buttonTexts">按钮文本</param>
        /// <param name="callback">回调</param>
        public void PopBox(string title, string content, string[] buttonTexts, Action<int> callback = null)
        {
            this.m_PopBoxForm.Title = title;
            this.m_PopBoxForm.Content = content;
            this.m_PopBoxForm.ButtonText = buttonTexts;
            this.m_PopBoxForm.Callback = callback;
            this.m_PopBoxForm.Show();
        }
        #endregion
    }
}