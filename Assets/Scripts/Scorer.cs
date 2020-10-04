using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Scorer : Manager<Scorer> {

    public float Score;
    public float TimeAlive;

    public int ComboMultiplier = 1;
    int comboLevel = 0;
    [Range(0f, 2f)]
    public float BaseComboTime = 0.7f;
    [Range(0f, 0.5f)]
    public float ComboMinus = 0.12f;
    float comboTime;

    public SpriteRenderer Grid;

    bool canRestart;

    void Awake() {
        Grid.transform.position = Grid.transform.position.withX(Mathf.Lerp(-1.5f, 1.5f, Random.value));
        Grid.transform.localScale = new Vector3(
            Grid.transform.localScale.x * (Random.value < 0.5f ? 1 : -1),
            Grid.transform.localScale.y * (Random.value < 0.5f ? 1 : -1),
            1
        );
    }

    void Update() {
        if (canRestart && Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate() {
        // decay combo time
        {
            if (comboTime > 0) {
                comboTime -= Time.deltaTime;
                if (comboTime <= 0) {
                    ComboMultiplier = ResetFibbonnacci();
                }
            }
        }
        // track time
        {
            if (Scroller.Inst.Scrolling) {
                TimeAlive += Time.deltaTime;
            }
        }
	}

    public void OnStarGet(float score) {
        comboTime = BaseComboTime - ComboMinus * comboLevel;
        ComboMultiplier = NextFibbonacci();
        var totalScore = score * ComboMultiplier;
        Score += totalScore;
        PlayerPrefs.SetFloat("MaxStar", Mathf.Max(PlayerPrefs.GetFloat("MaxStar"), totalScore));
    }

    int a = 0, b = 1;
    int NextFibbonacci() {
        comboLevel++;
        var next = a + b;
        a = b;
        b = next;
        return next;
    }
    int ResetFibbonnacci() {
        comboLevel = 0;
        a = 0;
        b = 1;
        return a + b;
    }

    public void GameOver() {
        PlayerPrefs.SetFloat("MaxScore", Mathf.Max(PlayerPrefs.GetFloat("MaxScore"), Score));
        PlayerPrefs.SetFloat("MaxTime", Mathf.Max(PlayerPrefs.GetFloat("MaxTime"), TimeAlive));
        Scroller.Inst.Scrolling = false;

        Debug.Log($"SCORE: {Score} BEST: {PlayerPrefs.GetFloat("MaxScore")}");
        Debug.Log($"TIME: {TimeAlive} BEST: {PlayerPrefs.GetFloat("MaxTime")}");
        Debug.Log($"BEST STAR: {PlayerPrefs.GetFloat("MaxStar")}");

        StartCoroutine(DoGameOverStuff());
    }

    IEnumerator DoGameOverStuff() {
        Grid.maskInteraction = SpriteMaskInteraction.None;
        var originalA = Grid.color.a;
        while (Grid.color.a > 0.01f) {
            Grid.color = Grid.color.withAlpha(Mathf.Lerp(Grid.color.a, 0f, 0.12f));
            yield return null;
        }
        while (Grid.color.a < originalA) {
            Grid.color = Grid.color.withAlpha(Mathf.Lerp(Grid.color.a, 1f, 0.12f));
            yield return null;
        }
        while (Grid.color.a > 0.01f) {
            Grid.color = Grid.color.withAlpha(Mathf.Lerp(Grid.color.a, 0f, 0.3f));
            yield return null;
        }
        while (Grid.color.a < originalA) {
            Grid.color = Grid.color.withAlpha(Mathf.Lerp(Grid.color.a, 1f, 0.3f));
            yield return null;
        }
        Grid.color = Grid.color.withAlpha(originalA);
        canRestart = true;
    }
}