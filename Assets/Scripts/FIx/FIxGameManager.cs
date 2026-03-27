using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FixGameManager : MonoBehaviour
{
    public static FixGameManager Instance { get; private set; }

    [SerializeField]private int totalErrorCount = 3;
    [SerializeField] private float gameOverLoadDelay = 1f;
    public int currentErrorCount = 0;
    enum WeatherType
    {
        Rain,
        River,
        Wind,
    }
    [SerializeField] private WeatherType currentWeather;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void AddError()
    {
        currentErrorCount++;
        if (currentErrorCount >= totalErrorCount)
        {
            GameOver();
        }
    }
    public void GameOver()
    {
        if (currentWeather!=WeatherType.Wind)
        {
           StartCoroutine(DelayLoad());
        }
        else
        {
            Debug.Log("Game Over! You have completed all levels.");
        }
    }
    private IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(gameOverLoadDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);//延迟1s后进入下一关
    }
}
