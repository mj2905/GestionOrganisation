using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public UiManager uiManager;
	public GameObject sceneRoot;
	public Terminal terminalPrefab;
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
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 1000)) {
				if (hit.transform.gameObject.tag == "Terminal") {
					Terminal clickedTerminal = hit.transform.gameObject.GetComponentInParent<Terminal>();
					uiManager.SetPopUpText (clickedTerminal.zoneId.ToString());	
					clickedTerminal.takeDamage ();
					FirebaseManager.AddTerminal (clickedTerminal); 	
			} 
		}
	}
	}

	public void ChangeGame(Game game){
		previousGame = currentGame;
		currentGame = game;
		DrawTerminals ();
	}

	public void DrawTerminals(){
		List<Terminal> newTerminals =  currentGame.GetNewTerminals (previousGame);
		List<Terminal> deletedTerminals =  currentGame.GetDeletedTerminals (previousGame);
		/*List<Terminal> newTerminals =  new List<Terminal>();
		newTerminals.Add (new Terminal("1-0","RLC",1,1,1,0,0));
		newTerminals.Add (new Terminal("1-1","RLC",1,1,1,10,0));
		newTerminals.Add (new Terminal("1-2","RLC",1,1,1,0,10));
		newTerminals.Add (new Terminal("1-3","RLC",1,1,1,10,10));
		List<Terminal> deletedTerminals =  new List<Terminal>();*/

		for (int i = 0; i < newTerminals.Count; i++) {
			Terminal t = (Terminal)Instantiate (
				terminalPrefab,
				new Vector3(newTerminals[i].x,2,newTerminals[i].z),
				new Quaternion (),
				sceneRoot.gameObject.transform);
			
			t.name = newTerminals [i].GetTerminalId ();
			t.team = newTerminals [i].team;
			t.zoneId = newTerminals [i].zoneId;
			t.hp = newTerminals [i].hp;
			t.SetTerminalId (newTerminals [i].GetTerminalId ());

			t.gameObject.transform.localPosition = new Vector3(newTerminals [i].x,2,newTerminals [i].z);

			t.SetTarget(GameObject.Find("SceneRoot/Zones/"+newTerminals[i].zoneId+"_building/"+newTerminals[i].zoneId));
			t.Init ();
		}

		for (int i = 0; i < deletedTerminals.Count; i++) {
			Destroy(GameObject.Find("SceneRoot/"+deletedTerminals[i].GetTerminalId()));
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

		Terminal terminal = new Terminal (FirebaseManager.userTeam.ToString() + "-"+(maxIndex+1).ToString(),zoneId,100,100,FirebaseManager.userTeam,x,z);
		//uiManager.SetPopUpText (zoneId);
		FirebaseManager.AddTerminal (terminal);
	}
}
