using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForGummy : MonoBehaviour
{

    public NextStage Parent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GummyPlayer"))
        {
            Debug.Log("Enter gummy");
            Parent.GummyInPlace = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GummyPlayer"))
        {
            Parent.GummyInPlace = false;
        }
    }
}
