using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour {

	public GameObject menuTab;

	public void OpenMenuHandler() {
		menuTab.SetActive (true);
	}

	public void CloseMenuHandler() {
		menuTab.SetActive (false);
	}
}
