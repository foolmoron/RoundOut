using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pattern : MonoBehaviour {
		
	void Awake() {
		transform.localScale = new Vector3(
			Random.value < 0.5f ? 1 : -1,
			Random.value < 0.5f ? 1 : -1,
			1
		);
		Destroy(transform.Find("Area").gameObject);
		transform.DetachChildren();
		Destroy(gameObject);
	}
}