using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    public bool isEnabled = false;
    public float moveSpeed = 2f;
    public float heightToRaise = 2f;
    public float heightToLower = 0f;
    private float currentHeight = 0f;
    public bool goingUp = true;
    private Vector2 startPosition;


    private void Awake()
    {
        startPosition = transform.position;
    }
    private void Update()
    {
        if (!isEnabled)
        {
            currentHeight -= Time.deltaTime * moveSpeed;
        }
        else
        {
            currentHeight += Time.deltaTime * moveSpeed;
        }
        currentHeight = Mathf.Clamp(currentHeight, heightToLower, heightToRaise);
        Debug.Log(currentHeight);
        transform.position = new Vector2(transform.position.x, startPosition.y + currentHeight);

    }

}
