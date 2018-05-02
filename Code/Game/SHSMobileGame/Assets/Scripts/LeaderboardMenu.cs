using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LeaderboardMenu : MonoBehaviour {

	public GameObject teamLeaderboard;
	public GameObject userLeaderboard;

	private List<RectTransform> teamEntries;
	private List<Text> pointsText = new List<Text>();
	private List<float> positionTeamLeaderboard = new List<float>();

	private List<RectTransform> usersEntries;
	private List<Tuple<Text,Text>> usersText = new List<Tuple<Text,Text>>();
	private List<float> positionUsers = new List<float>();

	private GameObject lastUser;
	private Tuple<Text,Text> pointTextLastUser;

	private List<int> scores = new List<int>(){0,0,0,0};
	private List<User> bestUsers = new List<User> ();
	private List<float> newPosition  = new List<float>();

	private enum state{TEAM,USER}
	private state currentState = state.USER;

	private string currentXp;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < teamEntries.Count; i++) {
			teamEntries[i].transform.Find ("Team").GetComponent<Text> ().color = ColorConstants.getTextColor (i+1);
			teamEntries[i].transform.Find ("Team").GetComponent<Text> ().text = ColorConstants.getTeamName (i+1);
			teamEntries[i].transform.Find ("Points").GetComponent<Text> ().color = ColorConstants.getTextColor (i+1);
		}
	}

	public void SetLastUser(GameObject lastUser, Tuple<Text,Text> pointTextLastUser){
		this.lastUser = lastUser;
		this.pointTextLastUser = pointTextLastUser;
	}

	public void SetPositionAndPoints(List<Text> pointsText,List<float> positionTeamLeaderboard,List<RectTransform> teamEntries){
		this.pointsText = pointsText;
		this.positionTeamLeaderboard = positionTeamLeaderboard;
		this.teamEntries = teamEntries;
	}

	public void SetPositionAndBestUsers(List<Tuple<Text,Text>> usersText,List<float> positionUsers,List<RectTransform> usersEntries){
		this.usersText = usersText;
		this.positionUsers = positionUsers;
		this.usersEntries = usersEntries;
	}

	public void SetScores(List<int> scores){
		this.scores = scores;
		UpdateEntries();
		UpdatePoints ();
	}

	public void SetBestUsers(List<User> bestUsers,string xp){
		this.bestUsers = bestUsers;
		this.currentXp = xp;
		UpdateBestUsers ();
	}

	public void Update(){
		if (teamLeaderboard.activeSelf) {
			for (int i = 0; i < teamEntries.Count; i++) {
				teamEntries [i].position = new Vector3 (teamEntries [i].position.x, Mathf.MoveTowards (teamEntries [i].position.y, newPosition [i], 25), teamEntries [i].position.z);
			}
		}
		if (userLeaderboard.activeSelf) {
			
		}
	}

	private void UpdateBestUsers(){
		bool isUserTopPlayer = false;

		for (int i = 0; i < Mathf.Min(usersText.Count,bestUsers.Count); i++) {
			if (bestUsers [i].GetId () == FirebaseManager.user.UserId) {
				isUserTopPlayer = true;
				usersText [i].Item1.text = bestUsers [i].pseudo + " (YOU)";
			} else {
				usersText [i].Item1.text = bestUsers [i].pseudo;
			}
			usersText [i].Item1.color = ColorConstants.getTextColor(bestUsers [i].team);
			usersText [i].Item2.text = bestUsers [i].xp.ToString();
			usersText [i].Item2.color = ColorConstants.getTextColor(bestUsers [i].team);
		}

		if (isUserTopPlayer) {
			lastUser.SetActive (false);
		} else {
			lastUser.SetActive (true);
			pointTextLastUser.Item1.text = FirebaseManager.userPseudo + " (YOU)";
			pointTextLastUser.Item1.color = ColorConstants.getTextColor(FirebaseManager.userTeam);
			pointTextLastUser.Item2.text = currentXp;
			pointTextLastUser.Item2.color = ColorConstants.getTextColor(FirebaseManager.userTeam);
		}
	}

	private void UpdatePoints(){
		for (int i = 0; i < pointsText.Count; i++) {
			pointsText [i].text = scores [i].ToString ();
		}
	}

	private void UpdateEntries(){
		newPosition = new List<float> ();

		var sorted = scores.Select((x, i) => new KeyValuePair<int, int>(x, i))
			.OrderByDescending(x => x.Key)
			.ToList();
		List<int> idx = sorted.Select(x => x.Value).ToList();

		for (int i = 0; i < scores.Count; i++) {
			newPosition.Add(positionTeamLeaderboard[idx.IndexOf(i)]);
		}
	}


	public void OpenUserTab(){
		ChangeState (state.USER);
	}

	public void OpenTeamTab(){
		ChangeState (state.TEAM);
	}

	private void ChangeState(state state){
		currentState = state;
		switch(currentState){
		case state.TEAM:
			userLeaderboard.SetActive (false);
			teamLeaderboard.SetActive (true);
			break;
		case state.USER:
			userLeaderboard.SetActive (true);
			teamLeaderboard.SetActive (false);
			break;
		}
	}
}



