using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
	public GameManager gameManager;
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
			gameManager.AddTerminal(currentZone.name,gameObject.transform.localPosition.x,gameObject.transform.localPosition.z);
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