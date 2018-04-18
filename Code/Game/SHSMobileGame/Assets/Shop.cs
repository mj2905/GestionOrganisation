﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {

	private SkinsInfo skins;

	public PlayerShop playerShop;

	// Use this for initialization
	void Start () {
		this.skins = new SkinsInfo(null);
	}
	
	// Update is called once per frame
	void Update () {
		playerShop.setBoughtSkins (skins.boughtPlayer);
	}

	public void SetSkinsInfo(SkinsInfo skins){
		this.skins = skins;
	}
}
