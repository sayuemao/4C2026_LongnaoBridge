using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public string selectLevelSceneName;

    void Start()
    {
        SceneTransitionManager.Instance.PlayFadeOut();
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
