using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.U2D;
using Unity.Collections;
using System;

public class LoopFill : MonoBehaviour {

    SpriteShapeController spriteShape;
    PolygonCollider2D polygonCollider;

    private void Awake() {
        spriteShape = GetComponent<SpriteShapeController>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    public void SetPoints(List<Vector3> line, Vector3 intersection, int start, int end) {
        List<Vector2> finalPoints = new List<Vector2>();
        spriteShape.transform.position = intersection;
        spriteShape.spline.Clear();
        spriteShape.spline.InsertPointAt(0, Vector2.zero);
        spriteShape.spline.SetTangentMode(0, ShapeTangentMode.Linear);
        finalPoints.Add(Vector2.zero);
        var p = 1;
        for (int i = start + 1; i < end; i++) {
            try {
                var point = line[i] - intersection;
                spriteShape.spline.InsertPointAt(p, point);
                spriteShape.spline.SetTangentMode(p, ShapeTangentMode.Linear);
                finalPoints.Add(point);
                p++;
            } catch (ArgumentException e) {
                // point too close exception, just skip it
            }
        }
        // collider
        polygonCollider.points = finalPoints.ToArray();
    }
}