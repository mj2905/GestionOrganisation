using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : LocationListener
{

    public float speed;
	private GameObject currentZone;

	private double moveVertical;
	private double moveHorizontal;

	//Obtained by computing the unity size of the map
	/*
	 * (77.15) x (50)
	957.9 m x 618.2 m
	0,01252 x 0,00556

	0.01252 -> 77.15
	x -> y

	y = 77.15x/0.01252
	 */

	private Vector3 initialPosition;

    void Start()
    {
		gameObject.GetComponent<Renderer>().enabled = false;
		initialPosition = new Vector3(77.15f, 2, -50.0f);
		transform.localPosition = initialPosition;

		moveVertical = 0;
		moveHorizontal = 0;
		 
		currentZone = null;
    }

	Vector3 GetPosition() {
		return -new Vector3 ((float)moveHorizontal, 0.0f, (float)moveVertical);
	}

	void FixedUpdate()
    {
		transform.localPosition = initialPosition + GetPosition();
    }

	void OnTriggerEnter(Collider other) 
	{
		if (other is CapsuleCollider) {
			print ("Safe zone entered: " + other.gameObject.name);
		}
		else {
			currentZone = other.gameObject;
			print ("Entered: " + currentZone.name);

			//currentZone.transform.parent.localPosition -= new Vector3 (0, 2, 0);
			//currentZone.transform.localPosition += new Vector3 (0, 0, 7);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other is CapsuleCollider) {
			print ("Safe zone exited: " + other.gameObject.name);
		} else {
			print ("Exited: " + currentZone.name);

			//currentZone.transform.localPosition -= new Vector3 (0, 0, 7);
			//currentZone.transform.parent.localPosition += new Vector3 (0, 2, 0);

			currentZone = null;
		}
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

	override public void CoordinateUpdate(XYCoordinate coords) {

		double lastMovH = moveHorizontal, lastMovV = moveVertical;

		XYCoordinate posXY = CoordinateConstants.EPFL_CENTER_DIF + coords;

		moveHorizontal = (posXY.x() - CoordinateConstants.EPFL_TOP_LEFT_XY.x())*CoordinateConstants.H_FACTOR/CoordinateConstants.EPFL_HORIZONTAL_DISTANCE;
		moveVertical = (posXY.y() - CoordinateConstants.EPFL_TOP_LEFT_XY.y())*CoordinateConstants.V_FACTOR/CoordinateConstants.EPFL_VERTICAL_DISTANCE;

		if (CoordinateConstants.DEBUG != CoordinateConstants.DEBUG_STATE.NO_DEBUG) {
			if (moveHorizontal != lastMovH || moveVertical != lastMovV) {
				print ("mh:" + moveHorizontal + " | mv:" + moveVertical);
			}
		}

	}

	override public void StopLocationHandling() {
		moveVertical = 0;
		moveHorizontal = 0;
		gameObject.GetComponent<Renderer>().enabled = false;
	}

	override public void FirstLocationSent() {
		gameObject.GetComponent<Renderer>().enabled = true;
	}

}
