using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LeaderBoardManager : MonoBehaviour {

	//generate a map that goes from a team to a score

	GameManager gameManager;
	//Game game;

	static Dictionary<string, Dictionary<string,float>> teamScores;

	//we want to able to do teamScores["team"]["attacked"]
	// or teamScores["team"]["defense"]
	// or teamScores["team"]["percentage"]

	static int changeCounter = 0;

	//user, scoreType --> ,
	void Start(){

		//teamScores = new Dictionary<string, Dictionary<string, int>> ();

		gameManager = GameObject.FindObjectOfType<GameManager> ();


		if (gameManager == null) {

			Debug.LogError ("didn't load GameManager");

			return;
		}

		Debug.Log ("all good");


//				SetScore ("RED", "Points", 1);
//				SetScore ("BLUE", "Points", 3);
//				SetScore ("GREEN", "Points",2);
//				SetScore ("YELLOW", "Points", 2);


	}
	void update(){

	}

	void Init(){
		if (teamScores != null) {
			return;
		}
		teamScores = new Dictionary<string, Dictionary<string, float>> ();
	}



	//how does outside world interact with the program

	public float GetScore(string team, string scoreType){
		Init ();

		// we check if the team exists
		if (!teamScores.ContainsKey (team)) {
			return 0;
		}
		// we check if the score type exist
		if (!teamScores[team].ContainsKey (scoreType)) {
			return 0;
		}

		return teamScores [team] [scoreType];

	}

	public void SetScore(string team, string scoreType, float value){

		Init ();

		changeCounter++;
		if (!teamScores.ContainsKey (team)) {
			teamScores [team] = new Dictionary<string, float> ();
		}
		teamScores [team] [scoreType] = value;

	}

	public void ChangeScore(string team , string scoreType, float amount) {
		Init ();

		float currScore = GetScore (team, scoreType);

		SetScore (team, scoreType, currScore + amount);

	}

	public string[] GetTeamNames(){
		Init ();
		return teamScores.Keys.ToArray();

	}

	public string[] GetTeamNames(string sortingScoreType){
		Init ();

		return teamScores.Keys.OrderByDescending (n => GetScore (n, sortingScoreType)).ToArray();
	}

	public void debug_add_kill(){

		ChangeScore ("RED", "Points", 1);

	}

	public int GetChangeCounter(){
		return changeCounter;
	}
}
