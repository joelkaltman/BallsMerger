using System.Threading.Tasks;
using UnityEngine;

public class ServerOnlyMonobehavior : MonoBehaviour
{
    void Awake()
    {
        CheckHostReady();
    }

    private async void CheckHostReady()
    {
        while (!MultiplayerManager.Instance || !MultiplayerManager.Instance.IsGameReady)
        {
            await Task.Yield();
        }
        
        if(!MultiplayerManager.Instance.IsHost)
            Destroy(this);
    }
}
