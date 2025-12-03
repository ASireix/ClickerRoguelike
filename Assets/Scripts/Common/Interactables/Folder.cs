using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Folder : NetworkBehaviour, IInteractable
{
    private int destinationIndex = -1;
    public TextMeshProUGUI folderName;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void SetDestination(int destination)
    {
        destinationIndex = destination;
    }

    public void Interact()
    {
        if (destinationIndex >= 0)
        {
            RequestRoomChangeServer(destinationIndex);
        }
        Debug.Log("Interacting");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRoomChangeServer(int destination)
    {
        RoomManager.Instance.ActivateRoom(destination);
    }
}
