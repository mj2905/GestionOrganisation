using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkin : MonoBehaviour {

	public int price;
	public string nameSkin;
	public int number;

	public Text text;
	public GameObject check;
	public GameObject mask;

	private bool isSelected,isBought;

	// Use this for initialization
	void Start () {
		check.SetActive (false);
		mask.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
		if (!isBought) {
			text.text = price + " Credits";
			mask.SetActive (true);
		} else {
			text.text = nameSkin;
			mask.SetActive (false);
		}

		if (isSelected) {
			check.SetActive (true);
		} else {
			check.SetActive (false);
		}
	}

	public void SetIsBought(bool isBought){
		this.isBought = isBought;
	}

	public void SetIsSelected(bool isSelected){
		this.isSelected = isSelected;
	}

	public int GetNumber(){
		return number;
	}
}
