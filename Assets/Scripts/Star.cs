using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Star : MonoBehaviour {

	public float BaseScore = 200;
	public float ScorePerSecond = 500;
	[Range(0f, 1f)]
	public float ScorePerSecondBuffer = 0.2f;
	float score;
	float scorePerSecondBuffer = 0;

	bool triggered;
	int triggers;
	
	void Awake() {
		GetComponentsInChildren<Rotating>().ForEach(r => r.DegreesPerSecond.z = Mathf.Lerp(40, 80, Random.value) * (Random.value < 0.5f ? 1 : -1));
		GetComponentsInChildren<AlphaPulser>().ForEach(a => a.PulseOffset = Mathf.Lerp(0, 1, Random.value));

		score = BaseScore;
	}

    void FixedUpdate() {
		if (triggers > 0) {
			triggered = true;
			scorePerSecondBuffer += Time.deltaTime;
			if (scorePerSecondBuffer >= ScorePerSecondBuffer) {
				score += ScorePerSecond * Time.deltaTime;
            }
        }
        // die and give score
        {
			if (triggered && triggers == 0) {
				Scorer.Inst.OnStarGet(score);
				Destroy(gameObject);
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
		triggers++;
	}

	private void OnTriggerExit2D(Collider2D collision) {
		triggers--;
	}
}