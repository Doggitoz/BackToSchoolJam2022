using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{

    public Action action;
    public GameObject platform;



    public void TriggerEffect()
    {
        if (action == Action.Platform)
        {
            platform.GetComponent<PlatformScript>().isEnabled = true;
        }
    }

    public void ExitEffect()
    {
        if (action == Action.Platform)
        {
            platform.GetComponent<PlatformScript>().isEnabled = false;
        }
    }

}

public enum Action
{
    Platform
}
