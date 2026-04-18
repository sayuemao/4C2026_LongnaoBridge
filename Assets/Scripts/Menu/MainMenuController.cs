using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public string selectLevelSceneName;
    public bool isNotFirstGame;
    public LevelData levelData;
    void Start()
    {
        isNotFirstGame = levelData.isNotFirstGame;
        if (!isNotFirstGame)//如果是第一次游戏，不播放转场动画
        {
            levelData.isNotFirstGame = isNotFirstGame = true;
        }
        else
        {
            if (SceneTransitionManager.Instance)
                SceneTransitionManager.Instance.PlayFadeOut();
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            EnterSelectLevel();
        }
    }

    void EnterSelectLevel()
    {
        SceneTransitionManager.Instance.TransitionToScene(selectLevelSceneName);
    }
}
