using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteSceneManager : MonoBehaviour
{
    public static LevelCompleteSceneManager Instance{get;private set;}
    public LevelData levelData;
    public int nowLevelNumber;
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
        
    }
}
