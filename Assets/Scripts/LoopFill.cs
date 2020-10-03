using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.U2D;
using Unity.Collections;
using System;

public class LoopFill : MonoBehaviour {

	SpriteShapeController spriteShape;

    private void Awake() {
        spriteShape = GetComponent<SpriteShapeController>();
    }

    public void SetPoints(List<Vector3> line, Vector3 intersection, int start, int end) {
        spriteShape.transform.position = intersection;
        spriteShape.spline.Clear();
        spriteShape.spline.InsertPointAt(0, Vector2.zero);
        spriteShape.spline.SetTangentMode(0, ShapeTangentMode.Linear);
        var p = 1;
        for (int i = start + 1; i < end; i++) {
            try {
                spriteShape.spline.InsertPointAt(p, line[i] - intersection);
                spriteShape.spline.SetTangentMode(p, ShapeTangentMode.Linear);
                p++;
            } catch (ArgumentException e) {
                // point too close exception, just skip it
            }
        }
    }
}