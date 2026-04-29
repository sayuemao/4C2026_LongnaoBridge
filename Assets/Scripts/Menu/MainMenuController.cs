using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public string nextSceneName;

    public string animSceneName;
    public bool isNotFirstGame;

    private bool shouldTransitionToAnim = true;
    public LevelData levelData;
    void Start()
    {
        levelData = DataManager.Instance.levelData;
        isNotFirstGame = levelData.isNotFirstGame;
        if (!isNotFirstGame)//如果是第一次游戏，不播放转场动画
        {
            levelData.isNotFirstGame = isNotFirstGame = true;
            shouldTransitionToAnim = true;
        }
        else
        {
            shouldTransitionToAnim = false;
            if (SceneTransitionManager.Instance)
                SceneTransitionManager.Instance.PlayFadeOut();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.anyKeyDown)
        {
            EnterSelectLevel();
        }
    }

    void EnterSelectLevel()
    {
        if (!shouldTransitionToAnim)
        {
            SceneTransitionManager.Instance.TransitionToScene(nextSceneName);
        }
        else
        {
            SceneTransitionManager.Instance.TransitionToScene(animSceneName);
        }
    }
}
