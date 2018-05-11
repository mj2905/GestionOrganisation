﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LocationHandler : LocationListener {

	private const int ATTACK = 1;
	private const int DEFENSE = 0;
	private const string USERPREF_ATTACK_KEY = "attackMode";

	public PopupScript popup;
	public Image switchGameButton;
	public FadingPlayer fadingPlayer;
	public Text locationTextTmp;

	public Image GPSImage;
	public Text GPSText;

	public Image cooldownImage;

	private bool activate = false;

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
			setGPSUIState (true);
		}
	}

	private void StopLocation() {
		if (locationWasEnabled) {
			Input.location.Stop ();
			started = false;
			coords = MapCoordinate.ZERO();
			setGPSUIState (false);
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
		isAttackMode = PlayerPrefs.GetInt (USERPREF_ATTACK_KEY, DEFENSE) == ATTACK;
		ActivateLocationIfPossible (isAttackMode); //should not run location at first, will be useful when app has quit
	}

	public void SwitchMode() {
		if (started) {
			print ("Deactivate location");
			DeactivateLocation ();
			locationSmootherFade.StopLocationHandling ();
		} else {
			print ("Attempting to activate location");
			ActivateLocationIfPossible ();
		}
	}

	public static void writeLastTerminalPlaced() {
		Persistency.writeTS (FirebaseManager.GetServerTime ());
	}

	private static float getRemainingTimeCapped(float cap = 1.0f) {
		long lastTS = Persistency.getLastTS ();
		if (lastTS == -1) {
			return cap;
		} else if (FirebaseManager.GetServerTime () == FirebaseManager.DEFAULT_TIME) {
			return 0;
		} else {
			return (float)(Countdown.getTime(FirebaseManager.GetServerTime()) - Countdown.getTime(lastTS)).TotalMinutes;
		}
	}

	private bool validSecondsSinceLastTerminalPlaced(float cap = 1.0f) {
		long lastTS = Persistency.getLastTS ();
		return getRemainingTimeCapped (cap) >= cap;
	}

	public void ActivateLocationIfPossible(bool additionalCondition = true, bool write = true) {

		if (additionalCondition && !Input.location.isEnabledByUser) {
			popup.SetText ("Location is not enabled on your device, please turn it on");
			return;
		}

		if (!validSecondsSinceLastTerminalPlaced()) {
			popup.SetText ("You are putting terminals too fast!");
			return;
		}

		locationWasEnabled = additionalCondition && (Input.location.status != LocationServiceStatus.Failed); //Only thing that can block it is if it's not allowed to be used
		if(write) {
			PlayerPrefs.SetInt (USERPREF_ATTACK_KEY, locationWasEnabled ? ATTACK : DEFENSE);
		}
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

				if (activate && Time.time % 30 < 25) {
					activate = false;
					Input.location.Stop ();
					Input.location.Start (1.0f, 0);
				} else if (Time.time % 30 > 25) {
					activate = true;
				}
				//Input.location.Stop ();
				//Input.location.Start (1.0f, 1);

				fadingPlayer.setVisible (true);

			} else {
				print ("In safe zone");
				locationSmootherFade.StopLocationHandling ();
				locationSmoother.CoordinateUpdate (coords);
				PlayerPrefs.SetInt (USERPREF_ATTACK_KEY, ATTACK);

				if (activate && Time.time % 30 < 25) {
					activate = false;
					Input.location.Stop ();
					Input.location.Start (1.0f, 0);
				} else if (Time.time % 30 > 25) {
					activate = true;
				}
				//Input.location.Stop ();
				//Input.location.Start (1.0f, 1);
			}
			//popup.SetText ("You have to be in a safe zone to switch to attack mode");

		} else if (coords != oldCoords) {

			print ("In attack mode");
			locationSmoother.CoordinateUpdate (coords);

			//Input.location.Stop ();
			//Input.location.Start (1.0f, 1);

		}
	}

	// Update is called once per frame
	void Update () {

		switchGameButton.fillAmount = 1.0f - getRemainingTimeCapped ();

		if (Input.location.status == LocationServiceStatus.Failed) {
			if (locationWasEnabled) {
				DeactivateLocation ();
			}
			popup.SetText ("You have to enable the geolocation to be able to be in attack mode. To do so, go to Settings, and choose to allow location access");
		}

		if (locationWasEnabled && Input.location.status == LocationServiceStatus.Running) {
			HandleCoordinates ();
		}
	}

	private void setGPSUIState(bool state) {
		float alpha;
		if (state) {
			alpha = 0.9f;

		} else {
			alpha = 0.1f;
		}

		Color im = GPSImage.color;
		im.a = alpha;
		GPSImage.color = im;

		Color te = GPSText.color;
		te.a = alpha;
		GPSText.color = te;
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