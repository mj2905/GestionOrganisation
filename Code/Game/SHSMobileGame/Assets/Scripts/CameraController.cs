using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {
    
    private Vector3 dragOrigin;
    private Vector3 previousSceneRootPos;
    private bool dragging;
    public Camera camera;
	public GameManager gameManager;
    public float zoomSpeed;

	private GameObject sceneRoot;

	private Vector3 touchPosWorld;
	private Vector3 initialCameraPosition;
	private Quaternion initialCameraRotation;
	private GameObject focusedBuilding;
	private bool isFocused;

	private const float SPEED = 4f;

	// Use this for initialization
	void Start () {
        dragOrigin = new Vector3(0.0f, 0.0f, 0.0f);
        dragging = false;
		isFocused = false;
		this.sceneRoot = gameManager.sceneRoot;
		this.initialCameraPosition = transform.position;
		this.initialCameraRotation = transform.rotation;
	}
	
	// LateUpdate is called once per frame, after all objects updates
	void LateUpdate () {

        // Starts dragging procedure if a click was started outside of the UI
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                dragOrigin = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.transform.position.y));
                previousSceneRootPos = sceneRoot.gameObject.transform.position;

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, 1000)) {
					if (hit.transform.gameObject.tag == "Zone" ||  hit.transform.gameObject.tag == "Ground") {
						focusedBuilding = hit.transform.gameObject;
						Debug.Log ("pepe");
						dragging = focusedBuilding.name == "Ground" && !isFocused;
						isFocused = !dragging;
						if(focusedBuilding.name != "Ground"){
						this.initialCameraPosition = transform.position;
						this.initialCameraRotation = transform.rotation;
						}
					}
				}
			} else {
                print("Clicked on UI");
                return;
            }
        } 

        if (Input.GetMouseButtonUp(0))
        {
            print("Stopped dragging");
            dragging = false;
        }

		if (focusedBuilding != null) {
			if (focusedBuilding.name == "Ground") {
				transform.position = Vector3.MoveTowards (transform.position, this.initialCameraPosition, SPEED);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, this.initialCameraRotation, SPEED);
			} else {
				transform.position = Vector3.MoveTowards (transform.position, focusedBuilding.transform.position + new Vector3 (0, 30, -30), SPEED);
				transform.rotation = Quaternion.RotateTowards (transform.rotation,  Quaternion.Euler(45,0,0), SPEED);
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
            
            // Set the transformation of the root object to give impression of camera movement
            sceneRoot.gameObject.transform.position = previousSceneRootPos + move;

            return;
        }

        Ray ray2 = camera.ScreenPointToRay(Input.mousePosition);
        float zoomDistance = zoomSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        camera.transform.Translate(ray2.direction * zoomDistance, Space.World);
    }
}
