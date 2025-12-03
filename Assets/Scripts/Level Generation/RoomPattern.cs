using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPattern : MonoBehaviour
{
    [Header("Room")]
    public RoomSize size;
    [Tooltip("Leave empty to take the background sprite renderer as bound")]
    public SpriteRenderer bounds; // zone de jeu utile

    [Header("Visual")]
    public SpriteRenderer background;
    public FolderStyleScriptableObject folderStyle;

    [Header("Obstacles")]
    public List<Obstacle> obstacles; // obstacles déjà dans la room

    [Header("Enemy Spawns")]
    public List<EnemySpawnCluster> enemySpawnPoints;

    public void Start()
    {
        if (!bounds)
        {
            bounds = background;
        }
    }
}
