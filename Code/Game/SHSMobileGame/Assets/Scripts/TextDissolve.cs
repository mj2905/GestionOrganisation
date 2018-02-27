using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDissolve : MonoBehaviour {

	private bool ascent = true;
	public float speed = 1.0f;

	// Update is called once per frame
	void Update () {
		float alpha = GetComponent<CanvasRenderer> ().GetAlpha();
		alpha += (ascent ? 1 : -1) * speed * Time.deltaTime;
		if (alpha < 0) {
			ascent = !ascent;
			alpha = 0;
		} else if (alpha > 1) {
			ascent = !ascent;
			alpha = 1;
		}
		GetComponent<CanvasRenderer> ().SetAlpha (alpha);
	}
}
