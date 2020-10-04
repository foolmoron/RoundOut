using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class Scorer : Manager<Scorer> {

    public float Score;
    public float TimeAlive;
    public float BestStar;

    public int ComboMultiplier = 1;
    int comboLevel = 0;
    [Range(0f, 2f)]
    public float BaseComboTime = 0.7f;
    [Range(0f, 0.5f)]
    public float ComboMinus = 0.12f;
    float comboTime;

    public SpriteRenderer Grid;

    public GameObject CaptureTextPrefab;

    public GameObject ComboBar;
    public TextMeshPro ComboText;
    float comboScaleOrig;

    public TextMeshPro TextTime;
    public TextMeshPro TextScore;
    public TextMeshPro TextBestTime;
    public TextMeshPro TextBestScore;
    public TextMeshPro TextBestStar;

    public TextMeshPro TextEndTime;
    public TextMeshPro TextEndScore;
    public TextMeshPro TextEndStar;
    public TextMeshPro TextEndBestTime;
    public TextMeshPro TextEndBestScore;
    public TextMeshPro TextEndBestStar;

    public GameObject[] EndTexts;

    bool canRestart;

    public AudioClip[] CrashSounds;
    public AudioClip BloopSound;

    void Awake() {
        Grid.transform.position = Grid.transform.position.withX(Mathf.Lerp(-1.5f, 1.5f, Random.value));
        Grid.transform.localScale = new Vector3(
            Grid.transform.localScale.x * (Random.value < 0.5f ? 1 : -1),
            Grid.transform.localScale.y * (Random.value < 0.5f ? 1 : -1),
            1
        );
        TextBestTime.text = PlayerPrefs.GetFloat("MaxTime").ToString("0.00s");
        TextBestScore.text = PlayerPrefs.GetFloat("MaxScore").ToString("0");
        TextBestStar.text = PlayerPrefs.GetFloat("MaxStar").ToString("0");
        comboScaleOrig = ComboBar.transform.localScale.x;
    }

    public void Crash() {
        StartCoroutine(DoCrash());
    }

    IEnumerator DoCrash() {
        foreach (var s in CrashSounds) {
            s.PlayWithPitchRange(0.85f, 1.1f, 1f);
            yield return null;
            s.PlayWithPitchRange(0.85f, 1.1f, 1f);
            yield return null;
        }
    }

    void Update() {
        // restart
        if (canRestart && Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        // tests
        TextTime.text = TimeAlive.ToString("0.00s");
        TextScore.text = Score.ToString("0");
        // combo
        var comboPerc = Mathf.Clamp01(comboTime / (BaseComboTime - ComboMinus * comboLevel));
        ComboBar.transform.localScale = ComboBar.transform.localScale.withX(comboPerc * comboScaleOrig);
        ComboText.text = ComboMultiplier.ToString("0x");
        ComboText.gameObject.SetActive(ComboMultiplier > 1);
        ComboText.transform.localScale = Vector3.one * (0.01834f * ComboMultiplier + 0.31155f);
    }

    void FixedUpdate() {
        // decay combo time
        {
            comboTime -= Time.deltaTime;
            if (comboTime <= 0) {
                comboTime = 0;
                ComboMultiplier = ResetFibbonnacci();
            }
        }
        // track time
        {
            if (Scroller.Inst.Scrolling) {
                TimeAlive += Time.deltaTime;
            }
        }
	}

    public void OnStarGet(float score, Vector2 position) {
        // increment multiplier
        var multiplier = ComboMultiplier;
        ComboMultiplier = NextFibbonacci();

        // add to score
        comboTime = BaseComboTime - ComboMinus * comboLevel;
        var totalScore = score * multiplier;
        Score += totalScore;
        BestStar = Mathf.Max(BestStar, totalScore);

        // capture text
        var text = Instantiate(CaptureTextPrefab, position, Quaternion.identity);
        var scale = 0.107825f * multiplier + 0.699675f;
        text.transform.localScale = Vector2.one * scale;
        StartCoroutine(FlashCaptureText(text, (int)score, multiplier, (int)totalScore));
    }

    int a = 1, b = 1;
    int NextFibbonacci() {
        comboLevel++;
        var next = a + b;
        a = b;
        b = next;
        return next;
    }
    int ResetFibbonnacci() {
        comboLevel = 0;
        a = 1;
        b = 1;
        return 1;
    }

    IEnumerator FlashCaptureText(GameObject captureText, int initial, int multiplier, int score) {
        var mult = captureText.transform.Find("Mult").GetComponent<TextMeshPro>();
        mult.text = $"{initial} x {multiplier}";
        var total = captureText.transform.Find("Total").GetComponent<TextMeshPro>();
        total.text = $"{score}";

        captureText.SetActive(true);
        yield return new WaitForSeconds(Mathf.Lerp(0.45f, 0.65f, Random.value));
        captureText.SetActive(false);
        yield return new WaitForSeconds(Mathf.Lerp(0.035f, 0.06f, Random.value));
        captureText.SetActive(true);
        yield return new WaitForSeconds(Mathf.Lerp(0.1f, 0.3f, Random.value));
        captureText.SetActive(false);
        yield return new WaitForSeconds(Mathf.Lerp(0.035f, 0.06f, Random.value));
        captureText.SetActive(true);
        yield return new WaitForSeconds(Mathf.Lerp(0.035f, 0.06f, Random.value));
        captureText.SetActive(false);
        Destroy(captureText);
    }

    bool gameOver;
    public void GameOver() {
        if (gameOver) return;
        gameOver = true;
        PlayerPrefs.SetFloat("MaxScore", Mathf.Max(PlayerPrefs.GetFloat("MaxScore"), Score));
        PlayerPrefs.SetFloat("MaxTime", Mathf.Max(PlayerPrefs.GetFloat("MaxTime"), TimeAlive));
        PlayerPrefs.SetFloat("MaxStar", Mathf.Max(PlayerPrefs.GetFloat("MaxStar"), BestStar));
        Scroller.Inst.Scrolling = false;
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
        yield return new WaitForSeconds(Mathf.Lerp(0.8f, 1.5f, Random.value));
        Grid.color = Grid.color.withAlpha(originalA);


        TextEndTime.text = TimeAlive.ToString("0.00s");
        TextEndScore.text = Score.ToString("0");
        TextEndStar.text = BestStar.ToString("0");
        TextEndBestTime.text = PlayerPrefs.GetFloat("MaxTime").ToString("0.00s");
        TextEndBestScore.text = PlayerPrefs.GetFloat("MaxScore").ToString("0");
        TextEndBestStar.text = PlayerPrefs.GetFloat("MaxStar").ToString("0");
        foreach (var t in EndTexts) {
            t.gameObject.SetActive(true);
            BloopSound.PlayWithPitchRange(0.8f, 1.15f, 1.6f);
            BloopSound.PlayWithPitchRange(0.8f, 1.15f, 1.6f);
            yield return new WaitForSeconds(Mathf.Lerp(0.1f, 0.45f, Random.value));
        }

        canRestart = true;
    }
}