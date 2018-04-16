using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LocationHandler : LocationListener {

	private const int ATTACK = 1;
	private const int DEFENSE = 0;
	private const string USERPREF_ATTACK_KEY = "attackMode";

	public PopupScript popup;
	public FadingPlayer fadingPlayer;
	public Text locationTextTmp;

	private MapCoordinate coords = MapCoordinate.ZERO();

	private bool started = false;
	private bool isAttackMode = false; 

	public LocationSmoother locationSmoother;
	public LocationSmoother locationSmootherFade;
	private Collider[] colliders = new Collider[10];

	private bool locationWasEnabled = false; //by app, when someone clicks on attack button for example
	//invariant : if locationWasEnabled = false, location is stopped

	private void BeginLocation() {
		if (locationWasEnabled && !started) {
			Input.location.Start (1.0f, 0);
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
		isAttackMode = false;
	}

	public void SwitchMode() {
		print ("Switch");
		if (started) {
			DeactivateLocation ();
			locationSmootherFade.StopLocationHandling ();
		} else {
			ActivateLocationIfPossible ();
		}
	}

	public void ActivateLocationIfPossible(bool additionalCondition = true, bool write = true) {
		locationWasEnabled = additionalCondition && (Input.location.status != LocationServiceStatus.Failed); //Only thing that can block it is if it's not allowed to be used
		if(write) {PlayerPrefs.SetInt (USERPREF_ATTACK_KEY, locationWasEnabled ? ATTACK : DEFENSE);}
		BeginLocation ();
	}

	public void DeactivateLocation() {
		StopLocation ();
		locationWasEnabled = false;
		PlayerPrefs.SetInt (USERPREF_ATTACK_KEY, DEFENSE);

		locationSmoother.StopLocationHandling ();
	}

	private MapCoordinate GetCoordinateTest() {
		float time = Mathf.Repeat(Time.time, 10);
		if (time < 5.0f) {
			return CoordinateConstants.ROLEX_MAP;
		} else {
			return CoordinateConstants.IN_MAP;
		}
	}

	private void HandleCoordinates() {

		MapCoordinate oldCoords = coords;
		coords = CoordinateConstants.DEBUG == CoordinateConstants.DEBUG_STATE.WALKING_PATH ? 
			CoordinateConstants.WALKING_PATH.next () :
			new MapCoordinate (Input.location.lastData.longitude, Input.location.lastData.latitude);//GetCoordinateTest ();

		locationTextTmp.text = string.Format("lon:{0} / lat:{1}", coords.Item1, coords.Item2);


		if (CoordinateConstants.DEBUG == CoordinateConstants.DEBUG_STATE.NO_DEBUG && (coords > CoordinateConstants.EPFL_TOP_RIGHT_MAP || coords < CoordinateConstants.EPFL_BOT_LEFT_MAP)) {

			print ("Not in campus");
			DeactivateLocation ();
			locationSmootherFade.StopLocationHandling ();
			popup.SetText ("You have to be on the EPFL campus to switch to attack mode");

		} else if(!isAttackMode) {

			print ("Not in attack mode");
			locationSmootherFade.CoordinateUpdate (coords); 
			fadingPlayer.setVisible (false);
			bool isInSafeZone = fadingPlayer.isInsideSafeZone();

			if (!isInSafeZone) {

				print ("Not in safe zone");
				PlayerPrefs.SetInt (USERPREF_ATTACK_KEY, DEFENSE);

				Input.location.Stop ();
				Input.location.Start (1.0f, 1);

				fadingPlayer.setVisible (true);

			} else {
				print ("In safe zone");
				locationSmootherFade.StopLocationHandling ();
				locationSmoother.CoordinateUpdate (coords);
				PlayerPrefs.SetInt (USERPREF_ATTACK_KEY, ATTACK);

				Input.location.Stop ();
				Input.location.Start (1.0f, 1);
			}
			//popup.SetText ("You have to be in a safe zone to switch to attack mode");

		} else if (coords != oldCoords) {

			print ("In attack mode");
			locationSmoother.CoordinateUpdate (coords);

			Input.location.Stop ();
			Input.location.Start (1.0f, 1);

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

	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		print ("Stop attacking");
		isAttackMode = false;
	}

	override public void FirstLocationSent() {
		print ("Going to attack");
		isAttackMode = true;
	}
}

public abstract class LocationListener : MonoBehaviour {
	abstract public void CoordinateUpdate(XYCoordinate coords);
	abstract public void StopLocationHandling();
	abstract public void FirstLocationSent();
}