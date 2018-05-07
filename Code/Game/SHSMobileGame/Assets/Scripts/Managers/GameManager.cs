using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : LocationListener {

	public UiManager uiManager;
	public InteractionManager interactionManager;
	public GameObject sceneRoot;
	public GameObject zonesRoot;
	public Terminal terminalPrefab;

	public PlayerController player;

	public Button modeButton;
	public Zone[] zones;
	private Dictionary<string, Zone> zoneDict = new Dictionary<string, Zone>();
	private Dictionary<string, Terminal> terminalDict = new Dictionary<string, Terminal>();

	private int team = -1;
	private Effects currentEffects = new Effects();
	private string currentXp;

	private Game previousGame;
	private Game currentGame;
	private string zoneIdClicked;

	public LeaderBoardManager leaderboardManager;

	public PopupScript messagePopup;

	public GameObject waitingScreen;

	public AchievementUnlocked achievement;
	private Action achievementFunction = () => {};

	void Awake(){
		FirebaseManager.SetMainGameRef (this);
	}

	// Use this for initialization
	void Start () {
        waitingScreen.SetActive (true);		

		FirebaseManager.SetListenerEnd ();
		FirebaseManager.SetListenerCreditScore ();
		FirebaseManager.SetListenerGame ();

		previousGame = new Game ();
		currentGame = new Game ();

		for (int i = 0; i < zones.Length; i++) {
			zoneDict[zones [i].zoneId] = zones [i];
		}
			

		List<Team> teams = currentGame.teams;

		zoneIdClicked = "";
	}

	public void ChangeGame(Game game){
		waitingScreen.SetActive (false);

		previousGame = currentGame;
		currentGame = game;
		DrawTerminals ();
		DrawZones ();

		leaderboardManager.SetCurrentGame (currentGame,currentXp);			
		uiManager.SetCurrentTerminals (previousGame,currentGame,zoneDict);

		uiManager.UpdateTokens (currentGame.GetToken (team));

		AddRankAchivements (game);
		CheckIfGotAllAchievement ();
	}

	private void CheckIfGotAllAchievement(){
		if (currentEffects.achievements.Count == 10 && !currentEffects.achievements.Contains (EffectsConstants.allAchievement)) {
			FirebaseManager.AddAllAchivement ();
		}
	}

	private void AddRankAchivements(Game game){
		bool addBest = false;
		for (int i = 0; i < game.bestUsers.Count; i++) {
			if (game.bestUsers[i].GetId () == FirebaseManager.user.UserId) {
				if (i == 0) {
					if(!currentEffects.achievements.Contains(EffectsConstants.bestPlayerAchievement)){
						FirebaseManager.AddBestPlayerAchivement ();
						addBest = true;
					}
				} else {
					if(!addBest && !currentEffects.achievements.Contains(EffectsConstants.top5PlayerAchievement)){
						FirebaseManager.AddTop5PlayerAchivement ();
					}
				}
			}
		}
	}

	public void DrawTerminalsUI(string zoneId) {
		zoneIdClicked = zoneId;
		foreach(KeyValuePair<string, Terminal> entry in terminalDict) {
			entry.Value.ShowUI (entry.Value.zoneId == zoneId);
		}
	}

	public void DrawTerminals(){
		List<Terminal> newTerminals =  currentGame.GetNewTerminals (previousGame);
		List<Terminal> deletedTerminals =  currentGame.GetDeletedTerminals (previousGame);
		List<Terminal> modifiedTerminals =  currentGame.GetModifiedTerminals (previousGame);
		for (int i = 0; i < newTerminals.Count; i++) {
			Terminal t = (Terminal)Instantiate (
				terminalPrefab,
				new Vector3(newTerminals[i].x,0.2f,newTerminals[i].z),
				new Quaternion (),
				sceneRoot.gameObject.transform);
			
			t.Copy (newTerminals [i]);
			t.gameObject.transform.localPosition = new Vector3(newTerminals [i].x,0.2f,newTerminals [i].z);
			t.SetTarget(GameObject.Find(newTerminals[i].zoneId+"_volume"));
			t.Init ();
			terminalDict.Add (t.GetTerminalId (), t);
			t.ShowUI (zoneIdClicked == t.zoneId); //If concurrence problem, comes from here (some terminal added, in the meantime the user clicked on the screen, and with bad luck the infos are not printed)
		}

		for (int i = 0; i < deletedTerminals.Count; i++) {
			if (terminalDict.ContainsKey (deletedTerminals [i].GetTerminalId ())) {
				Destroy (terminalDict[deletedTerminals [i].GetTerminalId()].gameObject);
				terminalDict.Remove (deletedTerminals [i].GetTerminalId ());
			}
		}

		for (int i = 0; i < modifiedTerminals.Count; i++) {
			if (terminalDict.ContainsKey (modifiedTerminals [i].GetTerminalId ())) {
				terminalDict [modifiedTerminals [i].GetTerminalId()].Copy (modifiedTerminals [i]);
				modifiedTerminals [i].ShowUI (zoneIdClicked == modifiedTerminals [i].zoneId);
			}
		}
	}

	public void DrawZones(){

		IList<Zone> zones = currentGame.GetZones ();
		foreach(Zone z in zones){
			//If the received zone id is valid, update the game with the value received from DB
			if (zoneDict.ContainsKey(z.zoneId)) {
				interactionManager.updateZoneInfo (z); //must be put before, otherwise the check of team modification can't be done in interaction manager
				zoneDict [z.zoneId].Copy (z);
			}
		}
	}


	public void UpdateUserStat(string xp, string credit, int team,string level,Effects effects,Statistics statistics,SkinsInfo skins){
		this.team = team;
		interactionManager.updateUserInfo (credit, level);
		uiManager.UpdateUserStat (xp, credit, team,level,effects,statistics,skins);
		currentEffects = effects;
		currentXp = xp;
	}

	public void AddTerminal(string zoneId,float x, float z){

		List<string> names = currentGame.GetTerminalsName ();
		int maxIndex = 0;
		for (int i = 0; i < names.Count; i++) {
			if(names[i][0].ToString() == FirebaseManager.userTeam.ToString()){
				int currentIndex = Int32.Parse(names[i].Remove(0,2));
				if (currentIndex > maxIndex) {
					maxIndex = currentIndex;
				}
			}
		}

		Terminal terminal = new Terminal (FirebaseManager.userTeam.ToString() + "-"+(maxIndex+1).ToString(),zoneId,0,30,100,FirebaseManager.userTeam,x,z);
		//uiManager.SetPopUpText (zoneId);
		FirebaseManager.AddTerminal (terminal, messagePopup);
	}

	public void AddTerminalAtPlayerPosition(){
		this.AddTerminal (player.GetCurrentZoneName (), player.GetMarkerPosition ().x, player.GetMarkerPosition ().y);
	}

	public bool IsPlayerInsideZone(){
		return player.isInsideAttackableZone ();
	}
		

	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		modeButton.GetComponentInChildren<Text>().text = "Switch to Attack";
	}

	override public void FirstLocationSent() {
		modeButton.GetComponentInChildren<Text>().text = "Switch to Defense";
	}

	public void addTransaction(){
		FirebaseManager.AddTerminalDamagedStat ();
	}

	void Update() {
		achievementFunction ();
	}

	public void setAchievement(string text) {

		achievementFunction = () => {
			achievement.gameObject.SetActive (true);
			achievement.setAchievement (text);
			achievementFunction = () => {
			};
		};

	}
}
