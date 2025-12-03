using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public NetworkObject enemyPrefab;
    public int maximumAmount = 1;
    public int minimumAmount = 1;
}
