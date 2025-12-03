using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Start,
    Boss,
    Upgrade,
    Normal,
    Secret
}

public class Node
{
    public int index;
    public Node parent;
    public RoomType roomType;
    public RoomSize roomSize;
    public List<Node> children;

    public Node(RoomType roomType, RoomSize roomSize)
    {
        this.roomType = roomType;
        this.children = new List<Node>();
        this.roomSize = roomSize;
    }

    public void AddNode(Node node)
    {
        node.parent = this;
        children.Add(node);
    }

    public void RemoveNode(Node node)
    {
        node.parent = null;
        children.Remove(node);
    }
    
    public void UpdateNode(RoomType roomType)
    {
        this.roomType = roomType;
    }

    public void ClearNode()
    {
        foreach (Node node in children)
        {
            RemoveNode(node);
        }
        children.Clear();
    }
}
