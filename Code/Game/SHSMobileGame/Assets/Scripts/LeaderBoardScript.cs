using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardScript : MonoBehaviour {

	public GameObject leaderBoard;


	public void Button_click(){
		leaderBoard.SetActive (!leaderBoard.activeSelf);
	}



	void Start(){
		leaderBoard.SetActive (false);
	}

}
