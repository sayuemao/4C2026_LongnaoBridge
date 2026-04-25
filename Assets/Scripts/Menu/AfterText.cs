using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterText : MonoBehaviour
{
    public void EndAfterText()
    {
        gameObject.SetActive(false);
        EndSceneManager.Instance.hasEndDialog = true;
    }
}
