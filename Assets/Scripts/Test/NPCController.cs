using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Fungus;
public class NPCController : MonoBehaviour
{
    public String ChatName;

    private bool canChat = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Say();
        }
    }
    private void OnMouseDown()
    {
        Say();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        canChat = true;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        canChat = false;
    }

    void Say()
    {
        if(canChat)
        {
            Flowchart flowChart = GameObject.Find("Flowchart").GetComponent<Flowchart>();
            if(flowChart.HasBlock(ChatName))
            {
                flowChart.ExecuteBlock(ChatName);
            }
        }
    }
}
