using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System;

public class Circler : Manager<Circler> {

    struct LoopKey {
        public int points;
        public float intersectionY;
        public float intersectionX;
        public float sumX;
        public float sumY;
        public override bool Equals(object other) {
            return other is LoopKey key && this == key;
        }
        public override int GetHashCode() {
            int hashCode = -983117098;
            hashCode = hashCode * -1521134295 + points.GetHashCode();
            hashCode = hashCode * -1521134295 + intersectionX.GetHashCode();
            hashCode = hashCode * -1521134295 + intersectionY.GetHashCode();
            hashCode = hashCode * -1521134295 + sumX.GetHashCode();
            hashCode = hashCode * -1521134295 + sumY.GetHashCode();
            return hashCode;
        }
        public static bool operator ==(LoopKey x, LoopKey y) {
            return (x.intersectionX == y.intersectionX && x.intersectionY == y.intersectionY && x.points == y.points)
                || (x.sumX == y.sumX && x.sumY == y.sumY);
        }
        public static bool operator !=(LoopKey x, LoopKey y) {
            return !(x == y);
        }
    }

	public Navigator Nav;
    
    public GameObject LoopFillPrefab;
    ObjectPool loopFillPool;

    const int MIN_EDGES_PER_LOOP = 5;
    readonly ListDict<LoopKey, (int start, int end)> detectedLoops = new ListDict<LoopKey, (int start, int end)>(10);
    readonly ListDict<LoopKey, LoopFill> loops = new ListDict<LoopKey, LoopFill>(10);

	float cross(Vector3 a, Vector3 b) => a.x * b.y - a.y * b.x;

    void Awake() {
        loopFillPool = LoopFillPrefab.GetObjectPool(40);
    }

    void Update() {
		var line = Nav.Line;
        // detect intersections
        {
            detectedLoops.Clear();
            for (int i = 0; i < line.Count - 1; i++) {
                var p = line[i];
                var r = line[i + 1] - p;
                for (int j = i + MIN_EDGES_PER_LOOP; j < line.Count - 1; j++) {
                    var q = line[j];
                    var s = line[j + 1] - q;
                    // calculate intersection based on some formula
                    var qp = q - p;
                    var rxs = cross(r, s);
                    var t = cross(qp, s) / rxs;
                    var u = cross(qp, r) / rxs;
                    if (rxs != 0 && 0 <= t && t <= 1 && 0 <= u && u <= 1) {
                        // intersection!
                        var intersection = p + t * r;
                        var sum = new Vector2();
                        for (int z = i + 1; z <= j; z++) {
                            sum.x += line[z].x;
                            sum.y += line[z].y;
                        }
                        detectedLoops.Add(new LoopKey {
                            points = (1 + j) - i,
                            intersectionX = intersection.x,
                            intersectionY = intersection.y,
                            sumX = sum.x,
                            sumY = sum.y,
                        }, (i, j + 1));
                        i = j + 1;
                        break;
                    }
                }
            }
        }
        // kill old loops
        {
            for (int i = 0; i < loops.Count; i++) {
                if (!detectedLoops.ContainsKey(loops.Keys[i])) {
                    loops.Values[i].gameObject.Release();
                    loops.RemoveAt(i);
                    i--;
                }
            }
        }
        // fill loops
        {
            for (int i = 0; i < detectedLoops.Count; i++) {
                if (!loops.ContainsKey(detectedLoops.Keys[i])) {
                    var loopFill = loopFillPool.Obtain<LoopFill>();
                    loopFill.SetPoints(
                        Nav.Line, 
                        new Vector2(detectedLoops.Keys[i].intersectionX, detectedLoops.Keys[i].intersectionY),
                        detectedLoops.Values[i].start, detectedLoops.Values[i].end
                    );
                    loops.Add(detectedLoops.Keys[i], loopFill);
                }
            }
        }
	}
}