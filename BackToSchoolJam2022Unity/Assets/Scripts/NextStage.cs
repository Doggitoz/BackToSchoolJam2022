using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStage : MonoBehaviour
{

    GameManager gm;
    public bool GummyInPlace = false;
    public bool PeppermintInPlace = false;


    void Awake()
    {
        gm = GameManager.GM; 
    }

    // Update is called once per frame
    void Update()
    {
        if (GummyInPlace && PeppermintInPlace)
        {
            Debug.Log("IN PLACE");
            TriggerNextStage();
        }
    }

    public void TriggerNextStage()
    {
        gm.NextScene();
    }
}
