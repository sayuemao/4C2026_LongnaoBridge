using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteSceneManager : MonoBehaviour
{
    public static LevelCompleteSceneManager Instance{get;private set;}
    public LevelData levelData;
    public int nowLevelNumber;
    public bool animComplete = false;
    public string nextSceneName = "SelectLevel";
    public bool isTransition = false;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Time.timeScale = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(SceneTransitionManager.Instance)
        {
            SceneTransitionManager.Instance.PlayFadeOut();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(animComplete&&Input.anyKeyDown)
        {
            if(DataManager.Instance&&!DataManager.Instance.unLockLevel)
            {
                DataManager.Instance.unLockLevel = true;
                DataManager.Instance.levelData.unlockLevelNumber=Mathf.Min(nowLevelNumber+1,DataManager.Instance.levelData.totalLevelNumber);
            }
            // if(nowLevelNumber>=DataManager.Instance.levelData.totalLevelNumber)
            // {
            //     nextSceneName = "EndScene";
            // }
            // else
            // {
            //     nextSceneName = "SelectLevel";
            // }
            if(SceneTransitionManager.Instance&&!isTransition)
            {
                isTransition = true;
                SceneTransitionManager.Instance.TransitionToScene(nextSceneName);         
            }
            else
            {
                if(!isTransition)
                {
                    isTransition = true;
                    SceneManager.LoadScene(nextSceneName);               
                } 
            }
        }
    }
}
