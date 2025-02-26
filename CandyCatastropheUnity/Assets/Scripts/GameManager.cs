

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Tooltip("The prefab to use for representing the player")]
    public GameObject gummyPrefab;
    public GameObject peppermintPrefab;
    public GameObject cubePrefab;
    public float scaleFactor;
    private GameObject gummyPlayer;
    private GameObject peppermintPlayer;
    [HideInInspector] public bool isLocalCoop = false;
    int help = 0;
    public Camera cam;
    public AudioClip GameOver;
    private AudioSource audio;

    #region GameManager Singleton
    static private GameManager gm; //refence GameManager
    static public GameManager GM { get { return gm; } } //public access to read only gm 


    //Check to make sure only one gm of the GameManager is in the scene
    void CheckGameManagerIsInScene()
    {

        //Check if instnace is null
        if (gm == null)
        {
            gm = this; //set gm to this gm of the game object
            Debug.Log(gm);
        }
        else //else if gm is not null a Game Manager must already exsist
        {
            Destroy(this.gameObject); //In this case you need to delete this gm
            Debug.Log("Game Manager exists. Deleting...");
        }
        Debug.Log(gm);
        DontDestroyOnLoad(this.gameObject);
    }//end CheckGameManagerIsInScene()
    #endregion

    #region Photon Callbacks


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }

    #endregion


    #region Public Methods

    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion

    void Awake()
    {

        CheckGameManagerIsInScene();
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        DontDestroyOnLoad(cam);
    }

    private void Update()
    {
        if (isLocalCoop)
        {
            float midpointX = (gummyPlayer.transform.position.x + peppermintPlayer.transform.position.x) / 2;
            float midpointY = (gummyPlayer.transform.position.y + peppermintPlayer.transform.position.y) / 2;
            Vector3 playerMidpoint = new Vector3(midpointX, midpointY, -10);
            float distance = Vector2.Distance(gummyPlayer.transform.position, peppermintPlayer.transform.position);

            cam.transform.position = playerMidpoint;
            cam.transform.localScale = Vector2.one * distance * scaleFactor;

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetScene();
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                cam.transform.Translate(Vector2.up * Time.deltaTime * 4);
            } 
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                cam.transform.Translate(Vector2.down * Time.deltaTime * 4);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                cam.transform.Translate(Vector2.left * Time.deltaTime * 4);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                cam.transform.Translate(Vector2.right * Time.deltaTime * 4);
            }
        }

    }

    private void Start()
    {
        if (PhotonNetwork.InRoom == false)
        {
            Debug.Log("local");
            cam.gameObject.SetActive(true);
            isLocalCoop = true;
            gummyPlayer = Instantiate(gummyPrefab);
            peppermintPlayer = Instantiate(peppermintPrefab);
        }
        else if (BasicPlayerMovement.LocalPlayerInstance == null)
        {
            Debug.Log("multiplayer");
            //Debug.Log("help " + help);
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            //we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            string name = PhotonNetwork.LocalPlayer.NickName;
            foreach (KeyValuePair<int, Player> p in players)
            {
                if (p.Value.NickName == name)
                {
                    if (p.Key == 1)
                    {
                        GameObject player = PhotonNetwork.Instantiate(this.gummyPrefab.name, new Vector2(0f, 5f), Quaternion.identity, 0);
                        player.name = "Player 1";

                    }
                    else if (p.Key == 2)
                    {
                        GameObject player = PhotonNetwork.Instantiate(this.peppermintPrefab.name, new Vector2(0f, 5f), Quaternion.identity, 0);
                        player.name = "Player 2";
                    }
                }
            }
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }

    #region Private Methods


    void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for 2");
        }


        #endregion

    public void ResetScene()
    {
        audio.clip = GameOver;
        audio.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gummyPlayer.transform.position = Vector2.up * 3;
        peppermintPlayer.transform.position = Vector2.up * 3;
    }

    public void NextScene()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        if (activeScene == "Room for 2")
        {
            SceneManager.LoadScene("LevelOne");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}