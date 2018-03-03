using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : LocationListener {

	public UiManager uiManager;
	public GameObject sceneRoot;
	public GameObject zonesRoot;
	public Terminal terminalPrefab;
	public PlayerController player;
	public Button modeButton;
	public Zone[] zones;
	private Dictionary<string, Zone> zoneDict = new Dictionary<string, Zone>();

	private Game previousGame;
	private Game currentGame;

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
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 1000)) {
				if (hit.transform.gameObject.tag == "Terminal") {
					Terminal clickedTerminal = hit.transform.gameObject.GetComponentInParent<Terminal>();
					FirebaseManager.HurtTerminal (clickedTerminal.GetTerminalId(), 20);
				}
			}
		}
	}

	public void ChangeGame(Game game){
		previousGame = currentGame;
		currentGame = game;
		DrawTerminals ();
		DrawZones ();
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
		}

		for (int i = 0; i < deletedTerminals.Count; i++) {
			Destroy(GameObject.Find("SceneRoot/"+deletedTerminals[i].GetTerminalId()));
		}

		for (int i = 0; i < modifiedTerminals.Count; i++) {
			GameObject.Find ("SceneRoot/" + modifiedTerminals [i].GetTerminalId ()).GetComponent<Terminal> ().Copy (modifiedTerminals [i]);
		}
	}

	public void DrawZones(){

		IList<Zone> zones = currentGame.GetZones ();

		foreach(Zone z in zones){

			//If the received zone id is valid, update the game with the value received from DB
			if (zoneDict.ContainsKey(z.zoneId)) {
				z.copyInto (zoneDict [z.zoneId]);
			}

		}

	}

	public void UpdateScoreAndCredit(string xp, string credit){
		uiManager.UpdateScoreAndCredit (xp, credit);
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

		Terminal terminal = new Terminal (FirebaseManager.userTeam.ToString() + "-"+(maxIndex+1).ToString(),zoneId,3,100,100,FirebaseManager.userTeam,x,z);
		//uiManager.SetPopUpText (zoneId);
		FirebaseManager.AddTerminal (terminal);
	}

	public void AddTerminalAtPlayerPosition(){
		this.AddTerminal (player.GetCurrentZoneName (), player.GetMarkerPosition ().x, player.GetMarkerPosition ().y);
	}

	public bool IsPlayerInsideZone(){
		return player.isInsideZone ();
	}
		

	override public void CoordinateUpdate(MapCoordinate coords) {}

	override public void StopLocationHandling() {
		modeButton.GetComponentInChildren<Text>().text = "Defense mode";
	}

	override public void FirstLocationSent() {
		modeButton.GetComponentInChildren<Text>().text = "Attack mode";
	}
}
