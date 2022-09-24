using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeppermintPlayer : MonoBehaviour
{
    public float maxHorizontalVelocity = 15f;
    public float speed = 8f;
    public float jumpForce = 1000f;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vX = rb.velocity.x;
        float vY = rb.velocity.y;

        //Add or subtract velocity based on horizontal input
        if (horizontal > 0)
        {
            //rb.AddForce(Vector2.right * speed);
            rb.velocity = new Vector2(vX + (maxHorizontalVelocity - vX) * speed/10 * Time.deltaTime, vY);
        } 
        else if (horizontal < 0)
        {
            //rb.AddForce(Vector2.left * speed);
            rb.velocity = new Vector2(vX - (maxHorizontalVelocity + vX) * speed/10 * Time.deltaTime, vY);
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
    }

    private void Jump(float jForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        isGrounded = false;
        rb.AddForce(Vector2.up * jForce);
    }

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
