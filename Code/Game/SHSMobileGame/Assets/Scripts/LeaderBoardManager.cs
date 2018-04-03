using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LeaderBoardManager : MonoBehaviour {

	private Game currentGame = new Game();
	public LeaderboardButton button;
	public LeaderboardMenu menu;
	public GameObject leaderBoardPanel;
	public List<RectTransform> teamEntries;
	public List<RectTransform> usersEntries;

	void Start(){
		List<float> positionTeamLeaderboard = new List<float>();
		List<Text> pointsText = new List<Text>();

		List<float> positionUsers = new List<float>();
		List<Tuple<Text,Text>> usersText = new List<Tuple<Text,Text>>();

		for (int i = 0; i < teamEntries.Count; i++) {
			positionTeamLeaderboard.Add(teamEntries[i].position.y);
			pointsText.Add (teamEntries [i].transform.Find ("Points").GetComponent<Text> ());
		}

		for (int i = 0; i < usersEntries.Count; i++) {
			positionUsers.Add(usersEntries[i].position.y);
			usersText.Add (new Tuple<Text,Text>(
				usersEntries [i].transform.Find ("Team").GetComponent<Text> (),
				usersEntries [i].transform.Find ("Points").GetComponent<Text> ())
			);
		}

		menu.SetPositionAndPoints (pointsText, positionTeamLeaderboard,teamEntries);
		menu.SetPositionAndBestUsers (usersText, positionUsers,usersEntries);
	}

	public void SetCurrentGame(Game currentGame){
		this.currentGame = currentGame;
		button.SetScores (currentGame.GetScores ());
		menu.SetScores (currentGame.GetScores ());
		menu.SetBestUsers (currentGame.bestUsers);
	}

	public bool isActive(){
		return leaderBoardPanel.gameObject.activeSelf;
	}

	public List<int> GetScore(){
		return currentGame.GetScores ();
	}

	public void ToggleMenu(){
		leaderBoardPanel.gameObject.SetActive (!leaderBoardPanel.gameObject.activeSelf);
	}
}
