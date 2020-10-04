using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoundMover : MonoBehaviour
{
    public float Radius;
    public float Speed;
    [Range(0, 360)]
    public float Angle;
    Vector2 originalPos;

    Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        originalPos = rb.position;
    }

    void Update() {
        Angle = (Angle + Speed * Time.deltaTime) % 360f;
        rb.position = Angle.toVectorDeg() * Radius + originalPos;
    }

}