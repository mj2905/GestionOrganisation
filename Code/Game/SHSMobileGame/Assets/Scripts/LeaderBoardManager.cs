using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LeaderBoardManager : MonoBehaviour {

	private Game currentGame = new Game();
	public LeaderboardButton button;
	public LeaderBoardScript LeaderBoardScript;

	void Start(){
			
	}

	public void SetCurrentGame(Game currentGame){
		this.currentGame = currentGame;
		button.SetScores (currentGame.GetScores ());
	}

	public void ChangeScore(string team , string scoreType, float amount) {
		
	}
	/*
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
	}*/
}
