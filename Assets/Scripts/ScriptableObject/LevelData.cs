using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public bool hasSet = false;
    public bool isNotFirstGame;
    public int unlockLevelNumber;
    public int totalLevelNumber;
    [Header("是否播放过对应的对话")]
    public bool dialogBeforeLevel1;
    public bool dialogAfterLevel1;

    public bool dialogBeforeLevel2;
    public bool dialogAfterLevel2;

    public bool dialogBeforeLevel3;

    public bool dialogAfterLevel3;

    [Header("得分记录")]
    public int[] levelMaxScores;
    public int[] levelScores;
}
