using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TitleText : MonoBehaviour {

	private bool hasWon = false;
	private int team;
	public bool TeamOrStatus = true; //team = true, status = false
	public GameObject win;
	public GameObject lose;

	private Action performParticleChoice;

	private void getTeamAndScore() {
		List<string> list = new List<string> (EndGameValues.SCORES.Keys);
		list.Sort();
		team = FirebaseManager.userTeam;
		hasWon = (int)EndGameValues.SCORES [list [list.Count - 1]] == team;
	}

	private void PutTitle() {
		getTeamAndScore ();


		if (TeamOrStatus) {
			string text = ColorConstants.getTeamName ((ColorConstants.TEAMS)team);
			if (text.Length < 4) {
				text = text + " ";
			}

			GetComponent<Text> ().text = text;
			GetComponent<Text> ().color = ColorConstants.getTextColor (team);
		} else {
			string text;
			if (hasWon) {
				text = "WON";
			} else {
				text = "LOST";
			}
			GetComponent<Text> ().text = text;
		}

	}

	void Start() {
		Persistency.Erase ();
		PutTitle ();

		if (win != null && lose != null) {
			performParticleChoice = () => {
				if(hasWon) {
					win.SetActive(true);
					lose.SetActive(false);
				} else {
					win.SetActive(false);
					lose.SetActive(true);
				}
			};
		} else {
			performParticleChoice = () => {
			};
		}
	}
	
	// Update is called once per frame
	void Update () {
		PutTitle ();
		performParticleChoice ();
	}
}
