using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ColorChangeScript : MonoBehaviour {


	Renderer thisRend; //Renderer of our Cube


	Text ra;


	float opacityIncrease = 0.02f;
	float opacityCurrent = 0.0f;

	Color color;

	int defaultColor = 1;

	void Start () {

		color = getColorForTeam (defaultColor);
		color.a = opacityCurrent;
		ra.text = "Text";
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			
			//Camera makes a ray to the screen point
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) {

				GameObject  game = GameObject.Find(hit.transform.name);

						
				if (game.name.Equals (gameObject.name)) {

			
					thisRend = gameObject.GetComponent<Renderer> ();

					if (opacityCurrent < .9) {

						opacityCurrent += opacityIncrease;
						color.a = opacityCurrent;
						thisRend.material.SetColor ("_Color", color);
				
					}
				}
		}
	}
	}
	//Retrieve
	Color getColorForTeam(int i){

		switch (i) {
		case 0:
			return Color.red;
		case 1:
			return Color.blue;
		case 2:
			return Color.yellow;
		case 3:
			return Color.green;
		default:
			return Color.white;
		}

	
	}

}




