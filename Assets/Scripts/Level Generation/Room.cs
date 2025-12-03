using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Node node;                     // The node used to build this room
    private RoomPattern pattern;           // The selected pattern prefab

    private List<EnemySpawnCluster> clusters;
    private List<Obstacle> obstacles;

    public int amountOfEnemiesToUnlockRoom = 0;
    public int amountOfEnemies = 0;

    public bool isCleared;

    /// <summary>
    /// Called by RoomManager when generating the dungeon.
    /// </summary>
    public void Initialize(Node node, RoomPattern pattern)
    {
        this.node = node;
        this.pattern = pattern;

        // cached refs
        clusters = pattern.enemySpawnPoints;
        obstacles = pattern.obstacles;
    }

    // -------------------------------------------
    // Public getters
    // -------------------------------------------

    public Node GetNode() => node;

    public RoomPattern GetPattern() => pattern;

    public List<EnemySpawnCluster> GetEnemyClusters() => clusters;

    public List<Obstacle> GetObstacles() => obstacles;
}
