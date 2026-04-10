using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogeUIManager : MonoBehaviour
{
    [SerializeField] private Flowchart flowchart;
    [SerializeField] private string blockName_2_1 = "Level2-1";
    [SerializeField] private string blockName_2_2 = "Level2-2";

    public void StartDialoge()
    {
        if (!DragGameManager.Instance.IsdragGameWin)
        {
            flowchart.ExecuteBlock(blockName_2_1);
        }
        else
        {
            flowchart.ExecuteBlock(blockName_2_2);
        }
    }
}
