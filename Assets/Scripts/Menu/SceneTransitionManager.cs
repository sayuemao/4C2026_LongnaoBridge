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

    private bool isTransitioning = false;
    private bool hasReturnedFromOtherScene = false;

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
        fadeInPanel.gameObject.SetActive(false);
        fadeOutPanel.gameObject.SetActive(false);
        //StartCoroutine(FadeOut());
    }

    public void TransitionToScene(string sceneName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("已经在转场中，忽略重复调用");
            return;
        }
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    IEnumerator TransitionCoroutine(string sceneName)
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeIn());
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        isTransitioning = false;

        if (hasReturnedFromOtherScene)
        {
            PlayFadeOut();
        }
        else
        {
            hasReturnedFromOtherScene = true;
        }
    }

    IEnumerator FadeIn()
    {
        fadeInPanel.gameObject.SetActive(true);
        float t = 0f;
        while (t < fadeInTime)
        {
            fadeInPanel.color = new Color(fadeInPanel.color.r, fadeInPanel.color.g, fadeInPanel.color.b, t);
            t += Time.unscaledDeltaTime / fadeInTime;
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
            t -= Time.unscaledDeltaTime / fadeOutTime;
            yield return null;
        }
        fadeOutPanel.gameObject.SetActive(false);
    }

    public Coroutine PlayFadeOut()
    {
        fadeInPanel.gameObject.SetActive(false);
        return StartCoroutine(FadeOut());
    }
}
