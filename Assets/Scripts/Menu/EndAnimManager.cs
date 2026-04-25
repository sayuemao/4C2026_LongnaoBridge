using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class EndAnimManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public float playVideoDelay = 1f;
    public float playVideoDelayCounter = 0f;
    public bool isPlaying = false;
    void Start()
    {
        if (SceneTransitionManager.Instance != null)
        {
            StartCoroutine(WaitForFadeOut());
        }
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "½áÎ²ÊÓÆµ.mp4");
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = Camera.main;
        videoPlayer.loopPointReached += OnVideoFinished;
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

    void Update()
    {
        if(!isPlaying)
        {        
            playVideoDelayCounter += Time.deltaTime;
            if (playVideoDelayCounter >= playVideoDelay)
            {
                videoPlayer.Play();
                isPlaying = true;
            }
        }

    }

    IEnumerator WaitForFadeOut()
    {
        yield return SceneTransitionManager.Instance.PlayFadeOut();
        if(!isPlaying)
        {
            videoPlayer.Play();
            isPlaying = true;
        }
    }
}
