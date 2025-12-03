using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] 
    private NetworkObject playerPrefab;
    [SerializeField]
    private string sceneName = "TestScene";

    private List<PlayerController> playerControllers = new List<PlayerController>();
    public List<PlayerController> PlayerControllers => playerControllers;

    private MapGenerator mapGenerator;

    [SerializeField]
    private NetworkObject testEnemyPrefab;
    private void Awake()
    {
        Instance = this;
        mapGenerator = GetComponent<MapGenerator>();
    }

    public void StartGame()
    {
        // Load the scene (server only)
        if (IsServerStarted)
        {
            SceneLoadData sld = new SceneLoadData(sceneName);

            sld.ReplaceScenes = ReplaceOption.All;
            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
            InstanceFinder.SceneManager.OnLoadEnd += OnGameSceneLoaded;
        }
    }

    private void OnGameSceneLoaded(SceneLoadEndEventArgs callback)
    {
        if (IsServerStarted && callback.LoadedScenes.FirstOrDefault().name == sceneName)
        {
            mapGenerator.SetupDungeon();
            RoomManager.Instance.GenerateRooms(mapGenerator.dungeon);
            RoomManager.Instance.ActivateRoom(mapGenerator.StartRoom);
            SpawnAllPlayers();
            //SpawnAllEnemies();
        }
    }


    [Server]
    private void SpawnAllPlayers()
    {
        foreach (NetworkConnection conn in InstanceFinder.ServerManager.Clients.Values)
        {
            if (!conn.IsAuthenticated)
                continue;

            NetworkObject playerObj = Instantiate(playerPrefab);
            playerControllers.Add(playerObj.GetComponent<PlayerController>());
            ServerManager.Spawn(playerObj, conn);
        }
    }

    [Server]
    private void SpawnAllEnemies()
    {
        NetworkObject enemyObj = Instantiate(testEnemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        Enemy e = enemyObj.GetComponent<Enemy>();
        e.InitEnemy(playerControllers);

        ServerManager.Spawn(enemyObj);
    }
}
