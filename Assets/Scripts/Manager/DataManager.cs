using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public LevelData levelData;
    public LevelData originalLevelData;

    public bool hasSet = false;

    public bool hasPlayStartAnim = false;

    public bool unLockLevel = false;

    ///<summary>
    /// 当前关卡编号（仅用于SelectLevel场景，从1开始）
    ///</summary>
    public int nowLevelNumber = 1;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
//#if UNITY_EDITOR
        if (!hasSet)
        {
            Debug.Log("DataManager Start");
            CopyLevelData(originalLevelData, levelData);
            hasSet = true;
        }
//#endif
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
        CopyLevelData(originalLevelData, levelData);
    }

    private void CopyLevelData(LevelData source, LevelData target)
    {
        if (source == null || target == null)
        {
            Debug.LogError("Source or target LevelData is null");
            return;
        }

        // 复制基础字段
        target.isNotFirstGame = source.isNotFirstGame;
        target.unlockLevelNumber = source.unlockLevelNumber;
        target.totalLevelNumber = source.totalLevelNumber;
        target.dialogBeforeLevel1 = source.dialogBeforeLevel1;
        target.dialogAfterLevel1 = source.dialogAfterLevel1;
        target.dialogBeforeLevel2 = source.dialogBeforeLevel2;
        target.dialogAfterLevel2 = source.dialogAfterLevel2;
        target.dialogBeforeLevel3 = source.dialogBeforeLevel3;
        target.dialogAfterLevel3 = source.dialogAfterLevel3;

        // 复制数组（深拷贝）
        if (source.levelMaxScores != null)
        {
            target.levelMaxScores = new int[source.levelMaxScores.Length];
            System.Array.Copy(source.levelMaxScores, target.levelMaxScores, source.levelMaxScores.Length);
        }

        if (source.levelScores != null)
        {
            target.levelScores = new int[source.levelScores.Length];
            System.Array.Copy(source.levelScores, target.levelScores, source.levelScores.Length);
        }

        levelData.hasSet = true;
    }
}
