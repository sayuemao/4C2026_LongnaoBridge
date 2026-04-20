using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance{get;private set;}
    public LevelData levelData;
    public LevelData originalLevelData;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
#if UNITY_EDITOR
        Debug.Log("DataManager Start");
        levelData = originalLevelData;
#endif
    }

    private void Start()
    {

        
    }
    private void OnDestroy()
    {
        Instance = null;
        //levelData.isNotFirstGame = false;
    }
    // Start is called before the first frame update
    public void ResetLevelData()
    {
        levelData = originalLevelData;
    }
}
