using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<Node> dungeon;

    [SerializeField]
    private int maxSubroom; // Max children per room
    [SerializeField]
    private int maxRoom;
    [SerializeField]
    private int minRoom;

    [SerializeField]
    private float additionnalSecretRoomProbability = 5f;
    [SerializeField]
    private float additionnalUpgradeRoomProbability = 30f;

    private int bossRoomIndex = -1;
    private int secretRoomIndex = -1;
    private int upgradeRoomIndex = -1;

    public void SetupDungeon()
    {
        if (dungeon == null)
        {
            dungeon = new List<Node>();
        }
        else
        {
            dungeon.Clear();
        }

        //---------------------------------------------
        // Generate the tree
        //---------------------------------------------

        Node root = new Node(RoomType.Start, GetRandomRoomSize());
        root.index = 0;
        dungeon.Add(root);

        int roomCount = Random.Range(minRoom, maxRoom + 1);

        // Generate Normal rooms
        for (int i = 1; i < roomCount; i++)
        {
            Node newNode = new Node(RoomType.Normal, GetRandomRoomSize());
            newNode.index = i;

            Node parent;

            if (Random.value < 0.5f)
            {
                // random parent
                parent = dungeon[Random.Range(0, dungeon.Count)];
            }
            else
            {
                // random parent but more often near the root
                int limit = Mathf.Max(1, dungeon.Count / 2);
                parent = dungeon[Random.Range(0, limit)];
            }

            // check subroom limit
            int safety = 0;
            while (parent.children.Count >= maxSubroom && safety < 20)
            {
                parent = dungeon[Random.Range(0, dungeon.Count)];
                safety++;
            }

            if (parent.children.Count >= maxSubroom)
                break;

            parent.AddNode(newNode);
            dungeon.Add(newNode);
        }
        //---------------------------------------------

        SetupSpecialRooms();

        PrintDungeon(dungeon.FirstOrDefault());
    }

    public Node StartRoom => dungeon[0];

    void SetupSpecialRooms()
    {
        //Mandatory rooms
        bossRoomIndex = GetRandomRoomIndex(true);
        dungeon[bossRoomIndex].roomType = RoomType.Boss;
        Debug.Log("Boss is in room : " + bossRoomIndex);

        upgradeRoomIndex = GetRandomRoomIndex();
        dungeon[upgradeRoomIndex].roomType = RoomType.Upgrade;
        Debug.Log("Upgrade is in room : " + upgradeRoomIndex);

        secretRoomIndex = GetRandomRoomIndex();
        dungeon[secretRoomIndex].roomType = RoomType.Secret;
        Debug.Log("Secret is in room : " + secretRoomIndex);

        if (secretRoomIndex == -1 || upgradeRoomIndex == -1 || bossRoomIndex == -1)
        {
            Debug.LogWarning("Could not populate dungeon with special rooms");
            return;
        }

        //Optional rooms
        for (int i = 1; i < dungeon.Count; i++)
        {
            if (dungeon[i].roomType == RoomType.Normal)
            {
                float proba = Random.Range(0f, 100f);

                if (proba < additionnalSecretRoomProbability)
                {
                    dungeon[i].roomType = RoomType.Secret;
                }
                else if (proba < additionnalUpgradeRoomProbability)
                {
                    dungeon[i].roomType = RoomType.Upgrade;
                }
            }
        }
    }

    int GetRandomRoomIndex(bool isEnding = false)
    {
        if (isEnding)
        {
            List<Node> endNodes = dungeon.Where(x => x.children.Count == 0 && x.index != 0).ToList();
            return Random.Range(0, endNodes.Count);
        }

        int rdnIndex = Random.Range(1, dungeon.Count);
        for (int i = rdnIndex; i < dungeon.Count; i++)
        {
            if (dungeon[i].roomType == RoomType.Normal)
            {
                return i;
            }
        }

        for (int i = rdnIndex-1; i > 0; i--)
        {
            if (dungeon[i].roomType == RoomType.Normal)
            {
                return i;
            }
        }

        return -1;
    }

    // ASCII-only tree print
    void PrintDungeon(Node node, string indent = "", bool isLast = true)
    {
        string prefix = indent + (isLast ? "|- " : "|- ");
        Debug.Log(prefix + node.roomType + " (" + node.index + ")");

        indent += isLast ? "   " : "|  ";

        for (int i = 0; i < node.children.Count; i++)
        {
            PrintDungeon(node.children[i], indent, i == node.children.Count - 1);
        }
    }

    RoomSize GetRandomRoomSize()
    {
        return RoomSize.Small;
    }
}
