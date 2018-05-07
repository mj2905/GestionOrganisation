using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour {

	public void TaskOnClick () {
		Persistency.Erase ();
		FirebaseManager.Logout ();
		SceneManager.LoadScene(0);
	}

	public void PlayTutorial() {
		FirebaseManager.Logout ();
		SceneManager.LoadScene("TutorialScene");
	}
}
