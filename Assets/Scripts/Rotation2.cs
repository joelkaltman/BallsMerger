using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation2 : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		this.transform.Rotate (100 * Time.deltaTime, 0, 0);
	}
}
