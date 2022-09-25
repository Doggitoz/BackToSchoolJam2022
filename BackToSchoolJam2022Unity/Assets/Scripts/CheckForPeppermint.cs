using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForPeppermint : MonoBehaviour
{
    public NextStage Parent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("touch"); 
        if (collision.gameObject.CompareTag("PeppermintPlayer"))
        {
            Debug.Log("Enter peppermint");
            Parent.PeppermintInPlace = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PeppermintPlayer"))
        {
            Parent.PeppermintInPlace = false;
        }
    }
}
