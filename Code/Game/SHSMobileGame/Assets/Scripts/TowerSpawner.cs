using UnityEngine;
using UnityEngine.UI;

public class TowerSpawner : MonoBehaviour {

    public GameObject sceneRoot; 
    public GameObject uiElement;
    public Vector3 initialPosition;

    public GameObject towerPrefab;

    // Use this for initialization
    void Start () {
        initialPosition = uiElement.gameObject.transform.position;
	}

    public void onDrag()
    {
        uiElement.gameObject.transform.position = Input.mousePosition;

        towerPrefab.gameObject.transform.position = new Vector3(
        towerPrefab.gameObject.transform.position.x, 
        10.2f,
        towerPrefab.gameObject.transform.position.z);
    }

    public void onBeginDrag()
    {
        print("This is begin drag");
        Image im = uiElement.GetComponent<Image>();
        im.color = new Color(46/255.0f, 204/255.0f, 64/255.0f);
    }

    public void onEndDrag()
    {
        uiElement.gameObject.transform.position = initialPosition;
        Image im = uiElement.GetComponent<Image>();
        im.color = new Color(1,1,1);    

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            Instantiate(
            towerPrefab,
            (ray.origin + ray.direction * hit.distance) + new Vector3(0.0f,0.2f,0.0f),
            new Quaternion(),
            sceneRoot.gameObject.transform);
        }
    }


}
