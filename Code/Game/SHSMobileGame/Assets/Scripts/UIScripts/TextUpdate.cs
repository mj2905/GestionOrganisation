using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour {

	public Animator animator;
	private Text updateText;

	// Use this for initialization
	void Start () {
		AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo (0);
		Destroy (gameObject, clipInfo [0].clip.length);
		animator.GetComponent<Text> ().resizeTextForBestFit = true;
		animator.GetComponent<Text> ().alignment = TextAnchor.MiddleRight;

	}
		
	public void setText(string text){
		animator.GetComponent<Text> ().text = text;
	}
		
}
