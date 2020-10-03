using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Navigator : MonoBehaviour {

	public bool GeneratingLine;
	[Range(10f, 50f)]
	public float SpeedToMouse = 37f;

	public LineRenderer LineRenderer;

	readonly List<Vector3> line = new List<Vector3>(100);
    public List<Vector3> Line => line;
	float totalDistance;

	[Range(0.1f, 3f)]
	public float TrailDecayTime = 1f;
	float decayRate;

	Rigidbody2D rb;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		line.Add(rb.position);
	}

	void FixedUpdate() {
		// stop mouse
		{
			if (!Input.GetMouseButton(0)) {
				GeneratingLine = false;
				Scroller.Inst.Scrolling = false;
			}
		}
		// chase mouse
		{
			if (GeneratingLine) {
				var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition).to2();
				var vecToMouse = mousePos - rb.position;
				rb.velocity = vecToMouse.normalized * Mathf.Min(SpeedToMouse, vecToMouse.magnitude / Time.deltaTime);
			} else {
				rb.velocity = Vector2.zero;
			}
		}
		// add to trail
		{
			if (GeneratingLine) {
				var dist = (line[line.Count - 1] - rb.position.to3()).magnitude;
				line.Add(rb.position);
				totalDistance += dist;
			} else {
				line[line.Count - 1] = rb.position;
            }
		}
        // update decay rate based on total distance
		// and freeze it when you stop generating line
        {
			if (GeneratingLine) {
				decayRate = totalDistance / TrailDecayTime;
            }
		}
		// decay from trail
		{
			if (line.Count >= 2) {
				var decayAmount = decayRate * Time.deltaTime;
				while (line.Count >= 2 && decayAmount > 0) {
					var nextDistance = (line[1] - line[0]).magnitude;
					if (nextDistance <= decayAmount) {
						line.RemoveAt(0);
						totalDistance -= nextDistance;
						decayAmount -= nextDistance;
                    } else {
						line[0] = Vector2.MoveTowards(line[0], line[1], decayAmount);
						totalDistance -= decayAmount;
						decayAmount = 0;
                    }
				}
            }
		}
        // set line
        {
			LineRenderer.positionCount = line.Count;
			LineRenderer.SetPositions(line.ToArray());
        }
	}

    private void OnMouseDown() {
		GeneratingLine = true;
		Scroller.Inst.Scrolling = true;
	}


}