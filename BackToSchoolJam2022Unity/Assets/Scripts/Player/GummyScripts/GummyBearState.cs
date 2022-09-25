using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GummyBearState : GummyBaseState
{
    public float jumpForce = 350f;
    public float moveSpeed = 4f;
    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void Jump(Rigidbody2D rb)
    {
        rb.AddForce(Vector2.up * jumpForce);

    }
    public override void Move(Transform t, float horizontal)
    {
        t.Translate(Vector2.right * horizontal * moveSpeed);
    }
}
