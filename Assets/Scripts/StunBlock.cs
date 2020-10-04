using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StunBlock : MonoBehaviour
{
	[Range(0, 3f)]
	public float StunTime = 1f;
	public float PushForce = 2f;
}