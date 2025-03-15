using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawn : NetworkBehaviour
{
    public GameObject enemy;

    public void SpawnEnemy()
    {
        if (!IsHost)
            return;
        
        var spawnedEnemy = Instantiate(enemy, transform.position, Quaternion.identity);
        var netObj = spawnedEnemy.GetComponent<NetworkObject>();
        netObj.Spawn(true);
    }
}
