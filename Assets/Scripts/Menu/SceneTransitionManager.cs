using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Fade Panels")]
    public Image fadeInPanel;
    public Image fadeOutPanel;

    [Header("Settings")]
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(FadeOut());
        fadeInPanel.gameObject.SetActive(false);
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    IEnumerator TransitionCoroutine(string sceneName)
    {
        yield return StartCoroutine(FadeIn());
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
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

    public void PlayFadeOut()
    {
        StartCoroutine(FadeOut());
    }
}
