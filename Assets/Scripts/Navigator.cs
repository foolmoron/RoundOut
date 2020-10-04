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

    public List<Vector3> Line { get; } = new List<Vector3>(100);
    float totalDistance;

	[Range(0.1f, 3f)]
	public float TrailDecayTime = 1f;
	[Range(1f, 100f)]
	public float OffDecayTime = 25f;

	SpriteRenderer anim;
	Rigidbody2D rb;
	float stunTime;

	void Awake() {
		anim = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		Line.Add(rb.position);
	}

	void FixedUpdate() {
        // dull if stun
        {
			anim.color = anim.color.withAlpha(stunTime <= 0 ? 1.0f : 0.4f);
		}
        // decay stun
        {
			stunTime -= Time.deltaTime;
        }
		// stop mouse
		{
			if (Input.GetMouseButton(0) && stunTime <= 0) {
				GeneratingLine = true;
				Scroller.Inst.Scrolling = true;
			} else {
				GeneratingLine = false;
			}
		}
		// chase mouse
		{
			if (GeneratingLine) {
				var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition).to2();
				var vecToMouse = mousePos - rb.position;
				rb.velocity = vecToMouse.normalized * Mathf.Min(SpeedToMouse, vecToMouse.magnitude / Time.deltaTime);
			} else if (stunTime <= 0) {
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
				var dist = (Line[Line.Count - 1] - roundedPosition.to3()).magnitude;
				if (dist > 0.001f) {
					Line.Add(roundedPosition);
					totalDistance += dist;
				}
			} else {
				Line[Line.Count - 1] = roundedPosition;
			}
		}
		// decay from trail
		{
			if (Line.Count >= 2) {
				var decayRate = Mathf.Max(totalDistance / TrailDecayTime, GeneratingLine ? 0f : OffDecayTime);
				var decayAmount = decayRate * Time.deltaTime;
				while (Line.Count >= 2 && decayAmount > 0) {
					var nextDistance = (Line[1] - Line[0]).magnitude;
					if (nextDistance <= decayAmount) {
						Line.RemoveAt(0);
						totalDistance -= nextDistance;
						decayAmount -= nextDistance;
                    } else {
						Line[0] = Vector2.MoveTowards(Line[0], Line[1], decayAmount);
						totalDistance -= decayAmount;
						decayAmount = 0;
                    }
				}
            }
		}
        // set line
        {
			LineRenderer.positionCount = Line.Count;
			LineRenderer.SetPositions(Line.ToArray());
        }
	}

    private void OnCollisionEnter2D(Collision2D collision) {
		var stunBlock = collision.collider.GetComponent<StunBlock>();
		if (stunBlock) {
			GeneratingLine = false;
			stunTime = stunBlock.StunTime;
			rb.velocity = Vector2.zero;
			rb.AddForce(collision.GetContact(0).normal * stunBlock.PushForce, ForceMode2D.Impulse);
		}
    }

}