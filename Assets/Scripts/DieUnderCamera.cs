using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DieUnderCamera : MonoBehaviour
{
	public float UnderY = -8f;

	void FixedUpdate() {
		if (transform.position.y < Camera.main.transform.position.y + UnderY) {
			Destroy(gameObject);
        }
    }
}