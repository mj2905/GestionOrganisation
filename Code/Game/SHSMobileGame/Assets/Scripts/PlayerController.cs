using UnityEngine;
using System.Collections;

public class PlayerController : LocationListener
{

    public float speed;
	private GameObject currentZone;

	private static readonly XYCoordinate EPFL_CENTER_DIF = CoordinateConstants.DEBUG == CoordinateConstants.DEBUG_STATE.TEST_LOCATION ? 
															CoordinateConstants.EPFL_CENTER_XY - CoordinateConstants.TEST_LOC_XY : XYCoordinate.ZERO ();

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

	private const double H_FACTOR = 2*77.15; //(1/l, 1/2h)/(l, h) = (1/2, 1/2). Need to multiply by 2 to get (1,1) and by the factors to reach the position in editor
	private const double V_FACTOR = 2*50.0;
	private Vector3 initialPosition;

    void Start()
    {
		gameObject.GetComponent<Renderer>().enabled = false;
		initialPosition = new Vector3(77.15f, 2, -50.0f);
		transform.localPosition = new Vector3(12, 2, -7);

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

	override public void CoordinateUpdate(MapCoordinate coords) {

		double lastMovH = moveHorizontal, lastMovV = moveVertical;

		XYCoordinate posXY = EPFL_CENTER_DIF + coords.toXYMercator ();

		moveHorizontal = (posXY.x() - CoordinateConstants.EPFL_TOP_LEFT_XY.x())*H_FACTOR/CoordinateConstants.EPFL_HORIZONTAL_DISTANCE;
		moveVertical = (posXY.y() - CoordinateConstants.EPFL_TOP_LEFT_XY.y())*V_FACTOR/CoordinateConstants.EPFL_VERTICAL_DISTANCE;

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
