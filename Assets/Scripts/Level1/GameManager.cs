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
    private bool dialogEnd = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetState();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ResetState()
    {
        startSpawnNotes = false;
        dialogEnd = false;
        levelComplete = false;
        pilesCompleted = 0;
        Time.timeScale = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetState();

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.PlayFadeOut();
        }
        if (DataManager.Instance)
        {
            DataManager.Instance.nowLevelNumber = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogEnd && Input.anyKeyDown && !startSpawnNotes)
        {
            if (UIManager.Instance != null && UIManager.Instance.startGameText != null)
            {
                UIManager.Instance.startGameText.gameObject.SetActive(false);
                startSpawnNotes = true;
            }
        }
    }

    public void DialogEnd()
    {
        if (UIManager.Instance != null && UIManager.Instance.startGameText != null)
        {
            UIManager.Instance.startGameText.gameObject.SetActive(true);
            dialogEnd = true;
        }
    }

    public void LevelComplete()
    {
        levelComplete = true;
        Time.timeScale = 0.2f;
        Debug.Log("Level Complete");

        if (pilesCompleted >= pilesShouldComplete)
        {
            if (SceneTransitionManager.Instance != null)
            {
                if (UIManager.Instance != null)
                {
                    levelData.levelScores[0] = UIManager.Instance.currentScore;
                    levelData.levelMaxScores[0] = Mathf.Max(levelData.levelScores[0], levelData.levelMaxScores[0]);
                }
                SceneTransitionManager.Instance.TransitionToScene("Level1Complete");
            }
            else
            {
                SceneManager.LoadScene("Level1Complete");
            }
        }
        else
        {
            StartCoroutine(GameFailBackToSelectLevel());
        }
    }

    private IEnumerator GameFailBackToSelectLevel()
    {
        if (UIManager.Instance != null && UIManager.Instance.startGameText != null)
        {
            UIManager.Instance.startGameText.gameObject.SetActive(true);
            UIManager.Instance.startGameText.text = "‘ŸΩ”‘Ÿ¿˜";
        }
        yield return new WaitForSeconds(0.5f);
        BackToSelectLevel();
    }

    public void BackToSelectLevel()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("SelectLevel");
        }
        else
        {
            SceneManager.LoadScene("SelectLevel");
        }
    }
}
