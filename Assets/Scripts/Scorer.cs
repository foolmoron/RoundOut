using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System;

public class Scorer : Manager<Scorer> {

    public float Score;

    public int ComboMultiplier = 1;
    [Range(0f, 2f)]
    public float ComboTime = 0.5f;
    float comboTime;

    void Awake() {
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
	}

    public void OnStarGet(float score) {
        comboTime = ComboTime;
        ComboMultiplier = NextFibbonacci();
        Debug.Log($"Got star! {score} x {ComboMultiplier} = {score * ComboMultiplier}");
        Score += score * ComboMultiplier;
    }

    int a = 0, b = 1;
    int NextFibbonacci() {
        var next = a + b;
        a = b;
        b = next;
        return next;
    }
    int ResetFibbonnacci() {
        a = 0;
        b = 1;
        return a + b;
    }
}