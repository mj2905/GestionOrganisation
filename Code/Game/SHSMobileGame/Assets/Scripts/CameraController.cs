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

	// Use this for initialization
	void Start () {
        dragOrigin = new Vector3(0.0f, 0.0f, 0.0f);
        dragging = false;
		this.sceneRoot = gameManager.sceneRoot;
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
                dragging = true;
                print("Started dragging");
                return;
            } else
            {
                print("Clicked on UI");
                return;
            }
        } 

        if (Input.GetMouseButtonUp(0))
        {
            print("Stopped dragging");
            dragging = false;
            return;
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

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        float zoomDistance = zoomSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        camera.transform.Translate(ray.direction * zoomDistance, Space.World);
    }
}
