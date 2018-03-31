using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamScoreList : MonoBehaviour {

	public GameObject playerScoreEntryPrefab;

	LeaderBoardManager leaderBoardManager;

	int lastChangeCounter;

	bool firstShow = true;



	// Use this for initialization
	void Start () {

		leaderBoardManager = GameObject.FindObjectOfType<LeaderBoardManager> ();
		lastChangeCounter = leaderBoardManager.GetChangeCounter ();

	}

	// Update is called once per frame
	void Update () {

		if (leaderBoardManager == null) {

			Debug.LogError ("didn't load leader manager");

			return;
		}

		if (!firstShow && leaderBoardManager.GetChangeCounter() == lastChangeCounter) {
			//no change;

			firstShow = false;
			return;
		}



		while (this.transform.childCount > 0) {
			Transform c = this.transform.GetChild (0);

			c.SetParent (null);
			Destroy (c.gameObject);
		}

		string[] names = leaderBoardManager.GetTeamNames ("Points");

		foreach (string name in names) {
			GameObject go = (GameObject)Instantiate (playerScoreEntryPrefab);

			go.transform.SetParent (this.transform, false);
			Text team = go.transform.Find ("Team").GetComponent<Text> ();
			team.text = name;


			Text points = go.transform.Find ("Points").GetComponent<Text> ();
			points.text = leaderBoardManager.GetScore(name,"Points").ToString();

			Color c = getColor (name);

			team.color = c; 
			points.color = c;
		}

	}

	public Color getColor(string name){


		Color color;
		switch (name) {
		case "ENAC":
			color = ColorConstants.textColorTeam1;
			break;
		case "STI":
			color = ColorConstants.textColorTeam2;
			break;
		case "FSB":
			color = ColorConstants.textColorTeam3;
			break;
		case "IC&SV":
			color = ColorConstants.textColorTeam4;
			break;
		default:
			color = ColorConstants.textColorTeam1;
			break;
		}
		return color;
	}
	}



