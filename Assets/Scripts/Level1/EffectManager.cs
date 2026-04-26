using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    public Animator hammerAnimator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayHitEffect(Vector3 position, JudgeAccuracy accuracy)
    {
        //throw new NotImplementedException();
        Debug.Log("未完成！！！！"+"播放击打特效，位置：" + position + "，判定结果：" + accuracy);
    }

    public void PlayHammerEffect()
    {
        if(hammerAnimator == null) return;
        hammerAnimator.ResetTrigger("Hammer_UpsAndDown");
        hammerAnimator.SetTrigger("Hammer_UpsAndDown");
    }
}
