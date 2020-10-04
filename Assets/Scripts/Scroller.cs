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

	[Range(0, 5)]
	public float DrumIntervalBase = 1;
	public float DrumSpeedFactor;

	public AudioClip DrumSound;
	float drumTime;

	AudioSource a;

	void Awake() {
		a = GetComponent<AudioSource>();
	}

	void Update() {
		var velocity = Scrolling ? VelocityOverTime.Evaluate(Scorer.Inst.TimeAlive) : 0f;
		currentVelocity = Mathf.Lerp(currentVelocity, velocity, Lerp);
		transform.localPosition += Vector3.up * velocity * Time.deltaTime;

		if (!wasScrolling && Scrolling) {
			EnableOnScroll.ForEach(go => go.SetActive(true));
			Scorer.Inst.Crash();
        } else if (wasScrolling && !Scrolling) {
			EnableOnScroll.ForEach(go => go.SetActive(false));
			Scorer.Inst.Crash();
		}
		
		wasScrolling = Scrolling;

		a.volume = Scrolling ? 0 : 1;

		// drum
		{
			if (Scrolling) {
				drumTime -= Time.deltaTime * (1 + currentVelocity * DrumSpeedFactor);
				if (drumTime <= 0) {
					drumTime = DrumIntervalBase;
					DrumSound.Play();
				}
			} else {
				drumTime = 0;
			}
		}
	}
}