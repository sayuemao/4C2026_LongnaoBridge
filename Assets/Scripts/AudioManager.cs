using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource[] soundEffects;
    public AudioSource[] bgm;

    public float bgmVolumn = 1.0f;

    public float soundEffectVolumn = 1.0f;

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
        
    }

    void Update()
    {
        
    }

    public void PlaySFX(int soundToPlay)
    {
        Debug.Log("Play SFX: " + soundToPlay);
        soundEffects[soundToPlay].Stop();
        soundEffects[soundToPlay].volume = soundEffectVolumn;
        soundEffects[soundToPlay].Play();
    }

    public void PlayBGM(int bgmToPlay)
    {
        Debug.Log("Play BGM: " + bgmToPlay);
        bgm[bgmToPlay].Stop();
        bgm[bgmToPlay].volume = bgmVolumn;
        bgm[bgmToPlay].Play();
    }

}
