using System;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    public static PlayerSpawn Instance;
    
    public GameObject playerPrefab;
    public Vector3 spawnPos;

    public event Action<GameObject> OnPlayerSpawn;

    private Dictionary<ulong, GameObject> playerInstances = new ();
    
    void Awake()
    {
        if (Instance && Instance != this)
            Destroy(Instance);

        Instance = this;
        DontDestroyOnLoad(this);
        
        Initialize();
    }

    public void Initialize()
    {
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
    }
    
    public void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (IsHost && sceneEvent.SceneName == "Game")
        {
            if (sceneEvent.ClientsThatCompleted == null)
            {
                // Host
                SpawnPlayer(sceneEvent.ClientId);
            }
            else
            {
                // Remote players
                foreach (var id in sceneEvent.ClientsThatCompleted)
                {
                    if(playerInstances.ContainsKey(id))
                        continue;
                    
                    SpawnPlayer(id);
                }
            }
        }
    }

    private void SpawnPlayer(ulong id)
    {
        var playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        var netObj = playerInstance.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(id, true);
                
        playerInstances.Add(id, playerInstance);
        OnPlayerSpawn?.Invoke(playerInstance);
    }

    public void AddListener(Action<GameObject> onSpawnCallback)
    {
        OnPlayerSpawn += onSpawnCallback;
        
        foreach (var playerInstance in playerInstances.Values)
        {
            onSpawnCallback?.Invoke(playerInstance);
        }
    }

    public void RemoveListener(Action<GameObject> onSpawnCallback)
    {
        OnPlayerSpawn -= onSpawnCallback;
    }
}
