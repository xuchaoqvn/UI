using TMPro;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

namespace SimpleFramework.UI
{
    /// <summary>
    /// 视频弹出面板
    /// </summary>
    public class VideoPopForm : BasePopForm
    {
        /// <summary>
        /// 视频控制
        /// </summary>
        [Serializable]
        private class VideoControl
        {
            #region Field
            /// <summary>
            /// 控制面板画布组
            /// </summary>
            [SerializeField]
            [Header("控制面板画布组")]
            private CanvasGroup m_CanvasGroup = default;

            /// <summary>
            /// 视频进度条
            /// </summary>
            [SerializeField]
            [Header("视频进度条")]
            private Slider m_VideoProgress = default;

            /// <summary>
            /// 播放或暂停按钮
            /// </summary>
            [SerializeField]
            [Header("播放或暂停按钮")]
            private Button m_PlayAndPause = default;

            /// <summary>
            /// 后退
            /// </summary>
            [SerializeField]
            [Header("后退")]
            private Button m_Back = default;

            /// <summary>
            /// 前进
            /// </summary>
            [SerializeField]
            [Header("前进")]
            private Button m_Forward = default;

            /// <summary>
            /// 静音
            /// </summary>
            [SerializeField]
            [Header("静音")]
            private Button m_Mute = default;

            /// <summary>
            /// 音量
            /// </summary>
            [SerializeField]
            [Header("音量")]
            private Slider m_Volume = default;

            /// <summary>
            /// 视频时间
            /// </summary>
            [SerializeField]
            [Header("视频时间")]
            private TextMeshProUGUI m_Time = default;

            /// <summary>
            /// 视频名称
            /// </summary>
            [SerializeField]
            [Header("视频名称")]
            private TextMeshProUGUI m_VideoName = default;
            #endregion

            #region Property
            /// <summary>
            /// 获取控制画布组
            /// </summary>
            /// <value>控制画布组</value>
            public CanvasGroup CanvasGroup => this.m_CanvasGroup;

            /// <summary>
            /// 获取视频进度条
            /// </summary>
            /// <value>视频进度条</value>
            public Slider VideoProgress => this.m_VideoProgress;

            /// <summary>
            /// 获取播放或暂停按钮
            /// </summary>
            /// <value>播放或暂停按钮</value>
            public Button PlayAndPause => this.m_PlayAndPause;

            /// <summary>
            /// 获取后退按钮
            /// </summary>
            /// <value>后退按钮</value>
            public Button Back => this.m_Back;

            /// <summary>
            /// 获取前进按钮
            /// </summary>
            /// <value>前进按钮</value>
            public Button Forward => this.m_Forward;

            /// <summary>
            /// 获取静音按钮
            /// </summary>
            /// <value>静音按钮</value>
            public Button Mute => this.m_Mute;

            /// <summary>
            /// 获取音量滑动条
            /// </summary>
            /// <value>音量滑动条</value>
            public Slider Volume => this.m_Volume;

            /// <summary>
            /// 获取视频时间文本
            /// </summary>
            /// <value>视频时间文本</value>
            public TextMeshProUGUI Time => this.m_Time;

            /// <summary>
            /// 获取视频名称文本
            /// </summary>
            /// <value>视频名称文本</value>
            public TextMeshProUGUI VideoName => this.m_VideoName;
            #endregion
        }

        #region Field
        /// <summary>
        /// 视频播放器
        /// </summary>
        [SerializeField]
        [Header("视频播放器")]
        private VideoPlayer m_VideoPlayer = default;

        /// <summary>
        /// 视频渲染
        /// </summary>
        [SerializeField]
        [Header("视频渲染")]
        private RawImage m_VideoRender = default;

        /// <summary>
        /// 提示文本
        /// </summary>
        [SerializeField]
        [Header("提示文本")]
        private TextMeshProUGUI m_TextMeshProUGUI;

        /// <summary>
        /// 视频控制面板
        /// </summary>
        [SerializeField]
        [Header("视频控制面板")]
        private VideoControl m_Control = default;

        /// <summary>
        /// 视频地址
        /// </summary>
        private string m_VideoPath = default;

        /// <summary>
        /// 视频片段
        /// </summary>
        private VideoClip m_VideoClip = default;

        /// <summary>
        /// 拖拽视频进度条状态
        /// 播放：true
        /// 暂停：false 
        /// </summary>
        private bool m_DragVideoProgressState = default;
        #endregion

        #region Property
        /// <summary>
        /// 设置视频地址
        /// </summary>
        /// <value>视频地址</value>
        public string VideoPath
        {
            set
            {
                this.m_VideoPath = value;
            }
        }

        /// <summary>
        /// 设置视频片段
        /// </summary>
        /// <value>视频片段</value>
        public VideoClip Clip
        {
            set
            {
                this.m_VideoClip = value;
            }
        }
        #endregion

        #region Function
        /// <summary>
        /// 注册
        /// </summary>
        protected override void Register()
        {
            base.Register();

            this.RegisterEvent();

            if (!string.IsNullOrEmpty(this.m_VideoPath))
            {
                this.m_TextMeshProUGUI.text = string.Empty;
                this.m_TextMeshProUGUI.gameObject.SetActive(false);

                this.m_VideoPlayer.source = VideoSource.Url;
                this.m_VideoPlayer.url = this.m_VideoPath;
            }
            else if (this.m_VideoClip != null)
            {
                this.m_TextMeshProUGUI.text = string.Empty;
                this.m_TextMeshProUGUI.gameObject.SetActive(false);

                this.m_VideoPlayer.source = VideoSource.VideoClip;
                this.m_VideoPlayer.clip = this.m_VideoClip;
            }
            else
            {
                this.m_TextMeshProUGUI.text = "无效视频！！！";
                this.m_TextMeshProUGUI.fontSize = 50.0f;
                this.m_TextMeshProUGUI.horizontalAlignment = HorizontalAlignmentOptions.Center;
                this.m_TextMeshProUGUI.verticalAlignment = VerticalAlignmentOptions.Middle;
                RectTransform rectTransform = this.m_TextMeshProUGUI.rectTransform;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                this.m_TextMeshProUGUI.gameObject.SetActive(true);

                return;
            }

            this.m_VideoPlayer.sendFrameReadyEvents = true;
            this.m_VideoPlayer.errorReceived += this.OnErrorReceived;
            this.m_VideoPlayer.prepareCompleted += this.OnPrepareCompleted;
            this.m_VideoPlayer.seekCompleted += this.OnSeekCompleted;
            this.m_VideoPlayer.frameReady += this.OnFrameReady;
            this.m_VideoPlayer.loopPointReached += this.OnLoopPointReached;
            this.m_VideoPlayer.Prepare();
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            base.UnRegister();

            this.UnRegisterEvent();

            this.m_VideoRender.texture = null;
            RenderTexture rt = this.m_VideoPlayer.targetTexture;
            if (rt != null)
            {
                RenderTexture.ReleaseTemporary(rt);
                this.m_VideoPlayer.targetTexture = null;
            }

            this.m_VideoPlayer.errorReceived -= this.OnErrorReceived;
            this.m_VideoPlayer.prepareCompleted -= this.OnPrepareCompleted;
            this.m_VideoPlayer.seekCompleted -= this.OnSeekCompleted;
            this.m_VideoPlayer.frameReady -= this.OnFrameReady;
            this.m_VideoPlayer.loopPointReached -= this.OnLoopPointReached;

            this.m_VideoPath = string.Empty;
            this.m_VideoClip = null;
            this.m_VideoPlayer.Stop();
            this.m_VideoPlayer.clip = null;
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvent()
        {
            this.m_Control.PlayAndPause.onClick.AddListener(this.OnPlayOrPauseClick);
            this.m_Control.Back.onClick.AddListener(this.OnBackClick);
            this.m_Control.Forward.onClick.AddListener(this.OnForwardClick);
            this.m_Control.Mute.onClick.AddListener(this.OnMuteClick);
            this.m_Control.Volume.onValueChanged.AddListener(this.OnVolumeValueChange);

            //视频进度
            this.m_Control.VideoProgress.value = 0.0f;
            EventTrigger eventTrigger = this.m_Control.VideoProgress.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => this.OnVideoProgressBarPointDown());
            eventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) => this.OnVideoProgressBarDrag());
            eventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((data) => this.OnVideoProgressBarPointUp());
            eventTrigger.triggers.Add(entry);

            //播放或暂停
            Transform play = this.m_Control.PlayAndPause.transform.GetChild(0);
            Transform pause = this.m_Control.PlayAndPause.transform.GetChild(1);
            play.gameObject.SetActive(true);
            pause.gameObject.SetActive(false);

            //静音和音量
            AudioSource audioSource = this.m_VideoPlayer.GetTargetAudioSource(0);
            Transform mute = this.m_Control.Mute.transform.GetChild(0);
            Transform openVolume = this.m_Control.Mute.transform.GetChild(1);
            audioSource.mute = false;
            mute.gameObject.SetActive(false);
            openVolume.gameObject.SetActive(true);
            audioSource.volume = 0.5f;
            this.m_Control.Volume.value = 0.5f;
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        private void UnRegisterEvent()
        {
            this.m_Control.PlayAndPause.onClick.AddListener(this.OnPlayOrPauseClick);
            this.m_Control.Back.onClick.AddListener(this.OnBackClick);
            this.m_Control.Forward.onClick.AddListener(this.OnForwardClick);
            this.m_Control.Mute.onClick.AddListener(this.OnMuteClick);
            this.m_Control.Volume.onValueChanged.AddListener(this.OnVolumeValueChange);

            EventTrigger eventTrigger = this.m_Control.VideoProgress.GetComponent<EventTrigger>();
            eventTrigger.triggers.Clear();
        }

        /// <summary>
        /// 当播放或暂停按钮被点击时
        /// </summary>
        private void OnPlayOrPauseClick()
        {
            Transform play = this.m_Control.PlayAndPause.transform.GetChild(0);
            Transform pause = this.m_Control.PlayAndPause.transform.GetChild(1);

            if (this.m_VideoPlayer.isPlaying)
            {
                this.m_VideoPlayer.Pause();

                play.gameObject.SetActive(true);
                pause.gameObject.SetActive(false);
            }
            else if (this.m_VideoPlayer.isPaused)
            {
                this.m_VideoPlayer.Play();

                play.gameObject.SetActive(false);
                pause.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 当后退按钮被点击时
        /// </summary>
        private void OnBackClick()
        {
            if (!this.m_VideoPlayer.isPrepared)
                return;

            double time = this.m_VideoPlayer.time;
            if (time <= 0.0d)
                return;

            time -= 1.0d;
            if (time < 0.0d)
                time = 0.0d;

            this.m_VideoPlayer.time = time;
            this.m_Control.CanvasGroup.interactable = false;
            this.m_Control.CanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 当前进按钮被点击时
        /// </summary>
        private void OnForwardClick()
        {
            if (!this.m_VideoPlayer.isPrepared)
                return;

            double time = this.m_VideoPlayer.time;
            double maxLength = this.m_VideoPlayer.length;
            if (time >= maxLength)
                return;

            time += 1.0d;
            if (time >= maxLength)
                time = maxLength;

            this.m_VideoPlayer.time = time;
            this.m_Control.CanvasGroup.interactable = false;
            this.m_Control.CanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 当静音按钮被点击时
        /// </summary>
        private void OnMuteClick()
        {
            Transform mute = this.m_Control.Mute.transform.GetChild(0);
            Transform openVolume = this.m_Control.Mute.transform.GetChild(1);

            AudioSource audioSource = this.m_VideoPlayer.GetTargetAudioSource(0);
            if (audioSource.mute)
            {
                audioSource.mute = false;

                mute.gameObject.SetActive(false);
                openVolume.gameObject.SetActive(true);
            }
            else
            {
                audioSource.mute = true;

                mute.gameObject.SetActive(true);
                openVolume.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 当音量滑动条值发生改变时
        /// </summary>
        private void OnVolumeValueChange(float arg0)
        {
            AudioSource audioSource = this.m_VideoPlayer.GetTargetAudioSource(0);

            this.m_Control.Volume.value = arg0;
            audioSource.volume = arg0;
        }

        /// <summary>
        /// 当视频进度被按下时
        /// </summary>
        private void OnVideoProgressBarPointDown()
        {
            this.m_DragVideoProgressState = this.m_VideoPlayer.isPlaying;
            if (this.m_DragVideoProgressState)
                this.m_VideoPlayer.Pause();

            this.OnVideoProgressBarDrag();
        }

        /// <summary>
        /// 当视频进度条拖拽时
        /// </summary>
        private void OnVideoProgressBarDrag()
        {
            double time = this.m_Control.VideoProgress.value * this.m_VideoPlayer.length;
            this.m_VideoPlayer.time = time;
        }

        /// <summary>
        /// 当视频进度条点抬起时
        /// </summary>
        private void OnVideoProgressBarPointUp()
        {
            if (this.m_DragVideoProgressState)
            {
                this.m_DragVideoProgressState = false;
                this.m_VideoPlayer.Play();
            }
        }

        /// <summary>
        /// 当视频发生错误时
        /// </summary>
        /// <param name="source">视频播放器</param>
        /// <param name="message">错误信息</param>
        private void OnErrorReceived(VideoPlayer source, string message)
        {
            this.m_TextMeshProUGUI.text = $"视频发生错误：{message}";
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

        /// <summary>
        /// 当预加载完成时
        /// </summary>
        /// <param name="source">视频播放器</param>
        private void OnPrepareCompleted(VideoPlayer source)
        {
            Texture texture = source.texture;
            int width = texture.width;
            int height = texture.height;

            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            source.renderMode = VideoRenderMode.RenderTexture;
            source.targetTexture = rt;
            this.m_VideoRender.texture = rt;
            this.SetVideoNativeSize(rt);

            this.m_VideoPlayer.Pause();

            //视频名称
            if (!string.IsNullOrEmpty(this.m_VideoPath))
            {
                string videoName = Path.GetFileName(this.m_VideoPath);
                this.m_Control.VideoName.text = videoName;
            }
            else if (this.m_VideoClip != null)
                this.m_Control.VideoName.text = this.m_VideoClip.name;
        }

        /// <summary>
        /// 当查找操作完成时
        /// </summary>
        /// <param name="source">视频播放器</param>
        private void OnSeekCompleted(VideoPlayer source)
        {
            this.m_Control.CanvasGroup.interactable = true;
            this.m_Control.CanvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// 当视频帧准备好时
        /// </summary>
        /// <param name="source">视频播放器</param>
        /// <param name="frameIdx">当前视频帧</param>
        private void OnFrameReady(VideoPlayer source, long frameIdx)
        {
            float maxFrame = (float)source.frameCount;
            float ratio = frameIdx / maxFrame;

            this.m_Control.VideoProgress.value = ratio;

            double time = source.time;
            double maxTime = source.length;

            int h = Mathf.FloorToInt((float)time / 3600.0f);
            time = time % 3600d;
            int m = Mathf.FloorToInt((float)time / 60.0f);
            int s = Mathf.FloorToInt((float)time % 60.0f);

            int maxH = Mathf.FloorToInt((float)maxTime / 3600.0f);
            maxTime = maxTime % 3600d;
            int maxM = Mathf.FloorToInt((float)maxTime / 60.0f);
            int maxS = Mathf.FloorToInt((float)maxTime % 60.0f);

            this.m_Control.Time.text = $"{h:00}:{m:00}:{s:00} / {maxH:00}:{maxM:00}:{maxS:00}";
        }

        /// <summary>
        /// 当视频播放完成时
        /// </summary>
        /// <param name="source">视频播放器</param>
        private void OnLoopPointReached(VideoPlayer source)
        {
            Transform play = this.m_Control.PlayAndPause.transform.GetChild(0);
            Transform pause = this.m_Control.PlayAndPause.transform.GetChild(1);

            play.gameObject.SetActive(true);
            pause.gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置视频合适的大小
        /// </summary>
        /// <param name="texture">视频纹理</param>
        private void SetVideoNativeSize(Texture texture)
        {
            Vector2 maxArea = this.m_MaskLayer.rectTransform.rect.size * 0.85f;

            RectTransform rectTransform = this.m_VideoRender.rectTransform;
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