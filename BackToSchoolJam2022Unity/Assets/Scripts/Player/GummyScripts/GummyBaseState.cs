using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GummyBaseState : MonoBehaviour
{
    public abstract void Jump(Rigidbody2D rb);

    public abstract void Move(Transform t, float horizontal);

    public abstract void EnterState();
}
