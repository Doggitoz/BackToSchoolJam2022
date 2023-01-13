using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{

    #region Vars

    //Public vars
    public PlayerSO playerType;
    public InputActionAsset input;
    public bool colorOverride;
    public Color color;
    [Range(0, 1)] public float speedModifier = 1f; //This might be better to make private and have a getter/setter

    //Private vars
    bool _flipped = false;
    bool _respawnProtection;
    float _timeHoldingKey;
    bool _leftHeld = false;
    bool _rightHeld = false;
    

    #endregion

    //Components
    SpriteRenderer sr;
    Animator animator;

    #region Monobehaviour Methods

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        //Set initial player type
        ChangePlayer(playerType);
        if (colorOverride)
        {
            //Change color on initial override
            SetPlayerColor(color);
        }
        //Reset speed modifier to 100%
        speedModifier = 1f;

        //Testing a statemachine implementation
        //StateMachine test = new StateMachine();
        //test.SetCallbacks("test", null, null, null, null);
        //foreach (KeyValuePair<string, State> kvp in test.states)
        //{
        //    Debug.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
        //    State currState = kvp.Value;
        //    currState.getUpdateMethod()();
        //}
    }

    void Update()
    {
        UpdatePhysics();
        UpdateVisuals();
    }

    #endregion

    void ChangePlayer(PlayerSO newPlayer)
    {
        playerType = newPlayer;
        sr.sprite = playerType.sprite;
        animator.runtimeAnimatorController = playerType.animController;
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
        Vector2 offset = new Vector2(0f, 0.062f);
        GetComponent<PolygonCollider2D>().offset = offset;
    }

    void SetPlayerColor(Color color)
    {
        sr.color = color;
    }

    void UpdatePhysics()
    {

        #region Horizontal movement
        if (_leftHeld)
        {
            _leftHeld = Input.GetKey(KeyCode.A);
        }
        else if (_rightHeld)
        {
            _rightHeld = Input.GetKey(KeyCode.D);
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                _flipped = true;
                _leftHeld = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _flipped = false;
                _rightHeld = true;
            }
        }

        if (_leftHeld || _rightHeld)
        {
            _timeHoldingKey = _timeHoldingKey > playerType.speedupTime ? playerType.speedupTime : _timeHoldingKey + Time.deltaTime;
            animator.SetFloat("Speed", _timeHoldingKey / playerType.speedupTime);
        }
        else
        {   
            _timeHoldingKey = 0;
            animator.SetFloat("Speed", _timeHoldingKey / playerType.speedupTime);
        }

        float xChange;
        if (_leftHeld) xChange = -(_timeHoldingKey / playerType.speedupTime);
        else if (_rightHeld) xChange = (_timeHoldingKey / playerType.speedupTime);
        else xChange = 0;
        transform.Translate(new Vector2(xChange * Time.deltaTime * playerType.maxSpeed * speedModifier, 0));
        #endregion



    }

    void UpdateVisuals()
    {
        sr.flipX = _flipped;
    }
}
