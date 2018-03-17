using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : LocationListener
{
	private Effects previousEffects = new Effects();
	private Canvas canvas;
	private List<Tuple<string, Medal>> medalList = new List<Tuple<string, Medal>>();

	public Transform initialPosition;
	public Medal MedalPrefab;

	public Text creditText;
	public Text scoreText;
	public Text levelText;
	public Text multiplierText;

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

	private bool showAchievementMenu = false;
	public Canvas achievementMenu;

	// Use this for initialization
	void Awake ()
	{
		GameObject clone = (GameObject)Instantiate(Resources.Load("Popup"));
		popup = clone.GetComponent<PopupScript>();		
		popup.transform.SetParent (this.transform.parent,false);
		popup.transform.SetAsLastSibling ();
		canvas = GetComponentInParent<Canvas> ();
	}

	public void UpdateUserStat(string xp, string credit,string level,Effects effects){

		int creditAsInt = Int32.Parse (credit);
		int creditDiff = creditAsInt - previousCredit;
		previousCredit = creditAsInt;

		scoreText.text = "Xp: " + xp;
		creditText.text = "Credits: " + credit;
		levelText.text = "Level: " + level;
		multiplierText.text = "Multiplier: x" + effects.GetTotalMultiplier();

		if (creditDiff < 0) {
			TextUpdate textUpdate = (TextUpdate)Instantiate (negativeUpdate, creditUpdateHandle.transform);
			textUpdate.transform.position = creditUpdateHandle.transform.position;
			textUpdate.setText (creditDiff.ToString ());
		} else if (creditDiff > 0) {
			TextUpdate textUpdate = (TextUpdate)Instantiate (positiveUpdate, creditUpdateHandle.transform);
			textUpdate.transform.position = creditUpdateHandle.transform.position;
			textUpdate.setText ('+'+creditDiff.ToString ());
		}

		Effects newEffects = effects.GetNewEffects (previousEffects);
		Effects modifiedEffects = effects.GetModifiedEffects (previousEffects);
		Effects deletedEffects = effects.GetDeletedEffects (previousEffects);

		DeleteEffects (deletedEffects);
		ModifyEffects (modifiedEffects);
		AddNewEffects (newEffects);

		UpdateCurrentMedalPosition ();

		previousEffects = effects;
	}

	public void	UpdateCurrentMedalPosition (){
		for (int i = 0; i < medalList.Count; i++) {
			medalList [i].Item2.SetPosition (i);
		}
	}

	public void DeleteEffects(Effects effects){
		foreach (Medal medal in effects.medals) {
			foreach (var medalUnity in medalList) {
				if (medalUnity.Item1 == medal.GetName ()) {
					medalUnity.Item2.DestroyMedal ();
				}
			}
			medalList.RemoveAll(item => item.Item1 == medal.GetName ());
		}
	}

	public void AddNewEffects(Effects effects){
		for (int i = 0; i < effects.medals.Count; i++) {
			Medal m = (Medal)Instantiate (MedalPrefab);
			m.SetInitialPosition (initialPosition.position);
			m.SetPosition(medalList.Count);
			m.transform.SetParent (canvas.transform);
			m.Copy (effects.medals[i]);
			medalList.Add (new Tuple<string,Medal>(m.GetName(),m));
		}
	}

	public void ModifyEffects(Effects effects){
		foreach (Medal medal in effects.medals) {
			foreach (var medalUnity in medalList) {
				if (medalUnity.Item1 == medal.GetName ()) {
					medalUnity.Item2.Copy (medal);
				}
			}
		}
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

	public void ToggleAchievementMenu(){
		showAchievementMenu = !showAchievementMenu;
		achievementMenu.enabled = showAchievementMenu;
	}
}
