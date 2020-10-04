using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pattern : MonoBehaviour {
		
	void Awake() {
		// unparent contents and destroy self
		foreach (Transform child in transform) {
			if (child.name != "Area") {
				child.parent = null;
            }
        }
		Destroy(gameObject);
	}
}