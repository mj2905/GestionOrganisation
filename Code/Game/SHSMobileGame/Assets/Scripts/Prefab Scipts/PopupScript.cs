using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PopupScript : MonoBehaviour {
	public Text text;
	private Image button;
	private float opacity;
	private float counter;
	private float displayingTime = 3f;

	private enum States {Poping,Depoping,Displaying,IDLE};
	private States state;

	void Start () {
		button = GetComponent<Image>();

		print ("Starting popup script");
		print ("Components in children: " + GetComponentsInChildren<Text> ().Length);
		opacity = 0;
		counter = 0;
		state = States.IDLE;
		setOpacity ();
	}

	public void SetText(string textPopUp){
		text.text = textPopUp;
		state = States.Poping;
		gameObject.SetActive (true);
	}
		
	void Update () {
		switch (state) {
		case States.Poping:
			print ("poping");
			if (opacity > 0.9) {
				state = States.Displaying;
				counter = 0;
			} else {
				opacity += 0.9f * Time.deltaTime;
			}
			break;
		case States.Depoping:
			opacity -= 0.4f * Time.deltaTime;
			if (opacity < 0) {
				state = States.IDLE;
			}
			break;
		case States.Displaying:
			counter += Time.deltaTime;
			if (counter > displayingTime) {
				state = States.Depoping;
			}
			break;
		case States.IDLE:
			gameObject.SetActive (false);
			break;
		default:
			break;
		}
		setOpacity ();
	}

	private void setOpacity(){
		Color buttonColor = button.color;
		Color textColor = text.color;

		GetComponent<Image> ().color = new Color (buttonColor.r, buttonColor.g, buttonColor.b, opacity);
		GetComponentsInChildren<Text> () [0].color = new Color (textColor.r, textColor.g, textColor.b, opacity);
	}
}

