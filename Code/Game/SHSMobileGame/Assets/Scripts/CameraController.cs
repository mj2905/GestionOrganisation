using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraController : LocationListener {

	public GameObject player;
	public GameObject fadingplayer;
	public InteractionManager interactionManager;
	public Camera camera;
	public GameManager gameManager;

	public BoxCollider bottom;
	public BoxCollider top;
	public BoxCollider left;
	public BoxCollider right;
	public BoxCollider Limit;

	private const float SPEED_ZOOM = 4f;
	private const float SPEED_OUTSIDE = 3f;
	private const float SPEED_SWITCH_MODE = 5f;
	private const float MAX_DIST_DRAG = 4.0f;

	private Vector3 dragOrigin;
	private Vector3 previousSceneRootPos;
	private GameObject sceneRoot;
	private Vector3 touchPosWorld;
	private Vector3 positionBeforeFocus;
	private Quaternion rotationBeforeFocus;
	private float zoomBeforeFocus;
	private Zone focusedZone;
	private GameObject focusedBuilding;
	private Vector3 offset;
	private bool onScreenBot = false;
	private bool onScreenTop = false;
	private bool onScreenLeft = false;
	private bool onScreenRight = false;
	private Vector3 groundSize;
	private Vector3 initialPosition;
	private bool isAttackMode = false;
	private bool adjustingFocusZoom = false;

	private Vector3 startPosition;
	private Vector3 currentPosition;

	public Button changeModeButton;

	// TOUCH GESTURE VARIABLE
	private Vector2 touchOrigin = -Vector2.one;
	private float perspectiveZoomSpeed = .08f;
	private int mainFingerId = 0;
	private Touch t0;
	private Touch t1;

	Dictionary<int, Touch> touchDict;

	enum state {Idle,Clicking,Dragging,Focusing,Focused,Unfocusing,FixedOnPlayer, FixedOnFadingPlayer,MovingToPlayer, MovingToFadingPlayer,MovingToInitialPos, MovingToInitialPosFromFading,MovingToPosition};
	private state currentState = state.Idle;

	// Use this for initialization
	void Start () {
		dragOrigin = new Vector3(0.0f, 0.0f, 0.0f);
		this.sceneRoot = gameManager.sceneRoot;
		this.positionBeforeFocus = transform.position;
		this.rotationBeforeFocus = transform.rotation;
		this.zoomBeforeFocus = camera.fieldOfView;

		this.adjustingFocusZoom = false;

		offset = new Vector3 (0, 65.9f, -8.1f);
		groundSize = GameObject.Find ("SceneRoot/Ground").GetComponent<Renderer> ().bounds.size;
		initialPosition = transform.position;

		Touch t0 = new Touch ();
		Touch t1 = new Touch ();

		touchDict = new Dictionary<int, Touch> ();
	}

	void Update(){
		//Debug.Log (currentState);

		// Reset fingerID if no more touches, otherwise if first touch, set it to 0.
		if (Input.touchCount == 0) {
			mainFingerId = -1;
			//Debug.Log ("Reset mainfingerID");
		}

		if (Input.touchCount >= 1) {
			t0 = Input.GetTouch (0);
			touchDict[t0.fingerId] = t0;

			if (mainFingerId == -1) {
				mainFingerId = t0.fingerId;
			}
		}
		if (Input.touchCount >= 2) {
			t1 = Input.GetTouch (1);
			touchDict[t1.fingerId] = t1;
			handleZoom ();
		}

		//Debug.Log ("Current state: " + currentState);

		updateState ();
		recenterOutboundCamera ();

	}

	private void updateState(){
		switch (currentState) {
		case state.Idle:
			if (Input.touchCount > 0) {
				if (!IsPointerOverUIObject ()) {
					this.currentState = state.Clicking;
					//this.startPosition = camera.ScreenToWorldPoint (new Vector3 (touchDict[mainFingerId].position.x, touchDict[mainFingerId].position.y, camera.transform.position.y));
				}
			}
			break;
		case state.Clicking:
			handleClickState ();
			break;
		case state.Dragging:
			handleDragState ();
			break;
		case state.FixedOnPlayer:
			transform.position = player.transform.position + offset;
			return;
		case state.FixedOnFadingPlayer:
			transform.position = fadingplayer.transform.position + offset;
			return;
		case state.MovingToInitialPos:
			transform.rotation = Quaternion.RotateTowards (transform.rotation, this.rotationBeforeFocus, SPEED_ZOOM);
			camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, this.zoomBeforeFocus, SPEED_ZOOM);
			transform.position = Vector3.MoveTowards (transform.position, initialPosition, SPEED_SWITCH_MODE);
			if (transform.position == initialPosition) {
				currentState = state.Idle;
			}
			break;
		case state.MovingToInitialPosFromFading:
			transform.rotation = Quaternion.RotateTowards (transform.rotation, this.rotationBeforeFocus, SPEED_ZOOM);
			camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, this.zoomBeforeFocus, SPEED_ZOOM);
			transform.position = Vector3.MoveTowards (transform.position, initialPosition, SPEED_SWITCH_MODE);
			if (transform.position == initialPosition) {
				currentState = state.Idle;
			}
			break;
		case state.MovingToPlayer:
			transform.rotation = Quaternion.RotateTowards (transform.rotation, this.rotationBeforeFocus, SPEED_ZOOM);
			camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, this.zoomBeforeFocus, SPEED_ZOOM);
			transform.position = Vector3.MoveTowards (transform.position, player.transform.position + offset, SPEED_SWITCH_MODE);
			if (transform.position == player.transform.position + offset) {
				currentState = state.FixedOnPlayer;
			}
			break;
		case state.MovingToFadingPlayer:
			transform.rotation = Quaternion.RotateTowards (transform.rotation, this.rotationBeforeFocus, SPEED_ZOOM);
			camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, this.zoomBeforeFocus, SPEED_ZOOM);
			transform.position = Vector3.MoveTowards (transform.position, fadingplayer.transform.position + offset, SPEED_SWITCH_MODE);
			if (transform.position == fadingplayer.transform.position + offset) {
				currentState = state.FixedOnFadingPlayer;
			}
			break;
		case state.Focused:
			handleFocusedState ();
			break;
		case state.Unfocusing:
			if (focusedBuilding != null) {
				transform.position = Vector3.MoveTowards (transform.position, this.positionBeforeFocus, SPEED_ZOOM);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, this.rotationBeforeFocus, SPEED_ZOOM);
				camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, this.zoomBeforeFocus, SPEED_ZOOM);
			}
			if (transform.position == positionBeforeFocus) {
				currentState = state.Idle;
				changeModeButton.interactable = true;
			}
			break;
		case state.Focusing:

			changeModeButton.interactable = false;

			if (focusedBuilding != null) {
				transform.position = Vector3.MoveTowards (transform.position, focusedBuilding.transform.position + new Vector3 (0, 30, -30), SPEED_ZOOM);
				transform.rotation = Quaternion.RotateTowards (transform.rotation,  Quaternion.Euler(45,0,0), SPEED_ZOOM);
				camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, 60, SPEED_ZOOM);
				if(transform.position == focusedBuilding.transform.position + new Vector3 (0, 30, -30)){
					currentState = state.Focused;
				}
			}
			break;
		default:
			break;
		}
	}

	private void handleZoom(){
		if (Input.touchCount >= 2) {
			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = t0.position - t0.deltaPosition;
			Vector2 touchOnePrevPos = t1.position - t1.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (t0.position - t1.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// Otherwise change the field of view based on the change in distance between the touches.
			camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

			// Clamp the field of view to make sure it's between 0 and 180.
			camera.fieldOfView = Mathf.Clamp (camera.fieldOfView, 20.0f, 120.0f);
		}
	}

	private void handleClickState(){
		if (Input.touchCount > 0) {
			if (touchDict[mainFingerId].phase == TouchPhase.Moved) {
				dragOrigin = camera.ScreenToWorldPoint (new Vector3 (touchDict[mainFingerId].position.x, touchDict[mainFingerId].position.y, camera.transform.position.y));
				//Debug.Log ((startPosition - currentPosition).magnitude);
				if (touchDict[mainFingerId].deltaPosition.magnitude > MAX_DIST_DRAG) {
					//dragOrigin = currentPosition;
					currentState = state.Dragging;
					previousSceneRootPos = sceneRoot.gameObject.transform.position;
				}
			} else if (touchDict[mainFingerId].phase == TouchPhase.Ended) {

				Ray ray = Camera.main.ScreenPointToRay (touchDict[mainFingerId].position);
				//RaycastHit hit;
				//if (Physics.Raycast (ray, out hit, 1000)) {
				RaycastHit[] hits = Physics.RaycastAll (ray, 1000);

				bool has_hit = false;
				int hitOne = -1;
				for (int i = 0; i < hits.Length; ++i) {
					if (hits [i].transform.gameObject.tag == "Zone") {
						has_hit = true;
						hitOne = i;
						break;
					}
				}

				if (has_hit) {
					positionBeforeFocus = transform.position;
					rotationBeforeFocus = transform.rotation;
					zoomBeforeFocus = camera.fieldOfView;

					focusedBuilding = hits [hitOne].transform.gameObject;
					currentState = state.Focusing;

					if (!isAttackMode) {
						//Notify the interaction manager that the user focused on a zone
						Zone targetZone = hits [hitOne].transform.gameObject.GetComponent<Zone> ();
						focusedZone = targetZone;
						focusedZone.hideChart (true);
						interactionManager.updateTargetedZone (targetZone);
						gameManager.DrawTerminalsUI (targetZone.zoneId);
					}

				} else {
					currentState = state.Idle;
				}
			}
		}
	}

	private void handleDragState(){
		if (Input.touchCount > 0) {
			if (Input.touchCount > 1 && touchDict [mainFingerId].phase == TouchPhase.Ended) {
				// Continue dragging if there are other fingers on screen
				Vector3 currentWorldPos = camera.ScreenToWorldPoint (new Vector3 (touchDict [mainFingerId].position.x, touchDict [mainFingerId].position.y, camera.transform.position.y));
				mainFingerId = (t0.fingerId != mainFingerId) ? t0.fingerId : t1.fingerId;
				//Debug.Log ("Main fingerID: " + mainFingerId);
				Vector3 newWorldPos = camera.ScreenToWorldPoint (new Vector3 (touchDict [mainFingerId].position.x, touchDict [mainFingerId].position.y, camera.transform.position.y));
				dragOrigin += newWorldPos - currentWorldPos;

			} else if (touchDict [mainFingerId].phase == TouchPhase.Moved) {
				Vector3 currentWorldPos = camera.ScreenToWorldPoint (new Vector3 (touchDict [mainFingerId].position.x, touchDict [mainFingerId].position.y, camera.transform.position.y));
				Vector3 move = currentWorldPos - dragOrigin;

				move.y = 0.0f;
				//Debug.Log ("x: " + move.x + " y: " + move.y + " z: " + move.z);
				sceneRoot.gameObject.transform.position = previousSceneRootPos + move;
			}
		} else {
			currentState = state.Idle;
		}
	}

	private void Dezoom(){
		currentState = state.Unfocusing;
		print ("CameraController: remove popups");
		if (focusedZone != null) {
			focusedZone.hideChart (false);
			focusedZone = null;
		}
		interactionManager.updateTargetedZone (null);
		interactionManager.updateTargetedTerminal (null);
		gameManager.DrawTerminalsUI ("");
	}

	private void handleFocusedState(){
		if (Input.touchCount > 1) {
			adjustingFocusZoom = true;
		} else if (Input.touchCount == 1) {
			Ray ray2 = Camera.main.ScreenPointToRay (touchDict[mainFingerId].position);
			RaycastHit hit2;

			if (!IsPointerOverUIObject () && touchDict[mainFingerId].phase == TouchPhase.Ended) {

				if (adjustingFocusZoom) {
					adjustingFocusZoom = false;
				} else if (Physics.Raycast (ray2, out hit2, 1000000)) {
					if (hit2.transform.gameObject.tag == "Zone" || hit2.transform.gameObject.tag == "Ground" || hit2.transform.gameObject.tag == "SafeZone") {
						if (focusedBuilding != null) {
							if (hit2.transform.gameObject.name != focusedBuilding.name) {
								Dezoom ();
							} else {
								if (interactionManager.isTerminalSelected ()) {
									Zone targetZone = hit2.transform.gameObject.GetComponent<Zone> ();
									interactionManager.updateTargetedZone (targetZone);
									print ("CameraController: back to zone");
								} else {
									Dezoom ();
								}
							}
						}
					} else {
						if (hit2.transform.gameObject.tag == "Terminal") {
							print ("CameraController: Hit on a terminal");
							Terminal targetTerminal = hit2.transform.gameObject.GetComponentInParent<Terminal> ();
							interactionManager.updateTargetedTerminal (targetTerminal);
						}
					}
				}
			}
		}
	}

	private void recenterOutboundCamera(){
		onScreenBot = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes (camera),bottom.bounds);
		onScreenTop = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes (camera),top.bounds);
		onScreenLeft = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes (camera),left.bounds);
		onScreenRight = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes (camera),right.bounds);

		if (onScreenBot && currentState == state.Idle) {
			sceneRoot.transform.position = Vector3.MoveTowards (sceneRoot.transform.position, sceneRoot.transform.position + new Vector3 (0,0, -30), SPEED_OUTSIDE);
		}
		if (onScreenTop && currentState == state.Idle) {
			sceneRoot.transform.position = Vector3.MoveTowards (sceneRoot.transform.position, sceneRoot.transform.position + new Vector3 (0,0, 30), SPEED_OUTSIDE);
		}
		if (onScreenLeft && currentState == state.Idle) {
			sceneRoot.transform.position = Vector3.MoveTowards (sceneRoot.transform.position, sceneRoot.transform.position + new Vector3 (-30,0, 0), SPEED_OUTSIDE);
		}
		if (onScreenRight && currentState == state.Idle) {
			sceneRoot.transform.position = Vector3.MoveTowards (sceneRoot.transform.position, sceneRoot.transform.position + new Vector3 (30,0, 0), SPEED_OUTSIDE);
		}

		sceneRoot.transform.position = new Vector3(
			Mathf.Clamp (sceneRoot.transform.position.x,- Limit.bounds.size.x + groundSize.x,Limit.bounds.size.x- groundSize.x),
			0,
			Mathf.Clamp (sceneRoot.transform.position.z,- Limit.bounds.size.z+ groundSize.z,Limit.bounds.size.z- groundSize.z)
		);
	}

	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(touchDict[mainFingerId].position.x, touchDict[mainFingerId].position.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	public void GoToFadingPlayer() {
		if (!isAttackMode) {
			currentState = state.MovingToFadingPlayer;
		}
	}

	public void LeaveFadingPlayer() {
		if (!isAttackMode) {
			currentState = state.MovingToInitialPosFromFading;
		}
	}

	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		print ("Loc finished on camera");
		currentState = state.MovingToInitialPos;
		isAttackMode = false;
	}

	override public void FirstLocationSent() {
		currentState = state.MovingToPlayer;
		isAttackMode = true;
	}

	public void GoToBuilding(Zone focusedBuilding){
		this.focusedBuilding = focusedBuilding.gameObject;
		interactionManager.updateTargetedZone (focusedBuilding);
		gameManager.DrawTerminalsUI (focusedBuilding.zoneId);
		currentState = state.Focusing;
	}
}
