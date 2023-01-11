using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class BasicPlayerMovement : MonoBehaviour
{
    //Public vars
    public PlayerSO playerType;
    public InputActionAsset input;
    public bool colorOverride;
    public Color color;
    [HideInInspector] public float speedModifier = 1f; //This might be better to make private and have a getter/setter

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
        ChangePlayer(playerType);
        if (colorOverride)
        {
            SetPlayerColor(color);
        }
        speedModifier = 1f;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * playerType.baseSpeed * speedModifier * Time.deltaTime;

        Debug.Log(horizontal);

        if (Mathf.Abs(horizontal) > 0)
        {
            animator.SetFloat("Speed", 1);
        } 
        else
        {
            animator.SetFloat("Speed", 0);
        }

        transform.Translate(Vector2.right * horizontal);
    }

    #endregion

    void ChangePlayer(PlayerSO newPlayer)
    {
        playerType = newPlayer;
        sr.sprite = playerType.sprite;
        animator.runtimeAnimatorController = playerType.animController;
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
        GetComponent<PolygonCollider2D>().offset = new Vector2(0, 0.06f);
    }

    void SetPlayerColor(Color color)
    {
        sr.color = color;
    }
}
