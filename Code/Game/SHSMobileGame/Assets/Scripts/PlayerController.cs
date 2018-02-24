using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
	public GameManager gameManager;
	private GameObject currentZone; 
	private static float defaultLongH = 6.699792f;
	private static float defaultLatV = 46.55598f;

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

    void Start()
    {
		Input.location.Start (5.0f, 5.0f);
		currentZone = null;
    }

	void OnApplicationFocus(bool focusStatus) {
		if (!focusStatus) {
			print ("Stopped");
			Input.location.Stop ();
		} else {
			print ("Restart");
			Input.location.Start (5.0f, 5.0f);
		}
	}

	void OnDisable() {
		Input.location.Stop ();
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
			print (Input.location.status);
			return new Vector3 (0.0f, 0.0f, 0.0f);
		}
	}

	void FixedUpdate()
    {
		Vector3 movement = GetMovement ();
		gameObject.transform.position += movement;// * speed;

		if (currentZone != null && Input.GetKeyDown (KeyCode.T)) {
			gameManager.AddTerminal(currentZone.name.Split('_')[0],gameObject.transform.localPosition.x,gameObject.transform.localPosition.z);
		}
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
}