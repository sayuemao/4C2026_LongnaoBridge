using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource[] soundEffects;
    public AudioSource[] bgm;

    public float bgmVolumn = 0.8f;
    public float soundEffectVolumn = 1.0f;

    private bool playA = true;
    public bool stopAllBGM = false;

    private bool hasFocus = true;
    private bool wasPausedByFocus = false;

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
        if (!IsAnyBgmPlaying())
        {
            PlayBGM(0);
            playA = false;
        }
    }

    void Update()
    {
        // 失焦时不做自动切歌，避免切屏后误判重复播放
        if (!hasFocus) return;
        if (stopAllBGM) return;

        if (!IsAnyBgmPlaying())
        {
            int index = playA ? 0 : 1;
            PlayBGM(index);
            playA = !playA;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;

        if (!focus)
        {
            PauseCurrentBGM();
            return;
        }

        // 回焦后优先恢复，不重新Play
        if (wasPausedByFocus)
        {
            ResumePausedBGM();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        hasFocus = !pauseStatus;

        if (pauseStatus)
        {
            PauseCurrentBGM();
        }
        else
        {
            if (wasPausedByFocus)
            {
                ResumePausedBGM();
            }
        }
    }

    private void PauseCurrentBGM()
    {
        wasPausedByFocus = false;

        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null && bgm[i].isPlaying)
            {
                bgm[i].Pause();
                wasPausedByFocus = true;
            }
        }
    }

    private void ResumePausedBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null && bgm[i].clip != null && !bgm[i].isPlaying && bgm[i].time > 0f)
            {
                bgm[i].UnPause();
            }
        }

        wasPausedByFocus = false;
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

        // 先停掉其它BGM，避免叠播
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null && i != bgmToPlay && bgm[i].isPlaying)
            {
                bgm[i].Stop();
            }
        }

        bgm[bgmToPlay].volume = bgmVolumn;
        if (!bgm[bgmToPlay].isPlaying)
        {
            bgm[bgmToPlay].Play();
        }
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
