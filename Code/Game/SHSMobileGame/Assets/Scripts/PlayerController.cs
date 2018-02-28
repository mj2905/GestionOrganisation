using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
	private GameObject currentZone; 
	private static double defaultLongH = 6.56586;//6.699792f;
	private static double defaultLatV = 46.52018;//46.55598f;

	private static double topLeftLat = 46.52261, topLeftLong = 6.56058;
	private static double topRightLat = 46.52261, topRightLong = 6.5731;
	private static double botLeftLat = 46.51705, botLeftLong = 6.56058;
	private static double botRightLat = 46.51705, botRightLong = 6.5731;

	private static double horizontalDistance = topRightLong - topLeftLong;
	private static double verticalDistance = topLeftLat - botLeftLat;

	private static double epflCenterLat = 46.52018, epflCenterLong = 6.56586;
	private static double epflCenterDifLat = epflCenterLat - defaultLatV, epflCenterDifLong = epflCenterLong - defaultLongH;

	private static double lastPosLongH = epflCenterLong;
	private static double lastPosLatV = epflCenterLat;

	private const bool DEBUG = false;

	//Obtained by computing the unity size of the map
	/*
	 * (77.15) x (50)
	957.9 m x 618.2 m
	0,01252 x 0,00556

	0.01252 -> 77.15
	x -> y

	y = 77.15x/0.01252
	 */

	private const double hFactor = 2*77.15; //(1/l, 1/2h)/(l, h) = (1/2, 1/2). Need to multiply by 2 to get (1,1) and by the factors to reach the position in editor
	private const double vFactor = 2*50.0;
	bool locationWasStarted = false;
	private Vector3 initialPosition;

	public void BeginLocation() {
		Input.location.Start (5.0f, 5.0f);
		locationWasStarted = true;
	}

	public void StopLocation() {
		Input.location.Stop ();
		locationWasStarted = false;
	}

    void Start()
    {
		initialPosition = new Vector3(12, 8, -6.2f);
		currentZone = null;
    }

	void OnApplicationFocus(bool focusStatus) {
		if (locationWasStarted) {
			if (!focusStatus) {
				print ("Stopped");
				StopLocation ();
				locationWasStarted = true; //So that the next app focus will begin again the location
			} else {
				print ("Restart");
				BeginLocation ();
			}
		}
	}

	void OnDisable() {
		StopLocation ();
	}

	Vector3 GetPosition() {
		if (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running) {
			double posLongH = Input.location.lastData.longitude + epflCenterDifLong;
			double posLatV = Input.location.lastData.latitude + epflCenterDifLat;

			double moveHorizontal = (posLongH - epflCenterLong)*hFactor/(topRightLong - topLeftLong);
			double moveVertical = (posLatV - epflCenterLat)*vFactor/(topLeftLat - botLeftLat);

			if (DEBUG) {
				if (posLongH != lastPosLongH || posLatV != lastPosLatV) {
					print ("-----" + posLatV + " " + epflCenterLat + " " + topLeftLat + " " + botLeftLat);
					print ("location retrieved : lat:" + posLatV + " | long:" + posLongH + " | mh:" + moveHorizontal + " | mv:" + moveVertical + "(~" + (957.9 * (posLongH - epflCenterLong) / (topRightLong - topLeftLong)) + "m)" + "(~" + (618.2 * (posLatV - epflCenterLat) / (topLeftLat - botLeftLat)) + "m)");
				}
			}

			lastPosLatV = posLatV;
			lastPosLongH = posLongH;
			return new Vector3 ((float)moveHorizontal, 0.0f, (float)moveVertical);
		} else {
			return new Vector3 (0.0f, 0.0f, 0.0f);
		}
	}

	void FixedUpdate()
    {
		gameObject.transform.position = initialPosition + GetPosition();
    }

	void OnTriggerEnter(Collider other) 
	{
		currentZone = other.gameObject;
		print ("Entered: " + currentZone.name);
	}

	void OnTriggerExit(Collider other)
	{
		currentZone = null;
		print ("Exited: " + other.gameObject.name);
	}

	public bool isInsideZone(){
		return currentZone != null;
	}

	public string GetCurrentZoneName(){
		if (currentZone != null) {
			return currentZone.name.Split ('_') [0];
		} else
			return "";
	}

	public Vector2 GetMarkerPosition(){
		return new Vector2 (gameObject.transform.localPosition.x, gameObject.transform.localPosition.z);
	}
}
