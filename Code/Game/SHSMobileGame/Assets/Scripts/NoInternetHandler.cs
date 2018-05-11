using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoInternetHandler : MonoBehaviour {

	public GameObject panel;
	
	// Update is called once per frame
	void Update () {
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			panel.SetActive(true);
		} else {
			panel.SetActive (false);
		}
	}
}
