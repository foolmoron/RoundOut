using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisableOnScroll : MonoBehaviour
{
	void Update() {
        if (Scroller.Inst.Scrolling) {
            gameObject.SetActive(false);
        }
    }
}