using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LeaderboardMenu : MonoBehaviour {

	public LeaderBoardManager manager;

	private List<RectTransform> teamEntries;
	private List<Text> pointsText = new List<Text>();
	private List<float> positionTeamLeaderboard = new List<float>();

	private List<RectTransform> usersEntries;
	private List<Tuple<Text,Text>> usersText = new List<Tuple<Text,Text>>();
	private List<float> positionUsers = new List<float>();

	private List<int> scores = new List<int>(){0,0,0,0};
	private List<User> bestUsers = new List<User> ();
	private List<float> newPosition  = new List<float>();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < teamEntries.Count; i++) {
			teamEntries[i].transform.Find ("Team").GetComponent<Text> ().color = ColorConstants.getTextColor (i+1);
			teamEntries[i].transform.Find ("Team").GetComponent<Text> ().text = ColorConstants.getTeamName (i+1);
			teamEntries[i].transform.Find ("Points").GetComponent<Text> ().color = ColorConstants.getTextColor (i+1);
		}
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

	public void SetBestUsers(List<User> bestUsers){
		this.bestUsers = bestUsers;
		UpdateBestUsers ();
	}

	public void Update(){
		if (manager.isActive() && false) {
			for (int i = 0; i < teamEntries.Count; i++) {
				teamEntries [i].position = new Vector3 (teamEntries [i].position.x, Mathf.MoveTowards (teamEntries [i].position.y, newPosition [i], 25), teamEntries [i].position.z);
			}
		}
	}

	private void UpdateBestUsers(){
		for (int i = 0; i < usersText.Count; i++) {
			usersText [i].Item1.text = bestUsers [i].pseudo;
			usersText [i].Item2.text = bestUsers [i].xp.ToString();
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
}



