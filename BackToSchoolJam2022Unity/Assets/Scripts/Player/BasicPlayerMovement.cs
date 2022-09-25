using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BasicPlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    //Inspector vars
    public float moveSpeed = 1.0f;
    public bool gravityEnabled = true;
    public float maxVelocity;

    //Public vars
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    //Private vars
    private float velocity;
    private Rigidbody2D rb;
    private bool isMoving = false;
#if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }
#endif

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

    void Start()
    {
#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
    }



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            BasicPlayerMovement.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

    }

    void Update()
    {
        if (photonView.IsMine)
        {
            isMoving = true;
            float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

            transform.Translate(Vector2.right * horizontal);
        }
        

    }


void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}



    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
    }
    


#if UNITY_5_4_OR_NEWER
    public override void OnDisable()
    {
        // Always call the base to remove callbacks
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
#endif
}
