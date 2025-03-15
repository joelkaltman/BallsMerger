using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DestroyTimer : ServerOnlyMonobehavior {

	public int destroySeconds;

	void Start () {
		StartCoroutine(DestroyObject());
	}

	IEnumerator DestroyObject()
	{
		yield return new WaitForSeconds(destroySeconds);
		var netObj = GetComponent<NetworkObject>();
		if (netObj)
		{
			netObj.Despawn();
		}
		else
		{
			Destroy (this.gameObject);
		}
	}
}
