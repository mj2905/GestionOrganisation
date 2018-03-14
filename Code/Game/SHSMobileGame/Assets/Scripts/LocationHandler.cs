using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHandler : LocationListener {

	private const int ATTACK = 1;
	private const int DEFENSE = 0;
	private const string USERPREF_ATTACK_KEY = "attackMode";

	public PopupScript popup;
	public FadingPlayer fadingPlayer;

	private MapCoordinate coords = MapCoordinate.ZERO();

	private bool firstLocation = false;
	private bool started = false;
	private bool isAttackMode = false; 

	public LocationListener[] listeners;
	private Collider[] colliders = new Collider[10];

	private bool locationWasEnabled = false; //by app, when someone clicks on attack button for example
	//invariant : if locationWasEnabled = false, location is stopped

	private void BeginLocation() {
		if (locationWasEnabled && !started) {
			Input.location.Start (1.0f, 0);
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
		isAttackMode = false;
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
		coords = CoordinateConstants.DEBUG == CoordinateConstants.DEBUG_STATE.WALKING_PATH ? 
			CoordinateConstants.WALKING_PATH.next() :
			new MapCoordinate (Input.location.lastData.longitude, Input.location.lastData.latitude);

		fadingPlayer.SetCoordsInvisible (coords); //used to get its 3d vector, can be used here because only choices after are either a location deactivated => reset, or put again to same position.
		Vector3 pos = fadingPlayer.transform.position;
		//print (pos.x + " " + pos.y + " " + pos.z);

		int colls = Physics.OverlapBoxNonAlloc(pos, new Vector3(1,1,1), colliders);
		//print (colls);

		bool isInSafeZone = true;
		for (int i = 0; i < colls; ++i) {
			//print (colliders [i].gameObject.name);
			if (colliders[i] is CapsuleCollider) {
				isInSafeZone = true;
				break;
			}
		}

		if (CoordinateConstants.DEBUG == CoordinateConstants.DEBUG_STATE.NO_DEBUG && (coords > CoordinateConstants.EPFL_TOP_RIGHT_MAP || coords < CoordinateConstants.EPFL_BOT_LEFT_MAP)) {

			DeactivateLocation ();
			popup.SetText ("You have to be on the EPFL campus to switch to attack mode");

		} else if(!isInSafeZone && !isAttackMode) {
			
			fadingPlayer.SetCoordsVisible (coords);

			DeactivateLocation ();
			popup.SetText ("You have to be in a safe zone to switch to attack mode");

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

	override public void CoordinateUpdate(MapCoordinate coords) {}

	override public void StopLocationHandling() {
		isAttackMode = false;
	}

	override public void FirstLocationSent() {
		isAttackMode = true;
	}
}

public abstract class LocationListener : MonoBehaviour {
	abstract public void CoordinateUpdate(MapCoordinate coords);
	abstract public void StopLocationHandling();
	abstract public void FirstLocationSent();
}