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
        PlayBGM(0);
        playA = true;
    }

    void Update()
    {
        if (!IsAnyBgmPlaying() && !stopAllBGM)
         {
             int index = playA ? 0 : 1;
            StopAllBGM();
             stopAllBGM = false;
            PlayBGM(index);
            playA = !playA;
         }
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
