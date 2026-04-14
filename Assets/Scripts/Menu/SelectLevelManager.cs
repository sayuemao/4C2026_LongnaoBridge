using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SelectLevelManager : MonoBehaviour
{
    public Button Level1Button;
    public Button Level2Button;
    public Button Level3Button;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectLevel1()
    {
        Debug.Log("Select Level 1");
        SceneManager.LoadScene("Level1");
    }

    public void SelectLevel2()
    {
        Debug.Log("Select Level 2");
        SceneManager.LoadScene("Level2");
    }
    public void SelectLevel3()
    {
        Debug.Log("Select Level 3");
        SceneManager.LoadScene("Level3_1");
    }
}
