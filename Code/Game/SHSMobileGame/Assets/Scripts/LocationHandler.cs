using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHandler : MonoBehaviour {

	private const int ATTACK = 1;
	private const int DEFENSE = 0;
	private const string USERPREF_ATTACK_KEY = "attackMode";

	public PopupScript popup;

	private MapCoordinate coords = MapCoordinate.ZERO();

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
			coords = MapCoordinate.ZERO();
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

		MapCoordinate oldCoords = coords;
		coords = new MapCoordinate (Input.location.lastData.longitude, Input.location.lastData.latitude);

		if(!CoordinateConstants.DEBUG && (coords > CoordinateConstants.EPFL_TOP_RIGHT_MAP || coords < CoordinateConstants.EPFL_BOT_LEFT_MAP)) {

			DeactivateLocation ();
			popup.SetText ("To be in attack mode, you have to be on the EPFL campus");

		} else if (coords != oldCoords) {
			foreach (LocationListener listener in listeners) {
				listener.CoordinateUpdate (coords);

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
			popup.SetText ("You have to enable the geolocation to be able to be in attack mode. To do so, go to Settings, SHSGameTest and choose to allow location access");
		}

		if (locationWasEnabled && Input.location.status == LocationServiceStatus.Running) {
			HandleCoordinates ();
		}
	}
}

public abstract class LocationListener : MonoBehaviour {
	abstract public void CoordinateUpdate(MapCoordinate coords);
	abstract public void StopLocationHandling();
	abstract public void FirstLocationSent();
}