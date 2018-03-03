using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHandler : MonoBehaviour {

	private const int ATTACK = 1;
	private const int DEFENSE = 0;
	private const string USERPREF_ATTACK_KEY = "attackMode";

	public PopupScript popup;

	private double latitude = 0;
	private double longitude = 0;
	private bool firstLocation = false;
	private bool started = false;

	public LocationListener[] listeners;
	private bool locationWasEnabled = false; //by app, when someone clicks on attack button for example
	//invariant : if locationWasEnabled = false, location is stopped

	private void BeginLocation() {
		if (locationWasEnabled && !started) {
			Input.location.Start (5.0f, 5.0f);
			firstLocation = true;
			started = true;
		}
	}

	private void StopLocation() {
		if (locationWasEnabled) {
			Input.location.Stop ();
			started = false;
			latitude = 0;
			longitude = 0;
		}
	}

	void OnApplicationFocus(bool focusStatus) {
		if (locationWasEnabled) {
			if (!focusStatus) {
				print ("Stopped");
				StopLocation ();
			} else {
				print ("Restart");
				BeginLocation ();
			}
		}
	}

	void OnDisable() {
		StopLocation ();
	}

	// Use this for initialization
	void Start () {
		locationWasEnabled = PlayerPrefs.GetInt (USERPREF_ATTACK_KEY, DEFENSE) == ATTACK;
		ActivateLocationIfPossible (locationWasEnabled); //should not run location at first, will be useful when app has quit
	}

	public void SwitchMode() {
		if (started) {
			DeactivateLocation ();
		} else {
			ActivateLocationIfPossible ();
		}
	}

	public void ActivateLocationIfPossible(bool additionalCondition = true) {
		locationWasEnabled = additionalCondition && (Input.location.status != LocationServiceStatus.Failed); //Only thing that can block it is if it's not allowed to be used
		PlayerPrefs.SetInt (USERPREF_ATTACK_KEY, locationWasEnabled ? ATTACK : DEFENSE);
		BeginLocation ();
	}

	public void DeactivateLocation() {
		StopLocation ();
		locationWasEnabled = false;
		PlayerPrefs.SetInt (USERPREF_ATTACK_KEY, DEFENSE);
		foreach (LocationListener listener in listeners) {
			listener.StopLocationHandling ();
		}
	}

	private void HandleCoordinates() {

		double lastLong = longitude;
		double lastLat = latitude;

		longitude = Input.location.lastData.longitude;
		latitude = Input.location.lastData.latitude;


		if (longitude != lastLong || latitude != lastLat) {
			foreach (LocationListener listener in listeners) {
				listener.CoordinateUpdate (latitude, longitude);

				if (firstLocation) {
					listener.FirstLocationSent ();
				}
			}
			firstLocation = false;
		}
	}

	// Update is called once per frame
	void Update () {

		if (Input.location.status == LocationServiceStatus.Failed) {
			if (locationWasEnabled) {
				DeactivateLocation ();
			}
			popup.SetText ("You have to enable the geolocation to be able to be in attack mode. To do so, go to Settings, SHSGameTest and choose to allow location access.");
		}

		if (locationWasEnabled && Input.location.status == LocationServiceStatus.Running) {
			HandleCoordinates ();
		}
	}
}

public abstract class LocationListener : MonoBehaviour {
	abstract public void CoordinateUpdate(double latitude, double longitude);
	abstract public void StopLocationHandling();
	abstract public void FirstLocationSent();
}