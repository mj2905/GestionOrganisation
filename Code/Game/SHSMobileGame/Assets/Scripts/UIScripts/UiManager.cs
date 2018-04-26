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
	public Text tokenText;
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

	public AchievementMenu achievement;
	public Shop shop;

	private PopupScript popup;

	private bool isAttackMode = false;
	private int previousCredit = 0;
	private int previousScore = 0;

	public GameObject leaderBoardMenu;
	public GameObject achievementMenu;
	public GameObject shopMenu;
	public GameObject levelTab;

	public Image backgroundTop;
	public Image backgroundBottom;

	private List<GameObject> menues;

	// Use this for initialization
	void Awake ()
	{
		GameObject clone = (GameObject)Instantiate(Resources.Load("Popup"));
		popup = clone.GetComponent<PopupScript>();		
		popup.transform.SetParent (this.transform.parent,false);
		popup.transform.SetAsLastSibling ();

		menues = new List<GameObject> (){ shopMenu,achievementMenu,leaderBoardMenu,levelTab };
	}

	public void UpdateTokens(int tokens) {
		tokenText.text = "Tokens: " + tokens;
	}

	public void UpdateUserStat(string xp, string credit, int team,string level,Effects effects,Statistics statistics,SkinsInfo skins){

		int creditAsInt = Int32.Parse (credit);
		int creditDiff = creditAsInt - previousCredit;
		previousCredit = creditAsInt;
		int lvlAsInt = Int32.Parse (level);
		int xpAsInt = Int32.Parse (xp);

		if(lvlAsInt < QuantitiesConstants.PLAYER_XP_THRESHOLDS.Length - 1){
			scoreBar.fillAmount = (float)(xpAsInt - QuantitiesConstants.PLAYER_XP_THRESHOLDS[lvlAsInt]) / (float)(QuantitiesConstants.PLAYER_XP_THRESHOLDS[lvlAsInt+1] - QuantitiesConstants.PLAYER_XP_THRESHOLDS[lvlAsInt]);
		} else {
			scoreBar.fillAmount = 1;
			scoreBar.color = ColorConstants.GRAY; 
		}
		creditText.text = credit;
		levelText.text = level;
		lth.setLevel (lvlAsInt);
		multiplierText.text = "x" + effects.GetTotalMultiplier();

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

		teamFlag.color = ColorConstants.getTextColor (team);
		teamText.text = ColorConstants.getColorAsString (team);
		teamText.color = ColorConstants.getTextColor (team);
	
		shop.SetSkinsInfo (skins);
		achievement.UpdateAchivement (statistics,skins,effects);
	}

	public void	UpdateCurrentMedalPosition (){
		for (int i = 0; i < medalList.Count; i++) {
			medalList [i].Item2.SetPosition (i);
		}
	}

	public void DeleteEffects(Effects effects){
		foreach (MedalInfo medal in effects.medals) {
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
			medalList.Add (new Tuple<string,Medal>(m.GetMedalInfo().GetName(),m));
		}
	}

	public void ModifyEffects(Effects effects){
		foreach (MedalInfo medal in effects.medals) {
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
		CloseAllMenues (achievementMenu);
		achievementMenu.gameObject.SetActive(!achievementMenu.gameObject.activeSelf);
	}

	public void ToggleShopMenu(){
		CloseAllMenues (shopMenu);
		shopMenu.gameObject.SetActive(!shopMenu.gameObject.activeSelf);
	}
		
	public void ToggleLeaderBoardMenu(){
		CloseAllMenues (leaderBoardMenu);
		leaderBoardMenu.gameObject.SetActive(!leaderBoardMenu.gameObject.activeSelf);
	}

	public void ToggleLevelTabMenu(){
		CloseAllMenues (levelTab);
		levelTab.gameObject.SetActive(!levelTab.gameObject.activeSelf);
	}


	private void CloseAllMenues(GameObject objectDontTouch){
		foreach (var item in menues) {
			if (item.name != objectDontTouch.name) {
				item.gameObject.SetActive (false);
			}
		}
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
