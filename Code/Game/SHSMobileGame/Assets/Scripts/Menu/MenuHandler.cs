using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour {

	public GameObject menuTab;
	public GameObject creditsTab;

	public void OpenMenuHandler() {
		menuTab.SetActive (true);
	}

	public void CloseMenuHandler() {
		menuTab.SetActive (false);
	}

	public void OpenCreditsHandler() {
		creditsTab.SetActive (true);
	}

	public void CloseCreditsHandler() {
		creditsTab.SetActive (false);
	}
}
