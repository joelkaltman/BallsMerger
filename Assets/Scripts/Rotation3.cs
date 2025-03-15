using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation3 : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		this.transform.Rotate (0, 0, 500 * Time.deltaTime);
	}
}
