using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GummyPlayer : MonoBehaviour
{
    //Inspector vars
    public GummyCharState GummyStartState;
    public float maxVertVelocity = 5f;

    public GameObject GummyBearObject;
    public GameObject GumDropObject;
    private GameObject[] GummyObjects;

    //Private vars
    private GummyCharState currentState = GummyCharState.Empty;
    private GameObject currentCharObject;
    private Rigidbody2D rb;
    private bool touchingMold = false;
    private bool isGrounded = true;
    private bool isJumpingUp = false;
    private Collider2D candyCollider;
    private Collider2D moldCollider;
    private GummyBaseState gummyInputState;
    private GameManager gm;
    private bool stuckInCandy = false;
    private bool fellInJello = false;


    // Start is called before the first frame update
    void Awake()
    {
        gm = GameManager.GM;
        rb = GetComponent<Rigidbody2D>();
        GummyObjects = new GameObject[] {
            GummyBearObject,
            GumDropObject
        };
        EnableGravity();
    }

    private void Start()
    {
        ToggleGummyState(GummyStartState);
    }

    // Update is called once per frame
    void Update()
    {

        if (fellInJello)
        {
            return;
        }

        #region User Input

        if (stuckInCandy && !candyCollider)
        {
            EnableGravity();
            stuckInCandy = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryEnterMold();
        }

        MovementStuff();

        #endregion
    }

    public void MovementStuff()
    {
        if (stuckInCandy)
        {
            rb.velocity = Vector3.zero;
            return;
        } 

        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime;
        gummyInputState.Move(transform, horizontal);

        #region Jump Logic

        if (rb.velocity.y == 0 && !isJumpingUp)
        {
            isGrounded = true;
        }

        if (isJumpingUp && rb.velocity.y < 0)
        {
            isJumpingUp = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //Possibly add an abstract class with a Jump() function, allowing seperate types to disable
            gummyInputState.Jump(rb);
            isGrounded = false;
            isJumpingUp = true;
        }

        #endregion

        float newVelo = Mathf.Clamp(rb.velocity.y, -maxVertVelocity, maxVertVelocity);
        rb.velocity = new Vector2(rb.velocity.x, newVelo);

        //Respawns the player up if falls off map
        if (transform.position.y < -10)
        {
            //transform.position = new Vector3(transform.position.x, 10, transform.position.z);
            Die();
        }


        #region Flip Sprite
        SpriteRenderer sr = currentCharObject.GetComponent<SpriteRenderer>();

        //Idk why I did this instead of making a bool "FacingRight" or smthn
        if (horizontal > 0)
        {
            rb.velocity = new Vector2(0.01f, rb.velocity.y);
        }
        else if (horizontal < 0)
        {
            rb.velocity = new Vector2(-0.01f, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (rb.velocity.x < 0)
        {
            sr.flipX = true;
        } 
        else if (rb.velocity.x > 0)
        {
            sr.flipX = false;
        }

        #endregion

    }

    public void TryEnterMold()
    {
        if (touchingMold)
        {
            MoldScript moldScript = moldCollider.gameObject.GetComponent<MoldScript>();
            Debug.Log("Entering Mold...");
            if (currentState == moldScript.MoldOptionOne)
            {
                ToggleGummyState(moldScript.MoldOptionTwo);
                moldScript.OneToTwo();
            }
            else if (currentState == moldScript.MoldOptionTwo)
            {
                ToggleGummyState(moldScript.MoldOptionOne);
                moldScript.TwoToOne();
            }
            else
            {
                Debug.LogWarning("Invalid mold state change! Current state isn't an option on mold");
            }
            
        }
        else
        {
            Debug.Log("Not touching mold");
        }

    }

    private void ToggleGummyState(GummyCharState newState)
    {
        if (newState == currentState || newState == GummyCharState.Empty)
        {
            return;
        }

        foreach (GameObject go in GummyObjects)
        {
            go.SetActive(false);
        }

        currentState = newState;

        switch (newState)
        {
            case GummyCharState.GummyBear:
                GummyBearObject.SetActive(true);
                gummyInputState = GummyBearObject.GetComponent<GummyBearState>();
                currentCharObject = GummyBearObject;
                break;
            case GummyCharState.GumDrop:
                GumDropObject.SetActive(true);
                gummyInputState = GumDropObject.GetComponent<GumDropState>();
                currentCharObject = GumDropObject;
                break;
            default:
                break;
        }
    }

    public void DisableGravity()
    {
        rb.gravityScale = 0f;
    }

    public void EnableGravity()
    {
        rb.gravityScale = 1f;
    }

    public void Die()
    {
        gm.ResetScene();
    }

    


    #region Trigger/Collision Handling

    //Trigger Enter
    public void TouchJello()
    {
        fellInJello = true;
        DisableGravity();
        rb.velocity = Vector3.zero;
    }

    public void TouchCottonCandy(Collider2D collision)
    {
        candyCollider = collision;
        DisableGravity();
        stuckInCandy = true;
    }

    public void TouchMold(Collider2D collision)
    {
        touchingMold = true;
        moldCollider = collision;
    }

    //Trigger Exit
    public void ExitMold()
    {
        touchingMold = false;
    }

    #endregion




    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    GameObject go = collision.gameObject;
    //    if (go.CompareTag("Jello"))
    //    {
    //        TouchJello();
    //    }
    //    else if (go.CompareTag("Cotton Candy"))
    //    {
    //        candyCollider = collision;
    //        DisableGravity();
    //        stuckInCandy = true;
    //    }
    //    else if (go.CompareTag("Mold"))
    //    {
    //        touchingMold = true;
    //        moldCollider = collision;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    GameObject go = collision.gameObject;

    //    if (go.CompareTag("Mold"))
    //    {
    //        touchingMold = false;
    //    }
    //}
}

public enum GummyCharState
{
    Empty, GummyBear, GumDrop
}
