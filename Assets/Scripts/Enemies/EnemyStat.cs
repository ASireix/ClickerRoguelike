using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enemy Stat", menuName = "Game/Ennemy Stat")]
public class EnemyStat : ScriptableObject
{
    public float maxHealth;
    public float speed = 3f;
    public float smoothTime = 0.15f;
    public float detectionRadius = 6f;
}
