using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShop : MonoBehaviour {
	public List<PlayerSkin> playerSkin;
	public Shop shop;

	private int selected;
	private List<int> boughtSkins = new List<int>();

	// Use this for initialization
	void Start () {
		playerSkin [0].SetIsBought (true);
		playerSkin [0].SetIsSelected (true);
		selected = 0;
	}

	public void setBoughtSkins(List<int> boughtSkins){
		this.boughtSkins = boughtSkins;
		boughtSkins.Add (0);
	}

	// Update is called once per frame
	void Update () {
		for (int i = 0; i < playerSkin.Count; i++) {
			if (i == selected) {
				playerSkin [i].SetIsSelected (true);
			} else {
				playerSkin [i].SetIsSelected (false);
			}
		}

		foreach (var elem in playerSkin) {
			if (boughtSkins.Contains (elem.GetNumber())) {
				elem.SetIsBought (true);
			} else {
				elem.SetIsBought (false);
			}
		}
	}

	public void selection(int number){
		if (boughtSkins.Contains (number)) {
			selected = number;
		} else {
			shop.BuyNewItem (playerSkin[number].price,number);
		}
	}
}
