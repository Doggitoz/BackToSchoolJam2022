using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GummyTriggerDetection : MonoBehaviour
{
    public GameObject GummyParent;
    public GummyPlayer GummyPlayerScript;


    private void Awake()
    {
        GummyPlayerScript = GummyParent.GetComponent<GummyPlayer>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag("Jello"))
        {
            GummyPlayerScript.TouchJello();
        }
        else if (go.CompareTag("Cotton Candy"))
        {
            GummyPlayerScript.TouchCottonCandy(collision);
        }
        else if (go.CompareTag("Mold"))
        {
            GummyPlayerScript.TouchMold(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;

        if (go.CompareTag("Mold"))
        {
            GummyPlayerScript.ExitMold();
        }
    }


}
