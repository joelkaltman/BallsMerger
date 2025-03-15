using UnityEngine;

public class Rotation : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		this.transform.Rotate (0, 100 * Time.deltaTime, 0);
	}
}
