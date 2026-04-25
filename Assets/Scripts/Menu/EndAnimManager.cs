using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class EndAnimManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public float playVideoDelay = 1f;
    public float playVideoDelayCounter = 0f;
    public bool isPlaying = false;

    [System.Serializable]
    public class VideoTimeEvent
    {
        [Tooltip("触发时间（秒）")]
        public float triggerTime;
        [Tooltip("是否已触发")]
        public bool hasTriggered;
        [Tooltip("到达该时间时执行的事件")]
        public UnityEvent onTimeReached;
    }

    [Tooltip("视频时间点事件列表")]
    public List<VideoTimeEvent> timeEvents = new List<VideoTimeEvent>();

    void Start()
    {
        if (SceneTransitionManager.Instance != null)
        {
            StartCoroutine(WaitForFadeOut());
        }
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "结尾视频.mp4");
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = Camera.main;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Update()
    {
        if (!isPlaying)
        {
            playVideoDelayCounter += Time.deltaTime;
            if (playVideoDelayCounter >= playVideoDelay)
            {
                videoPlayer.Play();
                isPlaying = true;
            }
        }
        else if (videoPlayer.isPlaying)
        {
            CheckTimeEvents();
        }
    }

    void CheckTimeEvents()
    {
        float currentTime = (float)videoPlayer.time;
        foreach (var timeEvent in timeEvents)
        {
            if (!timeEvent.hasTriggered && currentTime >= timeEvent.triggerTime)
            {
                timeEvent.hasTriggered = true;
                timeEvent.onTimeReached?.Invoke();
                Debug.Log($"[EndAnimManager] 视频时间点 {timeEvent.triggerTime}秒 的事件已触发");
            }
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(DelayQuit());
    }

    IEnumerator DelayQuit()
    {
        yield return new WaitForSeconds(1f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator WaitForFadeOut()
    {
        yield return SceneTransitionManager.Instance.PlayFadeOut();
        if (!isPlaying)
        {
            videoPlayer.Play();
            isPlaying = true;
        }
    }

    public void AddTimeEvent(float time, Action callback)
    {
        timeEvents.Add(new VideoTimeEvent
        {
            triggerTime = time,
            hasTriggered = false,
            onTimeReached = new UnityEvent()
        });
        timeEvents[timeEvents.Count - 1].onTimeReached.AddListener(() => callback?.Invoke());
    }

    public void ResetTimeEvents()
    {
        foreach (var timeEvent in timeEvents)
        {
            timeEvent.hasTriggered = false;
        }
    }

    public void SoundStop()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllBGM();
        }
    }
}
