﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public UiManager uiManager;
	public GameObject sceneRoot;
	public GameObject zonesRoot;
	public Terminal terminalPrefab;
	public PlayerController player;
	public Text textMode;

	private Game previousGame;
	private Game currentGame;
	private bool attackMode = false;
	private CameraController camera;

	void Awake(){
		FirebaseManager.SetMainGameRef (this);
		camera = Camera.main.GetComponent<CameraController> ();
	}

	// Use this for initialization
	void Start () {
		FirebaseManager.SetListenerCreditScore ();
		FirebaseManager.SetListenerGame ();

		previousGame = new Game ();
		currentGame = new Game ();

		ActionGivenMode ();
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

	public bool IsAttackMode(){
		return attackMode;
	}

	public void SwitchMode(){
		camera.SwitchMode ();
		attackMode = !attackMode;
		ActionGivenMode ();
	}
		
	private void ActionGivenMode() {
		if (attackMode) {
			player.GetComponent<Renderer>().enabled = true;
			player.GetComponent<PlayerController> ().BeginLocation ();
			textMode.GetComponent<Text>().text = "Attack mode";
		} else {
			player.GetComponent<Renderer>().enabled = false;
			player.GetComponent<PlayerController> ().StopLocation ();
			textMode.GetComponent<Text>().text = "Defense mode";
		}
	}
}
