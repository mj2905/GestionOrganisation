using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    
	public GameObject player;
	public InteractionManager interactionManager;
	public bool CAMERA_FIXED = false;
	public Camera camera;
	public GameManager gameManager;

	public BoxCollider bottom;
	public BoxCollider top;
	public BoxCollider left;
	public BoxCollider right;
	public BoxCollider Limit;

	private const float SPEED_ZOOM = 4f;
	private const float SPEED_OUTSIDE = 5f;
	private const float TIME_OF_CLICK = 0.12f;

    private Vector3 dragOrigin;
    private Vector3 previousSceneRootPos;
	private GameObject sceneRoot;
	private Vector3 touchPosWorld;
	private Vector3 initialCameraPosition;
	private Quaternion initialCameraRotation;
	private GameObject focusedBuilding;
	private Vector3 offset;
	private bool onScreenBot = false;
	private bool onScreenTop = false;
	private bool onScreenLeft = false;
	private bool onScreenRight = false;
	private Vector3 groundSize;


	enum state {Idle,Clicking,Dragging,Focusing,Focused,Unfocusing};
	private state currentState = state.Idle;

	private float timer = 0f;

	// Use this for initialization
	void Start () {
        dragOrigin = new Vector3(0.0f, 0.0f, 0.0f);
		this.sceneRoot = gameManager.sceneRoot;
		this.initialCameraPosition = transform.position;
		this.initialCameraRotation = transform.rotation;

		offset = new Vector3 (0, 65.9f, -8.1f);;
		groundSize = GameObject.Find ("SceneRoot/Ground").GetComponent<Renderer> ().bounds.size;
	}

	void Update(){
		switch (currentState) {
		case state.Idle:
			if (Input.GetMouseButton (0)) {
				this.currentState = state.Clicking;
			}
			break;
		case state.Clicking:
			if (Input.GetMouseButton (0)) {
				timer += Time.deltaTime;
				Debug.Log (timer.ToString ());
				if (timer > TIME_OF_CLICK) {
					timer = 0;
					currentState = state.Dragging;
					dragOrigin = camera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y));
					previousSceneRootPos = sceneRoot.gameObject.transform.position;
				}
			} else {
				timer = 0;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
					if (Physics.Raycast (ray, out hit, 1000)) {
						if (hit.transform.gameObject.tag == "Zone") {
							initialCameraPosition = transform.position;
							initialCameraRotation = transform.rotation;
							focusedBuilding = hit.transform.gameObject;
							currentState = state.Focusing;

							if (gameManager.IsAttackMode ()) {

							} else {
								//Notify the interaction manager that the user focused on a zone
								Zone targetZone = hit.transform.gameObject.GetComponent<Zone> ();
								interactionManager.updateTargetedZone (targetZone);
							}

						} else {
							currentState = state.Idle;
						}
				}
			}
			break;
		case state.Dragging:
			if (!Input.GetMouseButton (0)) {
				currentState = state.Idle;
			} else {
				Vector3 currentWorldPos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y));
				Vector3 move = currentWorldPos - dragOrigin;
				move.y = 0.0f;
				sceneRoot.gameObject.transform.position = previousSceneRootPos + move;
			}
			break;
		case state.Focused:
			Ray ray2 = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit2;
			if (Input.GetMouseButton (0)) {
				if (Physics.Raycast (ray2, out hit2, 1000)) {
					if (hit2.transform.gameObject.tag == "Zone" || hit2.transform.gameObject.tag == "Ground") {
						if (focusedBuilding != null) {
							if (hit2.transform.gameObject.name != focusedBuilding.name) {
								currentState = state.Unfocusing;
								print ("CameraController: remove popups");
								interactionManager.updateTargetedZone (null);
								interactionManager.updateTargetedTerminal (null);
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
			break;
		case state.Unfocusing:
			if (focusedBuilding != null) {
				transform.position = Vector3.MoveTowards (transform.position, this.initialCameraPosition, SPEED_ZOOM);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, this.initialCameraRotation, SPEED_ZOOM);
			}
			if (transform.position == initialCameraPosition) {
				currentState = state.Idle;
			}
			break;
		case state.Focusing:
			if (focusedBuilding != null) {
				transform.position = Vector3.MoveTowards (transform.position, focusedBuilding.transform.position + new Vector3 (0, 30, -30), SPEED_ZOOM);
				transform.rotation = Quaternion.RotateTowards (transform.rotation,  Quaternion.Euler(45,0,0), SPEED_ZOOM);
				if(transform.position == focusedBuilding.transform.position + new Vector3 (0, 30, -30)){
					currentState = state.Focused;
				}
			}
			break;
		default:
			break;
		}

		if (CAMERA_FIXED) {
			transform.position = player.transform.position + offset;
			return;
		}



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
}
