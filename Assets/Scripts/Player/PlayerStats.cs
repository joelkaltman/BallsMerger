using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

public class PlayerStats : NetworkBehaviour 
{
	[HideInInspector] public NetworkVariable<int> Score = new(0);
	[HideInInspector] public NetworkVariable<int> Caps = new(0);
	[HideInInspector] public NetworkVariable<FixedString64Bytes> Username = new();

    private UserManager user;

    [HideInInspector] public bool IsDead = false; // TODO

	private void Start()
	{
		if (IsOwner)
		{
			user = UserManager.Instance;
			user.ResetKills();

			string username = user.UserData.username;
			if (IsHost)
			{
				Username.Value = username;
			}
			else
			{
				SendUsernameServerRpc(username);
			}
		}

		MultiplayerManager.Instance.RegisterPlayer(gameObject);

		Caps.OnValueChanged += AddCap;
		Score.OnValueChanged += OnKill;
	}
	
	[ServerRpc]
	private void SendUsernameServerRpc(string username)
	{
		if (IsOwner)
			return;
		
		Username.Value = username;
	}


	private void AddCap(int prevCaps, int newCaps)
	{
		if (!IsOwner)
			return;
		
		user.AddCap();
	}
	
	private void OnKill(int prevScore, int newScore)
	{
		if (!IsOwner)
			return;
		
		user.SetKills(Score.Value);
	}
}
