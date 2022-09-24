using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerMovement : MonoBehaviour
{
    //Inspector vars
    public float moveSpeed = 1.0f;
    public bool gravityEnabled = true;
    public float maxVelocity;

    //Private vars
    private float velocity;
    private Rigidbody2D rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        transform.Translate(Vector2.right * horizontal);

    }
}
