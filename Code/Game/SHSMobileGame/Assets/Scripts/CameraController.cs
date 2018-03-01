using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    
	public GameObject player;
	public InteractionManager interactionManager;
	public bool CAMERA_FIXED = false;
	public Camera camera;
	public GameManager gameManager;
	public float zoomSpeed;

	private const float SPEED = 4f;
    private Vector3 dragOrigin;
    private Vector3 previousSceneRootPos;
    private bool dragging;
	private GameObject sceneRoot;
	private Vector3 touchPosWorld;
	private Vector3 initialCameraPosition;
	private Quaternion initialCameraRotation;
	private GameObject focusedBuilding;
	private bool isFocused,isFocusing;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
        dragOrigin = new Vector3(0.0f, 0.0f, 0.0f);
        dragging = false;
		isFocused = false;
		isFocusing = false;

		this.sceneRoot = gameManager.sceneRoot;
		this.initialCameraPosition = transform.position;
		this.initialCameraRotation = transform.rotation;

		offset = new Vector3 (0, 65.9f, -8.1f);
	}
	
	// LateUpdate is called once per frame, after all objects updates
	void LateUpdate () {

		if (CAMERA_FIXED) {
			transform.position = player.transform.position + offset;
			return;
		}

        // Starts dragging procedure if a click was started outside of the UI
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y));
            previousSceneRootPos = sceneRoot.gameObject.transform.position;
			dragging = !(isFocused || isFocusing);
        } 

        if (Input.GetMouseButtonUp(0))
        {
            print("Stopped dragging");

			if (!EventSystem.current.IsPointerOverGameObject())
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, 1000)) {
					if (hit.transform.gameObject.tag == "Zone" || hit.transform.gameObject.tag == "Ground") {


						if (focusedBuilding != null) {
							if (hit.transform.gameObject.name != focusedBuilding.name) {
								isFocusing = false;
					
							}
							if (!isFocused) {
								this.initialCameraPosition = transform.position;
								this.initialCameraRotation = transform.rotation;
							}
						}

						if (!isFocused && hit.transform.gameObject.tag == "Zone") {
							focusedBuilding = hit.transform.gameObject;
							isFocusing = true;

							//Check game mode to display the proper pop-up
							if (gameManager.IsAttackMode ()) {

							} else {
								//Notify the interaction manager that the user focused on a zone
								Zone targetZone = hit.transform.gameObject.GetComponent<Zone> ();
								interactionManager.updateTargetedZone (targetZone);
							}
						}

						dragging = !(isFocused || isFocusing) && !dragging;
					} else if (hit.transform.gameObject.tag == "Terminal") {
						print ("CameraController: Hit on a terminal");

						Terminal targetTerminal = hit.transform.gameObject.GetComponentInParent<Terminal> ();
						interactionManager.updateTargetedTerminal (targetTerminal);

					} else {
						//Notify the interaction manager that the user is no longer focused on a zone
						print ("CameraController: remove popups");
						interactionManager.updateTargetedZone (null);
						interactionManager.updateTargetedTerminal(null);
					}
				}
			} else {
				print("Clicked on UI");
				return;
			}
        }

		if (focusedBuilding != null) {
			if (!isFocusing) {
				transform.position = Vector3.MoveTowards (transform.position, this.initialCameraPosition, SPEED);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, this.initialCameraRotation, SPEED);
			} else {
				transform.position = Vector3.MoveTowards (transform.position, focusedBuilding.transform.position + new Vector3 (0, 30, -30), SPEED);
				transform.rotation = Quaternion.RotateTowards (transform.rotation,  Quaternion.Euler(45,0,0), SPEED);
			}

			if(transform.position == focusedBuilding.transform.position + new Vector3 (0, 30, -30)){
				isFocused = true;
			}
		}
			
		if (transform.position == initialCameraPosition) {
			isFocused = false;
		}



        if (dragging)
        {
			Vector3 currentWorldPos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y));
            Vector3 move = currentWorldPos - dragOrigin;
            move.y = 0.0f;
			move *= 0.7f;

            // Set the transformation of the root object to give impression of camera movement
            sceneRoot.gameObject.transform.position = previousSceneRootPos + move;

            return;
        }

        Ray ray2 = camera.ScreenPointToRay(Input.mousePosition);
        float zoomDistance = zoomSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        camera.transform.Translate(ray2.direction * zoomDistance, Space.World);
    }
}
