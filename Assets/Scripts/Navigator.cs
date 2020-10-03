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
	[Range(1f, 100f)]
	public float OffDecayTime = 25f;

	Rigidbody2D rb;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		line.Add(rb.position);
	}

	void FixedUpdate() {
		// stop mouse
		{
			if (Input.GetMouseButton(0)) {
				GeneratingLine = true;
				Scroller.Inst.Scrolling = true;
			} else {
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
			var roundingFactor = 5f;
			var roundedPosition = new Vector2(
				Mathf.Round(roundingFactor * rb.position.x) / roundingFactor,
				Mathf.Round(roundingFactor * rb.position.y) / roundingFactor
			);
			if (GeneratingLine) {
				var dist = (line[line.Count - 1] - roundedPosition.to3()).magnitude;
				if (dist > 0.001f) {
					line.Add(roundedPosition);
					totalDistance += dist;
				}
			} else {
				line[line.Count - 1] = roundedPosition;
			}
		}
		// decay from trail
		{
			if (line.Count >= 2) {
				var decayRate = Mathf.Max(totalDistance / TrailDecayTime, GeneratingLine ? 0f : OffDecayTime);
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

}