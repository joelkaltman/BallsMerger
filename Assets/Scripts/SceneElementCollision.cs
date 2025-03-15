using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneElementCollision : MonoBehaviour {

	public float speed;
	public float initialLimit;
	public float bounce;
	public GameObject particles;
	public Color color;

	private float limit;
	private float rot;
	private float currentDir;
	private float first;
	private bool shaking;

	private Vector3 originalPos;
	private Quaternion originalRotXY;

	public void Shake()
	{
		if (shaking) {
			return;
		}

		originalPos = transform.position;
		this.originalRotXY = this.transform.rotation;
		this.rot = 0;
		this.limit = this.initialLimit;
		this.currentDir = 1;
		this.first = 2;

		this.shaking = true;
	}

	void Update () {
		if (shaking) {
			Vector3 aroundPoint = new Vector3 (this.transform.position.x, 5, this.transform.position.z);

			rot += speed;
			if (rot > limit/first) {
				limit -= bounce;
				currentDir = -currentDir;
				rot = 0;
				first = 1;
			}
			this.transform.RotateAround (aroundPoint, Vector3.forward, currentDir * rot);

			if (limit < 0.5f) {
				shaking = false;
				transform.SetPositionAndRotation(originalPos, originalRotXY);
			}
		}
	}
		
	public void OnBulletCollision(Vector3 position)
	{
		Shake ();
		GameObject instancePart = Instantiate (this.particles, position, Quaternion.identity);

		Vector3 dif = position - this.transform.position;
		dif.Normalize ();
		dif.y = 2;

		instancePart.transform.rotation = Quaternion.LookRotation (dif);

		ParticleSystem.MainModule particles = instancePart.GetComponent<ParticleSystem> ().main;
		particles.startColor = new Color(this.color.r, this.color.g, this.color.b);
		Destroy (instancePart, 1);
	}
}
