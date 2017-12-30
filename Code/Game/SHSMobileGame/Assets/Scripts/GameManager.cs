using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public UiManager uiManager;
	public GameObject sceneRoot;
	private Game game;

	void Awake(){
		FirebaseManager.SetMainGameRef (this);
	}

	// Use this for initialization
	void Start () {
		FirebaseManager.SetListenerCreditScore ();
		FirebaseManager.SetListenerGame ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChangeGame(Game game){
		this.game = game;
	}

	public void UpdateScoreAndCredit(string xp, string credit){
		uiManager.UpdateScoreAndCredit (xp, credit);
	}

	public void AddTerminal(string zoneId){
		int i = Random.Range(0,10000000);
		Terminal terminal = new Terminal (i.ToString(),zoneId,100,100,FirebaseManager.userTeam);
		uiManager.SetPopUpText (zoneId);
		FirebaseManager.AddTerminal (terminal);
	}
}
