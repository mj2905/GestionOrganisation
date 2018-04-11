using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : LocationListener
{
	private Effects previousEffects = new Effects();
	private List<Tuple<string, Medal>> medalList = new List<Tuple<string, Medal>>();
	private List<Tuple<string, Notification>> notificationList = new List<Tuple<string, Notification>>();

	public GameObject initialMedalPosition;
	public GameObject initialNotificationPosition;

	public Medal MedalPrefab;
	public Notification NotificationPrefab;

	public Text creditText;
	public Image scoreBar;
	public Text levelText;
	public LevelTabHandler lth;
	public Text multiplierText;
	public Image teamFlag;
	public Text teamText;

	public GameManager game;
	public Animator turretButtonAnimator;
	public LocationHandler locationHandler;
	public TextUpdate positiveUpdate;
	public TextUpdate negativeUpdate;
	public GameObject creditUpdateHandle;
	//public GameObject scoreUpdateHandle;

	public List<Text> achievementsText;
	private Dictionary<string,Text> achievementsDict = new Dictionary<string,Text>();

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

		for (int i = 0; i < achievementsText.Count; i++) {
			achievementsDict.Add (achievementsText [i].name, achievementsText [i]);
		}
	}

	public void UpdateUserStat(string xp, string credit, int team,string level,Effects effects,Statistics statistics){

		int creditAsInt = Int32.Parse (credit);
		int creditDiff = creditAsInt - previousCredit;
		previousCredit = creditAsInt;
		int lvlAsInt = Int32.Parse (level);
		int xpAsInt = Int32.Parse (xp);
		if(lvlAsInt < QuantitiesConstants.PLAYER_XP_THRESHOLDS.Length - 1){
			//Debug.Log ("lvl as int: " + lvlAsInt);
			//Debug.Log ("xp: " + xpAsInt);
			//Debug.Log ("lvl threshold : " + QuantitiesConstants.PLAYER_XP_THRESHOLDS[lvlAsInt ]);
			//Debug.Log ("lvl threshold + 1 : " + QuantitiesConstants.PLAYER_XP_THRESHOLDS[lvlAsInt + 1]);
			scoreBar.fillAmount = (float)(xpAsInt - QuantitiesConstants.PLAYER_XP_THRESHOLDS[lvlAsInt ]) / (float)(QuantitiesConstants.PLAYER_XP_THRESHOLDS[lvlAsInt+1] - QuantitiesConstants.PLAYER_XP_THRESHOLDS[lvlAsInt ]);
		} else {
			scoreBar.fillAmount = 1;
			scoreBar.color = ColorConstants.GRAY; 
		}
		creditText.text = credit;
		levelText.text = level;
		lth.setLevel (lvlAsInt);
		multiplierText.text = "x" + 100 * effects.GetTotalMultiplier() + "%";

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

		teamFlag.color = ColorConstants.getTextColor (team);
		teamText.text = ColorConstants.getColorAsString (team);
		teamText.color = ColorConstants.getTextColor (team);

	}

	private void UpdateAchievement (Statistics statistics){
		foreach (KeyValuePair<string, int> entry in statistics.GetDict()) {
			CheckIfAchievementUnlocked (entry.Key, entry.Value);
		}
	}

	private void CheckIfAchievementUnlocked(string name,int num){
		Text text = achievementsDict [name];
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
			m.SetInitialPosition (initialMedalPosition.transform.position);
			m.SetPosition(medalList.Count);
			m.transform.SetParent (initialMedalPosition.gameObject.transform);
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
	}

	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		isAttackMode = false;
		//backgroundTop.color = ColorConstants.getColor(FirebaseManager.userTeam);
		//backgroundBottom.color = ColorConstants.getColor(FirebaseManager.userTeam);
	}

	override public void FirstLocationSent() {
		isAttackMode = true;
		//backgroundTop.color = ColorConstants.attack;
		//backgroundBottom.color = ColorConstants.attack;
	}

	public void ToggleAchievementMenu(){
		showAchievementMenu = !showAchievementMenu;
		Debug.Log (achievementMenu.enabled);
		achievementMenu.gameObject.SetActive(showAchievementMenu);
	}

	public void SetCurrentTerminals(Game previousGame,Game newGame,Dictionary<string, Zone> zoneDict){
		foreach (Terminal terminal in newGame.GetDeletedTerminals(previousGame)) {
			string name = "";
			foreach (var notification in notificationList) {
				if (notification.Item1.Contains(terminal.GetTerminalId ())) {
					name = notification.Item2.GetName ();
					notification.Item2.DestroyNotification ();
				}
			}
			removeNotification (name);
		}

		foreach(var terminal in newGame.GetNewAllyTerminals(previousGame)) {
			Notification n = (Notification)Instantiate (NotificationPrefab);
			n.SetPosition(notificationList.Count);
			n.SetInitialPosition (initialNotificationPosition.transform.position);
			n.SetType (Notification.Type.AllyTerminal);
			n.SetUi (this);
			n.SetText (zoneDict[terminal.Value.zoneId].name);
			n.SetName (terminal.Value.GetTerminalId () + "_ally");
			n.SetTargetPosition (zoneDict[terminal.Value.zoneId]);
			n.transform.SetParent (initialNotificationPosition.gameObject.transform);
			notificationList.Add (new Tuple<string,Notification>(n.GetName(),n));
		}

		foreach(var terminal in newGame.GetNewEnemyTerminals(previousGame)) {
			Notification n = (Notification)Instantiate (NotificationPrefab);
			n.SetPosition(notificationList.Count);
			n.SetInitialPosition (initialNotificationPosition.transform.position);
			n.SetType (Notification.Type.EnemyTerminal);
			n.SetUi (this);
			n.SetText (zoneDict[terminal.Value.zoneId].name);
			n.SetName (terminal.Value.GetTerminalId()+"_enemy");
			n.SetTargetPosition (zoneDict[terminal.Value.zoneId]);
			n.transform.SetParent (initialNotificationPosition.gameObject.transform);
			notificationList.Add (new Tuple<string,Notification>(n.GetName(),n));
		}

		for (int i = 0; i < notificationList.Count; i++) {
			notificationList [i].Item2.SetPosition (i);
		}
	}

	public void removeNotification(string name){
		notificationList.RemoveAll(item => item.Item1 == name);

		for (int i = 0; i < notificationList.Count; i++) {
			notificationList [i].Item2.SetPosition (i);
		}
	}
}
