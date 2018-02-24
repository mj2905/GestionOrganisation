using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLocationGet : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		if (!Input.location.isEnabledByUser) {
			print ("No Location");
			yield break;
		}

		Input.location.Start ();

		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds (1);
			maxWait--;
		}

		if (maxWait <= 0) {
			print ("Timed out");
			yield break;
		}

		if (Input.location.status == LocationServiceStatus.Failed) {
			print ("Unable to determine device location");
			yield break;
		} else {
			print ("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude +
			" " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy +
			" " + Input.location.lastData.timestamp);
		}

		Input.location.Stop ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
