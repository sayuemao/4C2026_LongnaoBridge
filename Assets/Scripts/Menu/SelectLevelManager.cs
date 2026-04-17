using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
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
    public LevelData levelData;
    // Start is called before the first frame update
    void Start()
    {
        if(SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.PlayFadeOut();
        }
        SelectLevelButton(nowLevelNumber);
    }

    // Update is called once per frame
    void Update()
    {

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
            if (i != nowLevelNumber-1)
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
        if(SceneTransitionManager.Instance != null)
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
        if(SceneTransitionManager.Instance != null)
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
        if(SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene("Level3_1");
        }
        else
        {
            SceneManager.LoadScene("Level3_1");
        }
    }
}
