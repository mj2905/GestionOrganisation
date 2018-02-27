using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    
	public GameObject player;
	public GameObject defendZonePopup;
	public GameObject defendTerminalPopup;
	public GameObject attackTerminalPopup;
	public ActionButtonState actionState;
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

		offset = transform.position - player.transform.position;
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
					if (hit.transform.gameObject.tag == "Zone" ||  hit.transform.gameObject.tag == "Ground") {


						if (focusedBuilding != null) {
							if (hit.transform.gameObject.name != focusedBuilding.name) {
								isFocusing = false;

								defendZonePopup.SetActive (false);
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
							if (actionState.attackMode) {

							} else {
								//TODO: refactor this for example in a popup handler object
								defendZonePopup.SetActive (true);
								Zone targetZone = hit.transform.gameObject.GetComponent<Zone> ();
								defendZonePopup.transform.Find ("ZoneLabel").GetComponent<Text> ().text = "Zone: " + targetZone.name;
								defendZonePopup.transform.Find ("HPLabel").GetComponent<Text> ().text = "HP: " + targetZone.health;
								defendZonePopup.transform.Find ("LevelLabel").GetComponent<Text> ().text = "Level: " + targetZone.level;
								defendZonePopup.transform.Find ("TeamLabel").GetComponent<Text> ().text = "Team: " + targetZone.team;
							}
						}

						dragging = !(isFocused || isFocusing) && !dragging;
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
