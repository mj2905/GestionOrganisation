using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardScript : MonoBehaviour {

	public GameObject leaderBoard;

	bool active = false;


	public void Button_click(){

		//UnityEngine.SceneManagement.SceneManager.LoadScene ("InitialScene");

		//Debug.Log ("hello");

		//Debug.Log (leaderBoard.activeSelf);
		leaderBoard.SetActive (!leaderBoard.activeSelf);


	}



	void Start(){

		leaderBoard = GameObject.Find ("LeaderBoardPanel");


		leaderBoard.SetActive (!leaderBoard.activeSelf);

		//baseLeaderBoard = GameObject.Find ("ZoneBoardPanel");
		//baseLeaderBoard.SetActive (!baseLeaderBoard);

	}

	void Update(){


	}



}
