using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GummyPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    //Inspector vars
    public GummyCharState GummyStartState;
    public float maxVertVelocity = 5f;

    public GameObject GummyBearObject;
    public GameObject GumDropObject;
    private GameObject[] GummyObjects;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

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
    private bool isMoving = false;



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

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            GummyPlayer.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        ToggleGummyState(GummyStartState);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {

        if (fellInJello)
        {
            return;
        }

        #region User Input

        if (photonView.IsMine)
        {
            isMoving = true;
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

        }
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

    #region Photon Stuff

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(isMoving);
        }
        else
        {
            // Network player, receive data
            this.isMoving = (bool)stream.ReceiveNext();
        }
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }

    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
    }

    public override void OnDisable()
    {
        // Always call the base to remove callbacks
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion


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
}

public enum GummyCharState
{
    Empty, GummyBear, GumDrop
}
