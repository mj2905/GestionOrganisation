using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
	public GameManager gameManager;
	public Terminal terminalPrefab;
	public GameObject sceneRoot;
	private GameObject currentZone;    

    void Start()
    {
		currentZone = null;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        gameObject.transform.position += movement * speed;

		if (Input.GetKeyDown (KeyCode.T)) {
			if (currentZone != null) {

				Terminal t = (Terminal)Instantiate (
					            terminalPrefab,
								gameObject.transform.position + new Vector3(0,2,0),
					            new Quaternion (),
					            sceneRoot.gameObject.transform);

				t.target = currentZone;
				t.Init ();
			}
		}
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
}