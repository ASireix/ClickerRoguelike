using FishNet;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class NetworkCallbacks : MonoBehaviour
{
    [SerializeField] private NetworkObject gameManagerPrefab;

    private void OnEnable()
    {
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerState;
    }

    private void OnDisable()
    {
        InstanceFinder.ServerManager.OnServerConnectionState -= OnServerState;
    }

    private void OnServerState(ServerConnectionStateArgs callback)
    {
        if (callback.ConnectionState == LocalConnectionState.Started)
        {
            // Only spawn if none exists
            if (GameManager.Instance == null)
            {
                NetworkObject gm = Instantiate(gameManagerPrefab);
                InstanceFinder.ServerManager.Spawn(gm);
            }
        }
    }
}