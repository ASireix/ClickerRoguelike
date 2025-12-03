using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class RoomManager : NetworkBehaviour
{
    #region Singleton
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogWarning("Multiple RoomManager detected.");
    }
    #endregion

    [Header("Icon Offset")]
    [SerializeField] Vector2 applicationStartOffset = Vector2.zero;
    [SerializeField] float spacing;

    [Header("Room Prefabs")]
    public List<RoomPattern> smallPatterns;
    public List<RoomPattern> mediumPatterns;
    public List<RoomPattern> largePatterns;

    [Header("Folder prefab")]
    public NetworkObject folderPrefab;

    [Header("Where rooms are instantiated")]
    public Transform roomRoot;

    private Dictionary<int, Room> roomMap = new Dictionary<int, Room>();
    private Room currentActiveRoom;


    // ------------------------------------------------------------
    // Called by GameManager AFTER dungeon tree generation (server)
    // ------------------------------------------------------------
    [Server]
    public void GenerateRooms(List<Node> dungeon)
    {
        roomMap.Clear();

        foreach (Node node in dungeon)
        {
            // choose a pattern based on size or type
            RoomPattern patternPrefab = ChoosePattern(node);

            // instantiate local room visual
            RoomPattern pattern = Instantiate(patternPrefab, roomRoot);

            // add Room behaviour to store logic
            Room room = pattern.gameObject.AddComponent<Room>();
            room.Initialize(node, pattern);

            roomMap.Add(node.index, room);

            // deactivate everything at generation
            room.gameObject.SetActive(false);
        }
    }


    // ------------------------------------------------------------
    // When the player enters a node/room
    // ------------------------------------------------------------
    [Server]
    public void ActivateRoom(Node node)
    {
        Room room = roomMap[node.index];
        if (room == null)
        {
            Debug.LogError("Could not activate room: not found.");
            return;
        }

        // deactivate previous
        if (currentActiveRoom != null)
            currentActiveRoom.gameObject.SetActive(false);

        // activate target
        room.gameObject.SetActive(true);

        currentActiveRoom = room;
        if (!currentActiveRoom.isCleared)
        {
            SpawnEnemiesServer(room);
        }
    }

    [Server]
    public void ActivateRoom(int roomIndex)
    {
        Room room = roomMap[roomIndex];
        if (room == null)
        {
            Debug.LogError("Could not activate room: not found.");
            return;
        }

        // deactivate previous
        if (currentActiveRoom != null)
            currentActiveRoom.gameObject.SetActive(false);

        // activate target
        room.gameObject.SetActive(true);

        currentActiveRoom = room;
        if (!currentActiveRoom.isCleared)
        {
            SpawnEnemiesServer(room);
        }
    }


    // ------------------------------------------------------------
    // SPAWN ENNEMIS (SERVER ONLY)
    // ------------------------------------------------------------
    [Server]
    private void SpawnEnemiesServer(Room room)
    {
        foreach (var cluster in room.GetEnemyClusters())
        {
            if (cluster.absolute)
            {
                // Spawn enemies at center
                foreach(var spawn in cluster.spawns)
                {
                    int amountToSpawn = Random.Range(spawn.minimumAmount,spawn.maximumAmount);
                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        SpawnOneEnemy(spawn.enemyPrefab, cluster.transform.position);
                        room.amountOfEnemies++;
                    }
                }
            }
            else
            {
                // Spawn random spread in radius
                foreach (var spawn in cluster.spawns)
                {
                    int amountToSpawn = Random.Range(spawn.minimumAmount, spawn.maximumAmount);
                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        Vector2 pos =
                        (Vector2)cluster.transform.position +
                        Random.insideUnitCircle * cluster.radius;
                        SpawnOneEnemy(spawn.enemyPrefab, pos);
                        room.amountOfEnemies++;
                    }
                }
            }
        }
    }

    [Server]
    void GenerateApplications(Room room)
    {
        Bounds roomBound = room.GetPattern().bounds.bounds;
        float xOffset = roomBound.min.x + applicationStartOffset.x;
        float yOffset = roomBound.max.y + applicationStartOffset.y;

        Vector2 pos = new Vector2 (xOffset, yOffset);

        Node roomNode = room.GetNode();

        if (roomNode.parent != null)
        {
            SpawnOneFolder(pos, roomMap[roomNode.parent.index]).folderName.text = "...";
            pos.x += spacing;
        }

        if (roomNode.children.Count > 0)
        {
            for (int i = 0; i < roomNode.children.Count; i++)
            {
                SpawnOneFolder(pos, roomMap[roomNode.children[i].index]).folderName.text = "Destination : "+ roomNode.children[i].index;
                pos.x += spacing;
            }
        }
    }

    [Server]
    private Folder SpawnOneFolder(Vector2 pos, Room roomDest)
    {
        NetworkObject folder = Instantiate(folderPrefab, pos, Quaternion.identity);
        Folder f = folder.GetComponent<Folder>();

        Node roomNode = roomDest.GetNode();

        FolderStyleScriptableObject fStyle = roomDest.GetPattern().folderStyle;

        if (fStyle != null)
        {
            switch (roomNode.roomType)
            {
                case RoomType.Start:
                    f.spriteRenderer.sprite = fStyle.defaultSprite;
                    f.spriteRenderer.color = fStyle.defaultSpriteColor;
                    break;
                case RoomType.Boss:
                    f.spriteRenderer.sprite = fStyle.bossSprite;
                    f.spriteRenderer.color = fStyle.bossSpriteColor;
                    break;
                case RoomType.Upgrade:
                    f.spriteRenderer.sprite = fStyle.upgradeSprite;
                    f.spriteRenderer.color = fStyle.upgradeSpriteColor;
                    break;
                case RoomType.Normal:
                    f.spriteRenderer.sprite = fStyle.defaultSprite;
                    f.spriteRenderer.color = fStyle.defaultSpriteColor;
                    break;
                case RoomType.Secret:
                    f.spriteRenderer.sprite = fStyle.secretSprite;
                    f.spriteRenderer.color = fStyle.secretSpriteColor;
                    break;
                default:
                    f.spriteRenderer.sprite = fStyle.defaultSprite;
                    f.spriteRenderer.color = fStyle.defaultSpriteColor;
                    break;
            }

        }
        f.SetDestination(roomNode.index);

        folder.transform.SetParent(currentActiveRoom.transform);

        ServerManager.Spawn(folder);

        return f;
    }


    [Server]
    private void SpawnOneEnemy(NetworkObject enemyPrefab, Vector2 position)
    {
        NetworkObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null)
        {
            e.InitEnemy(GameManager.Instance.PlayerControllers);
            e.onEnemyDie.AddListener(OnEnemyDeath);
        }

        ServerManager.Spawn(enemy);
    }


    void OnEnemyDeath(Enemy enemy)
    {
        currentActiveRoom.amountOfEnemies--;

        if (currentActiveRoom.amountOfEnemies <= currentActiveRoom.amountOfEnemiesToUnlockRoom)
        {
            currentActiveRoom.isCleared = true;
            GenerateApplications(currentActiveRoom);
        }
    }


    // ------------------------------------------------------------
    // Choose which room prefab to instantiate
    // ------------------------------------------------------------
    private RoomPattern ChoosePattern(Node node)
    {
        switch (node.roomSize)
        {
            case RoomSize.Small:
                return smallPatterns[Random.Range(0, smallPatterns.Count)];
            case RoomSize.Medium:
                return mediumPatterns[Random.Range(0, mediumPatterns.Count)];
            case RoomSize.Large:
                return largePatterns[Random.Range(0, largePatterns.Count)];
            default:
                return null;
        }
        
    }
}
