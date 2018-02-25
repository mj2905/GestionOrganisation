using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonState : MonoBehaviour {

	public CameraController camera;
	public GameObject player;
	public Text text;
	private bool attackMode = false;

	private void ActionGivenMode() {
		if (attackMode) {
			player.GetComponent<Renderer>().enabled = true;
			player.GetComponent<PlayerController> ().BeginLocation ();
			text.GetComponent<Text>().text = "Attack mode";
		} else {
			player.GetComponent<Renderer>().enabled = false;
			player.GetComponent<PlayerController> ().StopLocation ();
			text.GetComponent<Text>().text = "Defense mode";
		}
	}

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
		ActionGivenMode ();
	}
	
	// Update is called once per frame
	void TaskOnClick () {
		camera.CAMERA_FIXED = !camera.CAMERA_FIXED;
		attackMode = !attackMode;

		ActionGivenMode ();
	}
}
