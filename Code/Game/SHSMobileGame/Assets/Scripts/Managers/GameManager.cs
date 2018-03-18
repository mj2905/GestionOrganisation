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

	private Game previousGame;
	private Game currentGame;
	private string zoneIdClicked;

	public PopupScript messagePopup;

	void Awake(){
		FirebaseManager.SetMainGameRef (this);
	}

	// Use this for initialization
	void Start () {
		FirebaseManager.SetListenerCreditScore ();
		FirebaseManager.SetListenerGame ();

		previousGame = new Game ();
		currentGame = new Game ();

		for (int i = 0; i < zones.Length; i++) {
			zoneDict[zones [i].zoneId] = zones [i];
		}

		zoneIdClicked = "";
	}

	public void ChangeGame(Game game){
		previousGame = currentGame;
		currentGame = game;
		DrawTerminals ();
		DrawZones ();
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
				new Vector3(newTerminals[i].x,2,newTerminals[i].z),
				new Quaternion (),
				sceneRoot.gameObject.transform);
			
			t.Copy (newTerminals [i]);
			t.gameObject.transform.localPosition = new Vector3(newTerminals [i].x,2,newTerminals [i].z);
			t.SetTarget(GameObject.Find("SceneRoot/Zones/"+newTerminals[i].zoneId +"/"+newTerminals[i].zoneId+"_building/" + newTerminals[i].zoneId+"_volume"));
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

	public void UpdateUserStat(string xp, string credit,string level,Effects effects,Statistics statistics){
		interactionManager.updateCreditsInfo (credit);
		uiManager.UpdateUserStat (xp, credit,level,effects,statistics);
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

		Terminal terminal = new Terminal (FirebaseManager.userTeam.ToString() + "-"+(maxIndex+1).ToString(),zoneId,0,100,100,FirebaseManager.userTeam,x,z);
		//uiManager.SetPopUpText (zoneId);
		FirebaseManager.AddTerminal (terminal, messagePopup);
	}

	public void AddTerminalAtPlayerPosition(){
		this.AddTerminal (player.GetCurrentZoneName (), player.GetMarkerPosition ().x, player.GetMarkerPosition ().y);
	}

	public bool IsPlayerInsideZone(){
		return player.isInsideZone ();
	}
		

	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		modeButton.GetComponentInChildren<Text>().text = "Defense mode";
	}

	override public void FirstLocationSent() {
		modeButton.GetComponentInChildren<Text>().text = "Attack mode";
	}

	public void addTransaction(){
		FirebaseManager.AddTerminalDamagedStat ();
	}
}
