using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnCluster : MonoBehaviour
{
    public float radius;
    public List<EnemySpawn> spawns = new List<EnemySpawn>();
    public bool absolute; //spawn only one at the center
    public float spawnSpacing;
}
