using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : LocationListener
{

	public Text creditText;
	public Text scoreText;
	public Text levelText;

	public GameManager game;
	public Animator turretButtonAnimator;
	public LocationHandler locationHandler;
	public TextUpdate positiveUpdate;
	public TextUpdate negativeUpdate;
	public GameObject creditUpdateHandle;
	public GameObject scoreUpdateHandle;

	private PopupScript popup;

	private bool isAttackMode = false;
	private int previousCredit = 0;
	private int previousScore = 0;

	private int debugtmp = 0;
	private int tmpVal = 1000;


	// Use this for initialization
	void Awake ()
	{
		GameObject clone = (GameObject)Instantiate(Resources.Load("Popup"));
		popup = clone.GetComponent<PopupScript>();		
		popup.transform.SetParent (this.transform.parent,false);
		popup.transform.SetAsLastSibling ();
	}

	public void UpdateUserStat(string xp, string credit,string level){


		int creditAsInt = Int32.Parse (credit);

		int creditDiff = creditAsInt - previousCredit;
		previousCredit = creditAsInt;

		if (creditDiff < 0) {
			TextUpdate textUpdate = (TextUpdate)Instantiate (negativeUpdate, creditUpdateHandle.transform);
			textUpdate.transform.position = creditUpdateHandle.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f), 0, 0);
			textUpdate.setText (creditDiff.ToString ());
		} else if (creditDiff > 0) {
			TextUpdate textUpdate = (TextUpdate)Instantiate (positiveUpdate, creditUpdateHandle.transform);
			textUpdate.transform.position = creditUpdateHandle.transform.position + new Vector3(UnityEngine.Random.Range(-10f, 10f), 0, 0);
			textUpdate.setText (creditDiff.ToString ());
		}

		scoreText.text = "Xp: " + xp;
		creditText.text = "Credits: " + credit;
		levelText.text = "Level: " + level;
	}

	public void SetPopUpText(string text){
		popup.SetText (text);
	}

	public void PlaceTurret(){
		if (game.IsPlayerInsideZone ()) {
			turretButtonAnimator.SetBool ("isClicked", true);
			locationHandler.DeactivateLocation ();
			game.AddTerminalAtPlayerPosition ();
		}
	}

	void Update(){
		turretButtonAnimator.SetBool ("isClicked", false);
		turretButtonAnimator.SetBool ("isInside", isAttackMode && game.IsPlayerInsideZone());

		//Debug purposes only
		/*
		debugtmp = (debugtmp + 1) % 60;

		if (debugtmp == 0) {
			UpdateScoreAndCredit ("0", tmpVal.ToString());
			tmpVal -= 50;
		}
		*/
	}

	override public void CoordinateUpdate(MapCoordinate coords) {}

	override public void StopLocationHandling() {
		isAttackMode = false;
	}

	override public void FirstLocationSent() {
		isAttackMode = true;
	}
}
