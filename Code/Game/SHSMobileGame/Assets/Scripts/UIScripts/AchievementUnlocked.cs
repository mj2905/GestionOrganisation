using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AchievementUnlocked : MonoBehaviour {

	private Vector3 initPosImage;
	private string achievement = "Achievement unlocked";
	public Image image;
	public Text text;
	public Image imageRef;

	public Sprite[] sprites;

	public void setAchievement(string text) {
		string outV;
		achievement = "Achievement unlocked:\n" + (QuantitiesConstants.nameAchievementDisplay.TryGetValue(text, out outV) ? outV : "");
		int spriteId = Array.FindIndex (sprites, x => x.name.Equals(text));
		if (spriteId >= 0) {
			imageRef.gameObject.SetActive (true);
			imageRef.sprite = sprites [spriteId];
		} else {
			imageRef.sprite = null;
			imageRef.gameObject.SetActive (false);
		}
	}

	void OnEnable() {
		image.transform.localPosition = new Vector3(0, 750, 0);
	}

	// Update is called once per frame
	void Update () {
		text.text = achievement;
		if (image.transform.localPosition.y > -539) {
			Vector3 newPos = image.transform.localPosition;
			newPos.y -= Time.deltaTime * ((Math.Abs(image.transform.localPosition.y) > 3) ? (Math.Abs(image.transform.localPosition.y))*1.5f + 150 : 1);
			image.transform.localPosition = newPos;
		} else {
			gameObject.SetActive (false);
		}
	}
}
