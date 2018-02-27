using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
	}


	// Update is called once per frame
	void TaskOnClick () {
		Persistency.Erase ();
		FirebaseManager.Logout ();
		SceneManager.LoadScene(0);
	}
}
