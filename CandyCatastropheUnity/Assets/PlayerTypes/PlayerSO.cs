using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerType", menuName = "Create player type")]
public class PlayerSO : ScriptableObject
{
    [Header("Values")]
    public float baseSpeed;
    public float baseJumpHeight;

    [Header("Art")]
    public Sprite sprite;

    [Header("Animatior")]
    public RuntimeAnimatorController animController;

    [Header("Audio")]
    public AudioClip footstepAudio;
    public AudioClip jumpAudio;
    public AudioClip deathAudio;
}
