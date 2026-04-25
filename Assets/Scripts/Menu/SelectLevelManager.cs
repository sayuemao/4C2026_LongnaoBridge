using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Fungus;
using Unity.VisualScripting;
public class SelectLevelManager : MonoBehaviour
{
    public Button[] levelButtons;
    public int nowLevelNumber = 1;

    [Header("缩略图图片")]
    public Sprite[] levelImages;
    public Image levelSelectImage;
    public Text[] levelNames;
    public Text[] levelDescriptions;
    public Text[] levelPlayDescriptions;

    public Image level3Decoration;

    public Canvas LevelSelectCanvas;
    public LevelData levelData;

    public Flowchart flowchart;

    // Start is called before the first frame update
    void Start()
    {
        SelectLevelButton(nowLevelNumber);
        LevelSelectCanvas.gameObject.SetActive(false);
        if (SceneTransitionManager.Instance != null)
        {
            StartCoroutine(WaitForFadeOutAndSetVariable());
        }
        else
        {
            if (DataManager.Instance)
            {
                if (!DataManager.Instance.hasPlayStartAnim)
                {
                    DataManager.Instance.hasPlayStartAnim = true;
                    ExecuteDialog("主角对白1");
                }
                else if (DataManager.Instance.unLockLevel)
                {
                    flowchart.SetBooleanVariable("CharacterDialog1End", true);
                    ExecuteDialog("SelectedNarrator" + DataManager.Instance.levelData.unlockLevelNumber);
                    DataManager.Instance.unLockLevel = false;
                }
                else
                    flowchart.SetBooleanVariable("CharacterDialog1End", true);
            }
            else
                flowchart.SetBooleanVariable("CharacterDialog1End", true);
            
        }
    }

    private IEnumerator WaitForFadeOutAndSetVariable()
    {
        //flowchart.SetBooleanVariable("CharacterDialog1End", false);
        yield return SceneTransitionManager.Instance.PlayFadeOut();
        // FadeOut效果结束后设置Fungus变量
        //Fungus.Flowchart.SetVariable("nowLevelNumber", nowLevelNumber);
        if (DataManager.Instance)
        {
            if (!DataManager.Instance.hasPlayStartAnim)
            {
                DataManager.Instance.hasPlayStartAnim = true;
                ExecuteDialog("主角对白1");
            }
            else if (DataManager.Instance.unLockLevel)
            {
                flowchart.SetBooleanVariable("CharacterDialog1End", true);
                ExecuteDialog("SelectedNarrator" + DataManager.Instance.levelData.unlockLevelNumber);
                DataManager.Instance.unLockLevel = false;
            }
            else
            {
                flowchart.SetBooleanVariable("CharacterDialog1End", true);
            }
        }
        else
            flowchart.SetBooleanVariable("CharacterDialog1End", true);
        
    }

    private void ExecuteDialog(string dialogName)
    {
        flowchart.ExecuteBlock(dialogName);
    }

    void Update()
    {
        if (flowchart.GetBooleanVariable("CharacterDialog1End"))
        {
            LevelSelectCanvas.gameObject.SetActive(true);
        }
    }
    public void SelectLevelButton(int levelNumber)
    {
        if (levelNumber != nowLevelNumber)
        {
            if (levelNumber > levelData.unlockLevelNumber)
            {
                return;
            }
            levelSelectImage.sprite = levelImages[levelNumber - 1];

            levelNames[nowLevelNumber - 1].gameObject.SetActive(false);
            levelDescriptions[nowLevelNumber - 1].gameObject.SetActive(false);
            levelPlayDescriptions[nowLevelNumber - 1].gameObject.SetActive(false);

            levelNames[levelNumber - 1].gameObject.SetActive(true);
            levelDescriptions[levelNumber - 1].gameObject.SetActive(true);
            levelPlayDescriptions[levelNumber - 1].gameObject.SetActive(true);
            nowLevelNumber = levelNumber;

            if (levelNumber == 3)
            {
                level3Decoration.gameObject.SetActive(true);
            }
            else
            {
                level3Decoration.gameObject.SetActive(false);
            }
        }
        //更新按钮样式
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i != nowLevelNumber - 1)
            {
                levelButtons[i].transform.localScale = Vector3.one * 0.8f;
                TextMeshProUGUI tempText = levelButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                tempText.color = new Color(tempText.color.r, tempText.color.g, tempText.color.b, 0.5f);
            }
            else
            {
                levelButtons[i].transform.localScale = Vector3.one;
                TextMeshProUGUI tempText = levelButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                tempText.color = new Color(tempText.color.r, tempText.color.g, tempText.color.b, 1f);
            }
        }
    }

    public void StartGame()
    {
        EnterLevel(nowLevelNumber);
    }
    public void EnterLevel(int nowlevelNumber)
    {
        switch (nowlevelNumber)
        {
            case 1:
                EnterLevel1();
                break;
            case 2:
                EnterLevel2();
                break;
            case 3:
                EnterLevel3();
                break;
        }
    }
    public void EnterLevel1()
    {
        Debug.Log("Enter Level 1");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("Level1");
        }
        else
        {
            SceneManager.LoadScene("Level1");
        }
    }

    public void EnterLevel2()
    {
        Debug.Log("Enter Level 2");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("Level2");
        }
        else
        {
            SceneManager.LoadScene("Level2");
        }
    }
    public void EnterLevel3()
    {
        Debug.Log("Enter Level 3");
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("Level3");
        }
        else
        {
            SceneManager.LoadScene("Level3");
        }
    }

    public void BackToMainMenu()
    {
        // 返回主菜单
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
