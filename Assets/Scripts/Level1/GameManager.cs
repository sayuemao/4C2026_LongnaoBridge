using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool levelComplete = false;

    public int pilesCompleted = 0;
    public int pilesShouldComplete = 5;

    public LevelData levelData;

    public bool startSpawnNotes = false;
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

    public void DialogEnd()
    {
        startSpawnNotes = true;
    }
    public void LevelComplete()
    {
        levelComplete = true;
        Time.timeScale = 0.2f; 
        Debug.Log("Level Complete");
        if(pilesCompleted >= pilesShouldComplete)
        {
            if(SceneTransitionManager.Instance != null)
            {
                // 记录得分
                levelData.levelScores[0] = UIManager.Instance.currentScore;
                levelData.levelMaxScores[0] = Mathf.Max(levelData.levelScores[0], levelData.levelMaxScores[0]);
                SceneTransitionManager.Instance.TransitionToScene("Level1Complete");

            }
            else
            {
                SceneManager.LoadScene("Level1Complete");   
            }
        }
        else
        {
            
        }
    }

    public void BackToSelectLevel()
    {
        // 返回选择选择界面
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
