using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GummyPlayer : MonoBehaviour
{
    //Inspector vars
    public float defaultMoveSpeed = 4f;
    public float maxVertVelocity = 5f;
    public float jumpForce = 350f;
    public GameObject GummyBear;
    public GameObject GumDrop;

    //Public vars
    [HideInInspector] public float moveSpeed;
    public bool stuckInCandy = false;

    //Private vars
    private Rigidbody2D rb;
    private bool isGravityEnabled = true;
    private Collider2D candyCollider;


    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = defaultMoveSpeed;
        rb = GetComponent<Rigidbody2D>();
        EnableGravity();
    }

    // Update is called once per frame
    void Update()
    {

        if (stuckInCandy && !candyCollider)
        {
            EnableGravity();
            stuckInCandy = false;
        }

        MovementStuff();
    }

    public void MovementStuff()
    {
        if (stuckInCandy)
        {
            rb.velocity = Vector3.zero;
            return;
        } 

        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        transform.Translate(Vector2.right * horizontal);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }

        float newVelo = Mathf.Clamp(rb.velocity.y, -maxVertVelocity, maxVertVelocity);
        rb.velocity = new Vector2(rb.velocity.x, newVelo);

        if (transform.position.y < -10)
        {
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        }
    }

    public void TryEnterMold()
    {

    }

    public void DisableGravity()
    {
        rb.gravityScale = 0f;
    }

    public void EnableGravity()
    {
        rb.gravityScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        float absVelX = Mathf.Abs(rb.velocity.x);
        float absVelY = Mathf.Abs(rb.velocity.y);
        if (go.CompareTag("Cotton Candy"))
        {
            candyCollider = collision;
            DisableGravity();
            stuckInCandy = true;
        }
    }
}
