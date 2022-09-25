using UnityEngine;
using Photon.Pun;

public class GummyTriggerDetection : MonoBehaviourPunCallbacks
{
    public GameObject GummyParent;
    public GummyPlayer GummyPlayerScript;
    GameManager gm;


    private void Awake()
    {
        GummyPlayerScript = GummyParent.GetComponent<GummyPlayer>();
        gm = GameManager.GM;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine || gm.isLocalCoop)
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
            else if (go.CompareTag("Button"))
            {
                go.GetComponent<ButtonPress>().TriggerEffect(CharType.Gummy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (photonView.IsMine || gm.isLocalCoop)
        {
            GameObject go = collision.gameObject;

            if (go.CompareTag("Mold"))
            {
                GummyPlayerScript.ExitMold();
            }
            else if (go.CompareTag("Button"))
            {
                go.GetComponent<ButtonPress>().ExitEffect(CharType.Gummy);
            }
        }
    }

}
