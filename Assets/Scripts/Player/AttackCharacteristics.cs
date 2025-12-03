using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack Characteristics", menuName = "Game /Character Attack")]
public class AttackCharacteristics : ScriptableObject
{
    [Header("Attack")]
    public float clickSpeed = 1f;
    public float clickRange = 1f; //Hitbox of the base click
    public float clickDamage = 1f; // click damage
    public float clickKnockback = 0f;
}
