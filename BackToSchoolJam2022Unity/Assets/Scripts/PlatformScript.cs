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
    public bool startsUp = false;
    public bool automaticRaise = false;


    private void Awake()
    {
        startPosition = transform.position;
    }
    private void Update()
    {
        if (automaticRaise)
        {
            if (goingUp)
            {
                currentHeight += Time.deltaTime * moveSpeed;
                if (currentHeight >= heightToRaise)
                {
                    goingUp = false;
                }
            }
            else
            {
                currentHeight -= Time.deltaTime * moveSpeed;
                if (currentHeight <= heightToLower)
                {
                    goingUp = true;
                }
            }
        }
        else
        {
            if (!isEnabled)
            {
                if (startsUp)
                {
                    currentHeight += Time.deltaTime * moveSpeed;
                } 
                else
                {
                    currentHeight -= Time.deltaTime * moveSpeed;
                }
            }
            else
            {
                if (startsUp)
                {
                    currentHeight -= Time.deltaTime * moveSpeed;
                }
                else
                {
                    currentHeight += Time.deltaTime * moveSpeed;
                }
            }
        }

        currentHeight = Mathf.Clamp(currentHeight, heightToLower, heightToRaise);
        transform.position = new Vector2(transform.position.x, startPosition.y + currentHeight);
    }

}
