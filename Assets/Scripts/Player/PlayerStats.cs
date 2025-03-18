using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour 
{
	[HideInInspector] public NetworkVariable<FixedString64Bytes> Username = new();
	[HideInInspector] public NetworkVariable<int> Score = new(0, writePerm:NetworkVariableWritePermission.Owner);
	[HideInInspector] public NetworkVariable<bool> Lost = new(false, writePerm:NetworkVariableWritePermission.Owner);

    private UserManager user;

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

		Score.OnValueChanged += OnKill;
	}
	
	[ServerRpc]
	private void SendUsernameServerRpc(string username)
	{
		if (IsOwner)
			return;
		
		Username.Value = username;
	}
	
	private void OnKill(int prevScore, int newScore)
	{
		if (!IsOwner)
			return;
		
		user.SetKills(Score.Value);
	}
}
