using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Movement Characteristics", menuName = "Game /Character Speed")]
public class MovementCharacteristics : ScriptableObject
{
    [Header("Movement")]
    public float speedMultiplier = 1.5f;     // vitesse brute
    public float drag = 6f;                 // perte de vitesse quand la souris stop
    public float smoothTime = 0.05f;        // lissage
    public float maxSpeed = 20f;            // vitesse max
    public float hurtBoxSize = 2f;

    [Header("FocusMode")]
    public float fSpeedMultiplier = 1.5f;     // vitesse brute en mode focus
    public float fDrag = 6f;                 // perte de vitesse quand la souris stop en mode focus
    public float fSmoothTime = 0.05f;        // lissage en mode focus
    public float fMaxSpeed = 20f;            // vitesse max en mode focus
    public float fHurtBoxSize = 2f;
}
