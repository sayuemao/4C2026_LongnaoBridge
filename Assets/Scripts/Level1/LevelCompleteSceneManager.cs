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
            if(SceneTransitionManager.Instance)
            {
                SceneTransitionManager.Instance.TransitionToScene("SelectLevel");
            }
            else
            {
                SceneManager.LoadScene("SelectLevel");
            }
        }
    }
}
