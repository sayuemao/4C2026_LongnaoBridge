using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource[] soundEffects;
    public AudioSource[] bgm;

    public float bgmVolumn = 0.8f;
    public float soundEffectVolumn = 1.0f;

    private bool playA = true;
    public bool stopAllBGM = false;

    private int currentBgmIndex = -1;

    // WebGL：浏览器隐藏/切标签导致的暂停（此时禁止自动切歌）
    private bool pausedByVisibility = false;

    // 回到前台后短时间抑制自动切歌，避免恢复的那一帧误判
    [SerializeField] private float suppressDurationOnVisible = 0.5f;
    private float suppressAutoSwitchUntil = 0f;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RegisterVisibilityChange(string gameObjectName, string methodName);
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RegisterVisibilityChange(gameObject.name, "OnBrowserVisibilityChange");
#endif

        if (!IsAnyBgmPlaying())
        {
            PlayBGM(0);
            playA = false; // A播完下一首切B
        }
    }

    void Update()
    {
        if (stopAllBGM) return;

        // 浏览器隐藏/切屏期间，不允许触发“自动切歌”
        if (pausedByVisibility) return;

        // 回到前台后短时间抑制（避免那一瞬间 isPlaying=false 误判）
        if (Time.realtimeSinceStartup < suppressAutoSwitchUntil) return;

        // 自动切歌：仅在“真的没在播”时
        if (!IsAnyBgmPlaying())
        {
            int index = playA ? 0 : 1;
            PlayBGM(index);
            playA = !playA;
        }
    }

    // WebGL 回调：state = "hidden" / "visible"
    public void OnBrowserVisibilityChange(string state)
    {
        if (state == "hidden")
        {
            pausedByVisibility = true;
            PauseCurrentBGM();
            return;
        }

        if (state == "visible")
        {
            pausedByVisibility = false;
            ResumeCurrentBGM();
            suppressAutoSwitchUntil = Time.realtimeSinceStartup + suppressDurationOnVisible;
        }
    }

    private void PauseCurrentBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null && bgm[i].isPlaying)
            {
                currentBgmIndex = i;
                bgm[i].Pause();
                return;
            }
        }
    }

    private void ResumeCurrentBGM()
    {
        if (currentBgmIndex < 0 || currentBgmIndex >= bgm.Length) return;

        AudioSource src = bgm[currentBgmIndex];
        if (src == null || src.clip == null) return;

        // UnPause 不会重开、不会切歌，只会恢复同一首
        src.UnPause();
    }

    public void PlaySFX(int soundToPlay)
    {
        soundEffects[soundToPlay].Stop();
        soundEffects[soundToPlay].volume = soundEffectVolumn;
        soundEffects[soundToPlay].loop = false;
        soundEffects[soundToPlay].Play();
    }

    public void PlayBGM(int bgmToPlay)
    {
        if (bgmToPlay < 0 || bgmToPlay >= bgm.Length || bgm[bgmToPlay] == null) return;

        // 停掉其它BGM，避免叠播
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null && i != bgmToPlay && bgm[i].isPlaying)
            {
                bgm[i].Stop();
            }
        }

        currentBgmIndex = bgmToPlay;

        bgm[bgmToPlay].Stop();
        bgm[bgmToPlay].volume = bgmVolumn;
        bgm[bgmToPlay].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null && bgm[i].isPlaying) bgm[i].Stop();
        }
        stopAllBGM = true;
    }

    private bool IsAnyBgmPlaying()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null && bgm[i].isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    public void PlaySFXLoop(int soundToPlay)
    {
        if (soundToPlay < 0 || soundToPlay >= soundEffects.Length || soundEffects[soundToPlay] == null) return;

        soundEffects[soundToPlay].Stop();
        soundEffects[soundToPlay].loop = true;
        soundEffects[soundToPlay].volume = soundEffectVolumn;
        soundEffects[soundToPlay].Play();
    }

    public void StopSFXLoop(int soundToStop)
    {
        if (soundToStop < 0 || soundToStop >= soundEffects.Length || soundEffects[soundToStop] == null) return;
        soundEffects[soundToStop].Stop();
    }
}
