using System.Collections;
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

	private List<float> position = new List<float>();
	private List<Text> points = new List<Text>();

	void Start(){
		for (int i = 0; i < teamEntries.Count; i++) {
			position.Add(teamEntries[i].position.y);
			points.Add (teamEntries [i].transform.Find ("Points").GetComponent<Text> ());
		}
		menu.SetPositionAndPoints (points, position);
	}

	public void SetCurrentGame(Game currentGame){
		this.currentGame = currentGame;
		button.SetScores (currentGame.GetScores ());
		menu.SetScores (currentGame.GetScores ());
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
