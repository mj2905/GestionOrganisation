using UnityEngine;
using System.Collections;

public class PlayerController : LocationListener
{

    public float speed;
	private GameObject currentZone;

	private static double epflCenterY = MercatorProjection.latToY(46.52018), epflCenterX = MercatorProjection.lonToX(6.56586);

	//TODO: remettre les bonnes coordonnées de l'epfl, et pas celles de test
	private static double defaultY = MercatorProjection.latToY(46.55598), defaultX = MercatorProjection.lonToX(6.699792);
	//rolex : private static double defaultY = MercatorProjection.latToY(46.5189), defaultX = MercatorProjection.lonToX(6.5683);
	//private static double defaultY = epflCenterY, defaultX = epflCenterX;


	private static double topLeftY = MercatorProjection.latToY(46.52261), topLeftX = MercatorProjection.lonToX(6.56058);
	private static double topRightY = MercatorProjection.latToY(46.52261), topRightX = MercatorProjection.lonToX(6.5731);
	private static double botLeftY = MercatorProjection.latToY(46.51705), botLeftX = MercatorProjection.lonToX(6.56058);
	private static double botRightY = MercatorProjection.latToY(46.51705), botRightX = MercatorProjection.lonToX(6.5731);

	private static double horizontalDistance = topRightX - topLeftX;
	private static double verticalDistance = topLeftY - botLeftY;

	private static double epflCenterDifY = epflCenterY - defaultY;//0;
	private static double epflCenterDifX = epflCenterX - defaultX;//0;

	private double moveVertical;
	private double moveHorizontal;

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
	private Vector3 initialPosition;

    void Start()
    {
		gameObject.GetComponent<Renderer>().enabled = false;
		initialPosition = new Vector3(77.15f, 8, -50.0f);
		transform.localPosition = new Vector3(12, 8, -7);

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

	override public void CoordinateUpdate(double latitude, double longitude) {

		double lastMovH = moveHorizontal, lastMovV = moveVertical;

		double posX = epflCenterDifX + MercatorProjection.lonToX(longitude);
		double posY = epflCenterDifY + MercatorProjection.latToY(latitude);

		moveHorizontal = (posX - topLeftX)*hFactor/horizontalDistance;
		moveVertical = (posY - topLeftY)*vFactor/verticalDistance;

		if (DEBUG) {
			if (moveHorizontal != lastMovH || moveVertical != lastMovV) {
				print ("-----" + posX + " " + epflCenterX + " " + topLeftX + " " + topRightX);
				print ("-----" + posY + " " + epflCenterY + " " + topLeftY + " " + botLeftY);
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
