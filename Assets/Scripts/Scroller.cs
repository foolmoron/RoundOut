using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scroller : Manager<Scroller> {

	public bool Scrolling;

	public Vector2 Velocity = new Vector2(0, 2.65f);
	Vector2 currentVelocity;
	[Range(0, 0.025f)]
	public float Lerp = 0.01f;

	new Camera camera;

	void Awake() {
		camera = GetComponent<Camera>();
	}

	void Update() {
		var velocity = Scrolling ? Velocity : Vector2.zero;
		currentVelocity = Vector2.Lerp(currentVelocity, velocity, Lerp);
		transform.localPosition += currentVelocity.to3() * Time.deltaTime;
	}
}