using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LeaderboardMenu : MonoBehaviour {

	public List<RectTransform> teamEntries;
	public LeaderBoardManager manager;

	private List<Text> points = new List<Text>();
	private List<int> scores = new List<int>(){0,0,0,0};
	private List<float> position = new List<float>();
	private List<float> newPosition  = new List<float>();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < teamEntries.Count; i++) {
			teamEntries[i].transform.Find ("Team").GetComponent<Text> ().color = ColorConstants.getTextColor (i+1);
			teamEntries[i].transform.Find ("Team").GetComponent<Text> ().text = ColorConstants.getTeamName (i+1);
			teamEntries[i].transform.Find ("Points").GetComponent<Text> ().color = ColorConstants.getTextColor (i+1);
		}
	}

	public void SetPositionAndPoints(List<Text> points,List<float> position){
		this.points = points;
		this.position = position;
	}

	public void SetScores(List<int> scores){
		Debug.Log ("setscore");
		this.scores = scores;

		UpdateEntries();
		UpdatePoints ();
	}

	public void Update(){
		if (manager.isActive()) {
			for (int i = 0; i < teamEntries.Count; i++) {
				teamEntries [i].position = new Vector3 (teamEntries [i].position.x, Mathf.MoveTowards (teamEntries [i].position.y, newPosition [i], 10), teamEntries [i].position.z);
			}
		}
	}

	private void UpdatePoints(){
		for (int i = 0; i < points.Count; i++) {
			points [i].text = scores [i].ToString ();
		}
	}

	private void UpdateEntries(){
		newPosition = new List<float> ();

		var sorted = scores.Select((x, i) => new KeyValuePair<int, int>(x, i))
			.OrderByDescending(x => x.Key)
			.ToList();
		List<int> idx = sorted.Select(x => x.Value).ToList();

		for (int i = 0; i < scores.Count; i++) {
			newPosition.Add(position[idx.IndexOf(i)]);
		}
	}
}



