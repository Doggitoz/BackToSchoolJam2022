using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PeppermintPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public float maxHorizontalVelocity = 15f;
    public float speed = 8f;
    public float jumpForce = 1000f;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool isMoving = false;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PeppermintPlayer.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        MovementUpdate();
    }

    private void MovementUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vX = rb.velocity.x;
        float vY = rb.velocity.y;

        //Add or subtract velocity based on horizontal input
        if (horizontal > 0)
        {
            //rb.AddForce(Vector2.right * speed);
            rb.velocity = new Vector2(vX + (maxHorizontalVelocity - vX) * speed / 10 * Time.deltaTime, vY);
        }
        else if (horizontal < 0)
        {
            //rb.AddForce(Vector2.left * speed);
            rb.velocity = new Vector2(vX - (maxHorizontalVelocity + vX) * speed / 10 * Time.deltaTime, vY);
        }
        else
        {
            //Stops the x velocity if below 0.25f
            if (Mathf.Abs(rb.velocity.x) < 0.25f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        float newVelo = Mathf.Clamp(rb.velocity.x, -maxHorizontalVelocity, maxHorizontalVelocity);

        rb.velocity = new Vector2(newVelo, rb.velocity.y);

        if (rb.velocity.y == 0)
        {
            isGrounded = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump(jumpForce);
        }

        if (rb.velocity.magnitude != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    private void Jump(float jForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        isGrounded = false;
        rb.AddForce(Vector2.up * jForce);
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject go = other.gameObject;
        float absVelX = Mathf.Abs(rb.velocity.x);
        float absVelY = Mathf.Abs(rb.velocity.y);
        if (go.CompareTag("Gum"))
        {
            //Will flag game over and respawn later
            speed = 0;
            rb.velocity = Vector2.zero;
        }
        else if (go.CompareTag("Bounce")) {
            if (Input.GetAxis("Vertical") < 0)
            {
                Jump(jumpForce);
            }
            else
            {
                Jump(jumpForce * 2);
            }
        }
        else if (go.CompareTag("Breakable") || go.CompareTag("Cotton Candy"))
        {
            if (absVelX > 3 || absVelY > 5) //DO TRIG SHIT LATER
            {
                Destroy(go);
            }
        }
    }

}
