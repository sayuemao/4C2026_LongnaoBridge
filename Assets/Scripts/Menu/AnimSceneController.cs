using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimSceneController : MonoBehaviour
{
    public Sprite[] animSprites;
    public Image animImage;

    public enum AnimContinueType
    {
        Click,
        Auto
    }
    public AnimContinueType animType = AnimContinueType.Click;

    [Header("Timing Settings")]
    public float displayDuration = 5f;
    public float fadeDuration = 1.5f;

    [Header("Scene Transition")]
    public string nextSceneName;
    public bool loopSlideshow = false;

    private int currentIndex = 0;
    private bool isTransitioning = false;
    private Coroutine slideshowCoroutine;

    void Start()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.PlayFadeOut();
        }

        if (animSprites == null || animSprites.Length == 0)
        {
            Debug.LogWarning("No sprites assigned to animSprites array!");
            return;
        }

        if (animImage == null)
        {
            Debug.LogWarning("animImage is not assigned!");
            return;
        }

        animImage.sprite = animSprites[0];
        SetImageAlpha(1f);

        if (animType == AnimContinueType.Auto)
        {
            StartSlideshow();
        }
    }

    void Update()
    {
        if (animType == AnimContinueType.Click && Input.anyKey && !isTransitioning)
        {
            NextImage();
        }
    }

    public void StartSlideshow()
    {
        if (slideshowCoroutine != null)
        {
            StopCoroutine(slideshowCoroutine);
        }
        slideshowCoroutine = StartCoroutine(SlideshowRoutine());
    }

    public void StopSlideshow()
    {
        if (slideshowCoroutine != null)
        {
            StopCoroutine(slideshowCoroutine);
            slideshowCoroutine = null;
        }
    }

    public void NextImage()
    {
        if (isTransitioning || animSprites.Length == 0) return;

        int nextIndex = (currentIndex + 1) % animSprites.Length;
        if (nextIndex == 0 && !string.IsNullOrEmpty(nextSceneName))
        {
            isTransitioning = true;
            MoveToNextLevel(nextSceneName);
            return;
        }
        StartCoroutine(TransitionToImage(nextIndex));
    }

    public void PreviousImage()
    {
        if (isTransitioning || animSprites.Length == 0) return;

        int prevIndex = (currentIndex - 1 + animSprites.Length) % animSprites.Length;
        StartCoroutine(TransitionToImage(prevIndex));
    }

    private IEnumerator SlideshowRoutine()
    {
        while (loopSlideshow || currentIndex < animSprites.Length - 1)
        {
            yield return new WaitForSeconds(displayDuration);

            if (!isTransitioning)
            {
                int nextIndex = (currentIndex + 1) % animSprites.Length;

                if (!loopSlideshow && nextIndex == 0)
                {
                    break;
                }

                yield return StartCoroutine(TransitionToImage(nextIndex));
            }
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            MoveToNextLevel(nextSceneName);
        }
    }

    private IEnumerator TransitionToImage(int targetIndex)
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeOut());

        currentIndex = targetIndex;
        animImage.sprite = animSprites[currentIndex];

        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        //Color currentColor = animImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            SetImageAlpha(alpha);
            yield return null;
        }

        SetImageAlpha(0f);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            SetImageAlpha(alpha);
            yield return null;
        }

        SetImageAlpha(1f);
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = animImage.color;
        color.a = alpha;
        animImage.color = color;
    }

    void OnDestroy()
    {
        StopSlideshow();
    }

    private void MoveToNextLevel(string levelName)
    {
        if (SceneTransitionManager.Instance)
        {
            SceneTransitionManager.Instance.TransitionToScene(levelName);
        }
        else
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
