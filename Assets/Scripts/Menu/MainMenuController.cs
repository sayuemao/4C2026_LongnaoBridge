using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{
    public string selectLevelSceneName;
    public Image fadeInPanel;
    public float fadeInTime = 1f;
    public Image fadeOutPanel;
    public float fadeOutTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut());
        fadeInPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(SelectLevel());
        }
    }

    IEnumerator SelectLevel()
    {
        yield return StartCoroutine(FadeIn());
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(selectLevelSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        fadeInPanel.gameObject.SetActive(true);
        float t = 0f;
        while (t < fadeInTime)
        {
            fadeInPanel.color = new Color(fadeInPanel.color.r, fadeInPanel.color.g, fadeInPanel.color.b, t);
            t += Time.deltaTime / fadeInTime;
            yield return null;
        }
        //fadeInPanel.gameObject.SetActive(false);
    }
    IEnumerator FadeOut()
    {
        fadeOutPanel.gameObject.SetActive(true);
        float t = 1f;
        while (t > 0f)
        {
            fadeOutPanel.color = new Color(fadeOutPanel.color.r, fadeOutPanel.color.g, fadeOutPanel.color.b, t);
            t -= Time.deltaTime / fadeOutTime;
            yield return null;
        }
        fadeOutPanel.gameObject.SetActive(false);
    }
}
