using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scroller : Manager<Scroller> {

	public bool Scrolling;
	bool wasScrolling;

	public AnimationCurve VelocityOverTime = AnimationCurve.Linear(0, 0, 1, 1);
	float currentVelocity;
	[Range(0, 0.025f)]
	public float Lerp = 0.01f;

	public GameObject[] EnableOnScroll;

	void Awake() {
	}

	void Update() {
		var velocity = Scrolling ? VelocityOverTime.Evaluate(Scorer.Inst.TimeAlive) : 0f;
		currentVelocity = Mathf.Lerp(currentVelocity, velocity, Lerp);
		transform.localPosition += Vector3.up * velocity * Time.deltaTime;

		if (!wasScrolling && Scrolling) {
			EnableOnScroll.ForEach(go => go.SetActive(true));
        } else if (wasScrolling && !Scrolling) {
			EnableOnScroll.ForEach(go => go.SetActive(false));
		}
		
		wasScrolling = Scrolling;
	}
}