using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{

    public Action action;
    public GameObject platform;

    public bool triggeredByGummy = true;
    public bool triggeredByPepper = true;


    public void TriggerEffect(CharType cr)
    {
        if (cr == CharType.Gummy && !triggeredByGummy)
        {
            return;
        }
        else if (cr == CharType.Peppermint && !triggeredByPepper)
        {
            return;
        }

        if (action == Action.Platform)
        {
            platform.GetComponent<PlatformScript>().isEnabled = true;
        }
    }

    public void ExitEffect(CharType cr)
    {
        if (cr == CharType.Gummy && !triggeredByGummy)
        {
            return;
        }
        else if (cr == CharType.Peppermint && !triggeredByPepper)
        {
            return;
        }


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

public enum CharType
{
    Gummy, Peppermint
}
