using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayConnection : MonoBehaviour
{
    public Text textJoinCodeOut;
    public Text textJoinCodeIn;
    
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(1);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            textJoinCodeOut.text = joinCode;

            var relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log($"{e.Message}");
        }
    }

    public async void JoinRelay()
    {
        try
        {
            string joinCode = textJoinCodeIn.text;
            
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            var relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log($"{e.Message}");
        }
    }
}
