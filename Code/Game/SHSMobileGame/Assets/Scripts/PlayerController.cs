using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
	private GameObject currentZone; 
	private static float defaultLongH = 6.56586f;//6.699792f;
	private static float defaultLatV = 46.52018f;//46.55598f;

	private static float topLeftLat = 46.52261f, topLeftLong = 6.56058f;
	private static float topRightLat = 46.52261f, topRightLong = 6.5731f;
	private static float botLeftLat = 46.51705f, botLeftLong = 6.56058f;
	private static float botRightLat = 46.51705f, botRightLong = 6.5731f;

	private static float epflCenterLat = 46.52018f, epflCenterLong = 6.56586f;
	private static float epflCenterDifLat = epflCenterLat - defaultLatV, epflCenterDifLong = epflCenterLong - defaultLongH;

	private static float lastPosLongH = epflCenterLong;
	private static float lastPosLatV = epflCenterLat;

	//Obtained by computing the unity size of the map
	/*
	 * (77.15) x (50)
	957.9 m x 618.2 m
	0,01252 x 0,00556

	0.01252 -> 77.15
	x -> y

	y = 77.15x/0.01252
	 */

	private const float hFactor = 77.15f;
	private const float vFactor = 50.0f;
	bool locationWasStarted = false;

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

	Vector3 GetMovement() {
		if (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running) {
			float posLongH = Input.location.lastData.longitude + epflCenterDifLong;
			float posLatV = Input.location.lastData.latitude + epflCenterDifLat;

			float moveHorizontal = (posLongH - lastPosLongH)*hFactor/(topRightLong - topLeftLong);
			float moveVertical = (posLatV - lastPosLatV)*vFactor/(topLeftLat - botLeftLat);

			if (posLongH != lastPosLongH || posLatV != lastPosLatV) {
				print ("location retrieved : lat:" + posLatV + " | long:" + posLongH + " | mh:" + moveHorizontal + "(~" + (957.9f * (posLongH - lastPosLongH) / (topRightLong - topLeftLong)) + "m)" + " | mv:" + moveVertical + "(~" + (618.2f * (posLatV - lastPosLatV)/(topLeftLat - botLeftLat)) + "m)");
			}

			lastPosLatV = posLatV;
			lastPosLongH = posLongH;
			return new Vector3 (moveHorizontal, 0.0f, moveVertical);
		} else {
			return new Vector3 (0.0f, 0.0f, 0.0f);
		}
	}

	void FixedUpdate()
    {
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

		gameObject.transform.position += movement * speed;

		//Vector3 movement = GetMovement ();
		gameObject.transform.position += movement;// * speed;
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

	public Vector2 GetPosition(){
		return new Vector2 (gameObject.transform.localPosition.x, gameObject.transform.localPosition.z);
	}
}