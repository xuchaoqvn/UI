using TMPro;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

namespace SimpleFramework.UI
{
    /// <summary>
    /// 音频弹出面板
    /// </summary>
    public class AudioPopForm : BasePopForm
    {
        /// <summary>
        /// 音频控制
        /// </summary>
        [Serializable]
        private class AudioControl
        {
            #region Field
            /// <summary>
            /// 控制面板画布组
            /// </summary>
            [SerializeField]
            [Header("控制面板画布组")]
            private CanvasGroup m_CanvasGroup = default;

            /// <summary>
            /// 音频进度条
            /// </summary>
            [SerializeField]
            [Header("音频进度条")]
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
            /// 音频时间
            /// </summary>
            [SerializeField]
            [Header("音频时间")]
            private TextMeshProUGUI m_Time = default;

            /// <summary>
            /// 音频名称
            /// </summary>
            [SerializeField]
            [Header("音频名称")]
            private TextMeshProUGUI m_VideoName = default;
            #endregion

            #region Property
            /// <summary>
            /// 获取控制画布组
            /// </summary>
            /// <value>控制画布组</value>
            public CanvasGroup CanvasGroup => this.m_CanvasGroup;

            /// <summary>
            /// 获取音频进度条
            /// </summary>
            /// <value>音频进度条</value>
            public Slider AudioProgress => this.m_VideoProgress;

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
            /// 获取音频时间文本
            /// </summary>
            /// <value>音频时间文本</value>
            public TextMeshProUGUI Time => this.m_Time;

            /// <summary>
            /// 获取音频名称文本
            /// </summary>
            /// <value>音频名称文本</value>
            public TextMeshProUGUI AudioName => this.m_VideoName;
            #endregion
        }

        #region Field
        /// <summary>
        /// 音源
        /// </summary>
        [SerializeField]
        [Header("音源")]
        private AudioSource m_AudioSource;

        /// <summary>
        /// 音频波浪线材质球
        /// </summary>
        [SerializeField]
        [Header("音频波浪线材质球")]
        private Material m_AudioWaveLineMaterial;

        /// <summary>
        /// 提示文本
        /// </summary>
        [SerializeField]
        [Header("提示文本")]
        private TextMeshProUGUI m_TextMeshProUGUI;

        /// <summary>
        /// 音频控制面板
        /// </summary>
        [SerializeField]
        [Header("音频控制面板")]
        private AudioControl m_Control = default;

        /// <summary>
        /// 音频地址
        /// </summary>
        private string m_AudioPath = default;

        /// <summary>
        /// 音频片段
        /// </summary>
        private AudioClip m_AudioClip = default;

        /// <summary>
        /// 是否加载完成
        /// </summary>
        private bool m_LoadFinish;

        /// <summary>
        /// 音频数据
        /// </summary>
        private float[] m_AudioDatas;

        /// <summary>
        /// 拖拽音频进度条状态
        /// 播放：true
        /// 暂停：false 
        /// </summary>
        private bool m_DragVideoProgressState = default;
        #endregion

        #region Property
        /// <summary>
        /// 设置音频地址
        /// </summary>
        /// <value>音频地址</value>
        public string AudioPath
        {
            set
            {
                this.m_AudioPath = value;
            }
        }

        /// <summary>
        /// 设置音频片段
        /// </summary>
        /// <value>音频片段</value>
        public AudioClip Clip
        {
            set
            {
                this.m_AudioClip = value;
            }
        }
        #endregion

        private void Update()
        {
            if (!this.m_LoadFinish)
                return;

            if (!this.m_AudioSource.isPlaying)
            {
                if (this.m_AudioSource.time >= this.m_AudioSource.clip.length)
                {
                    this.m_AudioSource.time = 0.0f;
                    this.OnAudioFinish();
                }
                return;
            }

            //音频时间
            this.UpdateAudioTimeProgress();

            //音频数据
            this.UpdateAudioData();
        }

        #region Function
        /// <summary>
        /// 注册
        /// </summary>
        protected override void Register()
        {
            base.Register();

            this.RegisterEvent();

            if (!string.IsNullOrEmpty(this.m_AudioPath))
                this.Prepare();
            else if (this.m_AudioClip != null)
            {
                this.m_AudioSource.clip = this.m_AudioClip;
                this.m_Control.AudioName.text = this.m_AudioSource.clip.name;
                this.m_LoadFinish = true;
            }
            else
            {
                this.m_AudioSource.clip = null;
                this.m_TextMeshProUGUI.text = "无效音频！！！";
                this.m_TextMeshProUGUI.fontSize = 50.0f;
                this.m_TextMeshProUGUI.horizontalAlignment = HorizontalAlignmentOptions.Center;
                this.m_TextMeshProUGUI.verticalAlignment = VerticalAlignmentOptions.Middle;
                RectTransform rectTransform = this.m_TextMeshProUGUI.rectTransform;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                this.m_TextMeshProUGUI.gameObject.SetActive(true);

                this.m_LoadFinish = false;
                return;
            }

            this.m_AudioDatas = new float[64];
            this.UpdateAudioData(true);
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        protected override void UnRegister()
        {
            base.UnRegister();

            this.UnRegisterEvent();
            this.UpdateAudioData(true);
            this.m_AudioSource.Stop();
            if (!string.IsNullOrEmpty(this.m_AudioPath))
            {
                Destroy(this.m_AudioSource.clip);
                this.m_AudioSource.clip = null;
            }

            this.m_AudioPath = string.Empty;
            this.m_AudioClip = null;
            this.m_LoadFinish = false;
            Array.Clear(this.m_AudioDatas, 0, this.m_AudioDatas.Length);
            this.m_AudioDatas = null;
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
            this.m_Control.AudioProgress.value = 0.0f;
            EventTrigger eventTrigger = this.m_Control.AudioProgress.GetComponent<EventTrigger>();
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
            Transform mute = this.m_Control.Mute.transform.GetChild(0);
            Transform openVolume = this.m_Control.Mute.transform.GetChild(1);
            this.m_AudioSource.mute = false;
            mute.gameObject.SetActive(false);
            openVolume.gameObject.SetActive(true);
            this.m_AudioSource.volume = 0.5f;
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

            EventTrigger eventTrigger = this.m_Control.AudioProgress.GetComponent<EventTrigger>();
            eventTrigger.triggers.Clear();
        }

        /// <summary>
        /// 预加载
        /// </summary>
        private void Prepare()
        {
            UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(this.m_AudioPath, AudioType.MPEG);
            UnityWebRequestAsyncOperation asyncOperation = uwr.SendWebRequest();
            asyncOperation.completed += @async =>
            {
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    this.m_AudioSource.clip = null;
                    this.m_TextMeshProUGUI.text = $"音频加载错误：{uwr.error}";
                    this.m_TextMeshProUGUI.fontSize = 50.0f;
                    this.m_TextMeshProUGUI.horizontalAlignment = HorizontalAlignmentOptions.Center;
                    this.m_TextMeshProUGUI.verticalAlignment = VerticalAlignmentOptions.Middle;
                    RectTransform rectTransform = this.m_TextMeshProUGUI.rectTransform;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    this.m_TextMeshProUGUI.gameObject.SetActive(true);

                    this.m_LoadFinish = false;
                    return;
                }

                this.m_AudioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
                this.m_LoadFinish = true;

                //音频名称
                string audioName = Path.GetFileName(this.m_AudioPath);
                this.m_Control.AudioName.text = audioName;
                this.m_AudioSource.time = 0.0f;

                this.UpdateAudioTimeProgress();

                uwr.disposeDownloadHandlerOnDispose = true;
                uwr.disposeUploadHandlerOnDispose = true;
                uwr.disposeCertificateHandlerOnDispose = true;
                uwr.Dispose();
            };

        }

        /// <summary>
        /// 更新音频时间进度
        /// </summary>
        private void UpdateAudioTimeProgress()
        {
            float time = this.m_AudioSource.time;
            float maxTime = this.m_AudioSource.clip.length;

            float ratio = time / maxTime;
            this.m_Control.AudioProgress.value = ratio;

            int h = Mathf.FloorToInt(time / 3600.0f);
            time = time % 3600f;
            int m = Mathf.FloorToInt(time / 60.0f);
            int s = Mathf.FloorToInt(time % 60.0f);

            int maxH = Mathf.FloorToInt(maxTime / 3600.0f);
            maxTime = maxTime % 3600f;
            int maxM = Mathf.FloorToInt(maxTime / 60.0f);
            int maxS = Mathf.FloorToInt(maxTime % 60.0f);

            this.m_Control.Time.text = $"{h.ToString("00")}:{m.ToString("00")}:{s.ToString("00")} / {maxH.ToString("00")}:{maxM.ToString("00")}:{maxS.ToString("00")}";
        }

        /// <summary>
        /// 更新音频数据
        /// </summary>
        /// <param name="isReset">是否重置</param>
        private void UpdateAudioData(bool isReset = false)
        {
            if (isReset)
                Array.Clear(this.m_AudioDatas, 0, this.m_AudioDatas.Length);
            else
                this.m_AudioSource.GetSpectrumData(this.m_AudioDatas, 0, FFTWindow.BlackmanHarris);

            this.m_AudioWaveLineMaterial.SetFloatArray("_Points", this.m_AudioDatas);
        }

        /// <summary>
        /// 当播放或暂停按钮被点击时
        /// </summary>
        private void OnPlayOrPauseClick()
        {
            Transform play = this.m_Control.PlayAndPause.transform.GetChild(0);
            Transform pause = this.m_Control.PlayAndPause.transform.GetChild(1);

            if (this.m_AudioSource.isPlaying)
            {
                this.m_AudioSource.Pause();

                play.gameObject.SetActive(true);
                pause.gameObject.SetActive(false);

                this.UpdateAudioData(true);
            }
            else
            {
                this.m_AudioSource.Play();

                play.gameObject.SetActive(false);
                pause.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 当后退按钮被点击时
        /// </summary>
        private void OnBackClick()
        {
            if (!this.m_LoadFinish)
                return;

            float time = this.m_AudioSource.time;
            if (time <= 0.0f)
                return;

            time -= 1.0f;
            if (time < 0.0f)
                time = 0.0f;

            this.m_AudioSource.time = time;
            //this.m_Control.CanvasGroup.interactable = false;
            //this.m_Control.CanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 当前进按钮被点击时
        /// </summary>
        private void OnForwardClick()
        {
            if (!this.m_LoadFinish)
                return;

            float time = this.m_AudioSource.time;
            float maxLength = this.m_AudioSource.clip.length;
            if (time >= maxLength)
                return;

            time += 1.0f;
            if (time >= maxLength)
                time = maxLength;

            this.m_AudioSource.time = time;
            //this.m_Control.CanvasGroup.interactable = false;
            //this.m_Control.CanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 当静音按钮被点击时
        /// </summary>
        private void OnMuteClick()
        {
            Transform mute = this.m_Control.Mute.transform.GetChild(0);
            Transform openVolume = this.m_Control.Mute.transform.GetChild(1);

            if (this.m_AudioSource.mute)
            {
                this.m_AudioSource.mute = false;

                mute.gameObject.SetActive(false);
                openVolume.gameObject.SetActive(true);
            }
            else
            {
                this.m_AudioSource.mute = true;

                mute.gameObject.SetActive(true);
                openVolume.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 当音量滑动条值发生改变时
        /// </summary>
        private void OnVolumeValueChange(float arg0)
        {
            this.m_Control.Volume.value = arg0;
            this.m_AudioSource.volume = arg0;
        }

        /// <summary>
        /// 当视频进度被按下时
        /// </summary>
        private void OnVideoProgressBarPointDown()
        {
            this.m_DragVideoProgressState = this.m_AudioSource.isPlaying;
            if (this.m_DragVideoProgressState)
                this.m_AudioSource.Pause();

            this.UpdateAudioData(true);
            this.OnVideoProgressBarDrag();
        }

        /// <summary>
        /// 当视频进度条拖拽时
        /// </summary>
        private void OnVideoProgressBarDrag()
        {
            float time = this.m_Control.AudioProgress.value * this.m_AudioSource.clip.length;
            this.m_AudioSource.time = time;
        }

        /// <summary>
        /// 当视频进度条点抬起时
        /// </summary>
        private void OnVideoProgressBarPointUp()
        {
            if (this.m_DragVideoProgressState)
            {
                this.m_DragVideoProgressState = false;
                this.m_AudioSource.Play();
            }
        }

        /// <summary>
        /// 当音频播放完成时
        /// </summary>
        private void OnAudioFinish()
        {
            Transform play = this.m_Control.PlayAndPause.transform.GetChild(0);
            Transform pause = this.m_Control.PlayAndPause.transform.GetChild(1);

            play.gameObject.SetActive(true);
            pause.gameObject.SetActive(false);
        }
        #endregion
    }
}