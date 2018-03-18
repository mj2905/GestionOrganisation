using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : LocationListener
{
	private Effects previousEffects = new Effects();
	private List<Tuple<string, Medal>> medalList = new List<Tuple<string, Medal>>();

	public GameObject initialPosition;
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

	public Text numberOfTerminalPlaced;
	public Text numberOfTerminalBuffed;
	public Text numberOfTerminalDamaged;
	public Text numberOfZoneHealed;

	private PopupScript popup;

	private bool isAttackMode = false;
	private int previousCredit = 0;
	private int previousScore = 0;

	private int debugtmp = 0;
	private int tmpVal = 1000;

	private bool showAchievementMenu = false;
	public Canvas achievementMenu;

	public Image backgroundTop;
	public Image backgroundBottom;

	// Use this for initialization
	void Awake ()
	{
		GameObject clone = (GameObject)Instantiate(Resources.Load("Popup"));
		popup = clone.GetComponent<PopupScript>();		
		popup.transform.SetParent (this.transform.parent,false);
		popup.transform.SetAsLastSibling ();
	}

	public void UpdateUserStat(string xp, string credit, int team,string level,Effects effects,Statistics statistics){

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

		UpdateAchievement (statistics);

		Effects newEffects = effects.GetNewEffects (previousEffects);
		Effects modifiedEffects = effects.GetModifiedEffects (previousEffects);
		Effects deletedEffects = effects.GetDeletedEffects (previousEffects);

		DeleteEffects (deletedEffects);
		ModifyEffects (modifiedEffects);
		AddNewEffects (newEffects);

		UpdateCurrentMedalPosition ();

		previousEffects = effects;

		backgroundTop.color = ColorConstants.getColor (team);
		backgroundBottom.color = ColorConstants.getColor (team);
	}

	private void UpdateAchievement (Statistics statistics){
		foreach (KeyValuePair<string, int> entry in statistics.GetDict()) {
			switch (entry.Key) {
			case "numberOfTerminalPlaced":
				CheckIfAchievementUnlocked (numberOfTerminalPlaced, "numberOfTerminalPlaced", entry.Value);
				break;
			case "numberOfTerminalBuffed":
				CheckIfAchievementUnlocked (numberOfTerminalBuffed, "numberOfTerminalBuffed", entry.Value);
				break;
			case "numberOfTerminalDamaged":
				CheckIfAchievementUnlocked (numberOfTerminalDamaged, "numberOfTerminalDamaged", entry.Value);
				break;
			case "numberOfZoneHealed":
				CheckIfAchievementUnlocked (numberOfZoneHealed, "numberOfZoneHealed", entry.Value);
				break;
			}
		}
	}

	private void CheckIfAchievementUnlocked(Text text,string name,int num){
		int maxNum = EffectObtentionConstants.achievementMaxValue [name];
		if (num >= maxNum) {
			text.text = "DONE!";
		} else {
			text.text = num.ToString() + "/" + maxNum.ToString ();
		}
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
			m.SetInitialPosition (initialPosition.transform.position);
			m.SetPosition(medalList.Count);
			m.transform.SetParent (initialPosition.gameObject.transform);
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

	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		isAttackMode = false;
		backgroundTop.color = ColorConstants.getColor(FirebaseManager.userTeam);
		backgroundBottom.color = ColorConstants.getColor(FirebaseManager.userTeam);
	}

	override public void FirstLocationSent() {
		isAttackMode = true;
		backgroundTop.color = ColorConstants.attack;
		backgroundBottom.color = ColorConstants.attack;
	}

	public void ToggleAchievementMenu(){
		showAchievementMenu = !showAchievementMenu;
		Debug.Log (achievementMenu.enabled);
		achievementMenu.gameObject.SetActive(showAchievementMenu);
	}
}
