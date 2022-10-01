using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoldScript : MonoBehaviour
{
    public GummyCharState MoldOptionOne;
    public GummyCharState MoldOptionTwo;
    public GameObject moldStateOne;
    public GameObject moldStateTwo;
   // [Tooltip("The number HAS to be EITHER 0 or 1. 0 is gumdrop. 1 is Bear.")]
    //public int defaultMold;

    [Tooltip("Set this as the current Active Mold")]
    public Animator currentStateAnimator;

    //I cannot believe I need to do this. -Ava
    //Like seriously, what is this?! I assumed all I'd need to do is have one script for both things...

    public void Start()
    {
        
    }
    //But they're different?! I do not get this.

    public void OneToTwo() //same thing as TwoToOne
    {
        currentStateAnimator.SetTrigger("Activated");
    }

    public void ChangeToOne()
    {
        moldStateOne.SetActive(true);
        currentStateAnimator = moldStateOne.GetComponent<Animator>();
        moldStateTwo.SetActive(false);
    }

    public void ChangeToTwo()
    {
        moldStateTwo.SetActive(true);
        currentStateAnimator = moldStateTwo.GetComponent<Animator>();
        moldStateOne.SetActive(false);
    }

    public void TwoToOne() //Drop to Bear
    {
        currentStateAnimator.SetTrigger("Activated");
    }

   /* public void ChangeType(int currentMoldType)
    {
        //if (currentMoldType >= 1)
        //{
            currentStateAnimator.SetInteger("TypeofMold", currentMoldType); //Changes it to the other Mold
        //}
        //else
        //{
        //    currentStateAnimator.SetInteger("TypeofMold", 1); //Changes it to Bear Mold
        //}
    }*/

}
