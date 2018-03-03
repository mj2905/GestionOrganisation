using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : LocationListener
{

	public Text creditText;
	public Text scoreText;
	public GameManager game;
	public Animator turretButtonAnimator;

	private PopupScript popup;

	private bool isAttackMode = false;


	// Use this for initialization
	void Awake ()
	{
		GameObject clone = (GameObject)Instantiate(Resources.Load("Popup"));
		popup = clone.GetComponent<PopupScript>();		
		popup.transform.SetParent (this.transform.parent,false);
		popup.transform.SetAsLastSibling ();
	}

	public void UpdateScoreAndCredit(string xp, string credit){
		scoreText.text = "Xp: " + xp;
		creditText.text = "Credits: " + credit;
	}

	public void SetPopUpText(string text){
		popup.SetText (text);
	}

	public void PlaceTurret(){
		if (game.IsPlayerInsideZone ()) {
			turretButtonAnimator.SetBool ("isClicked", true);
			game.AddTerminalAtPlayerPosition ();
		}
	}

	void Update(){
		turretButtonAnimator.SetBool ("isClicked", false);
		turretButtonAnimator.SetBool ("isInside", isAttackMode && game.IsPlayerInsideZone());
	}

	override public void CoordinateUpdate(double latitude, double longitude) {}

	override public void StopLocationHandling() {
		isAttackMode = false;
	}

	override public void FirstLocationSent() {
		isAttackMode = true;
	}
}
