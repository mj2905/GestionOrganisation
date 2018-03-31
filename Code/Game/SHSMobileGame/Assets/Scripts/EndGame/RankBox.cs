using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using System.Collections.Generic;
using UnityEngine.UI;

public class RankBox : MonoBehaviour {

	public Text teamText;
	public Text scoreText;
	public int rankingPosition = 1;
	public float speed = 1.0f;
	private float speed_const = 400.0f;
	private Action update;

	private ColorConstants.TEAMS team = ColorConstants.TEAMS.ENAC;
	private int score = 0;

	private void getTeamAndScore() {
		List<int> list = new List<int> (EndGameValues.SCORES.Keys);
		list.Sort();
		list.Reverse ();
		score = list [rankingPosition - 1];
		team = EndGameValues.SCORES [score];
	}

	void Start () {

		if (transform.localPosition.x < 0) {
			update = () => {
				float delta = Time.deltaTime;
				Vector3 pos = transform.localPosition;
				pos.x = Math.Min(pos.x + delta * speed_const * speed, 0);
				transform.localPosition = pos;
			};
		} else {
			update = () => {
				float delta = Time.deltaTime;
				Vector3 pos = transform.localPosition;
				pos.x = Math.Max(pos.x - delta * speed_const * speed, 0);
				transform.localPosition = pos;
			};
		}

		PutTextAndMove ();
	}

	private void PutTextAndMove() {
		if (transform.localPosition.x != 0) {
			update ();
		}

		getTeamAndScore ();
		teamText.text = ColorConstants.getTeamName(team);
		scoreText.text = ""+score;
		Color color = ColorConstants.getColor ((int)team);
		color.a = 0.9f;
		GetComponent<Image> ().color = color;
	}

	void Update () {
		PutTextAndMove ();
	}
}
