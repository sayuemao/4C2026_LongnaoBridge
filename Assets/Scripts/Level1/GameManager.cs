using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool levelComplete = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 保持在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经存在实例，销毁新的对象
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.PlayFadeOut();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelComplete()
    {
        levelComplete = true;
        Debug.Log("Level Complete");
        if(SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("SelectLevel");
        }
        else
        {
            SceneManager.LoadScene("SelectLevel");
        }
    }

    
}
