using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PatternSpawner : MonoBehaviour {

	public float PatternInterval = 3;
	float lastSpawn = float.NegativeInfinity;

	public Transform[] SpawnPoints;

	public Pattern[] EasyPatterns;
	public AnimationCurve EasyChanceOverTime = AnimationCurve.Linear(0, 0, 1, 1);
	public Pattern[] MediumPatterns;
	public AnimationCurve MediumChanceOverTime = AnimationCurve.Linear(0, 0, 1, 1);
	public Pattern[] HardPatterns;
	public AnimationCurve HardChanceOverTime = AnimationCurve.Linear(0, 0, 1, 1);

	void Awake() {
	}

	void FixedUpdate() {
        // instantiate patterns every interval
        {
			if (transform.position.y >= lastSpawn + PatternInterval) {
				lastSpawn = transform.position.y;
				foreach (var spawnPoint in SpawnPoints) {
					InstantiatePattern(spawnPoint.position);
                }
            }
        }
    }

	void InstantiatePattern(Vector2 position) {
		Pattern pattern;
		var time = Scorer.Inst.TimeAlive;
		var easyChance = Mathf.Max(0, EasyChanceOverTime.Evaluate(time));
		var mediumChance = Mathf.Max(0, MediumChanceOverTime.Evaluate(time));
		var hardChance = Mathf.Max(0, HardChanceOverTime.Evaluate(time));
		var val = Random.value * (easyChance + mediumChance + hardChance);
		if (val > (easyChance + mediumChance)) {
			pattern = HardPatterns.Random();
		} else if (val > easyChance) {
			pattern = MediumPatterns.Random();
		} else {
			pattern = EasyPatterns.Random();
		}
		Instantiate(pattern, position, Quaternion.identity);
	}
}