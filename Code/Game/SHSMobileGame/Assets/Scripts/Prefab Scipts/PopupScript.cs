using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PopupScript : MonoBehaviour {
	private Image button;
	private Text text;
	private float opacity;
	private float counter;
	private float displayingTime = 3f;

	private enum States {Poping,Depoping,Displaying,IDLE};
	private States state;

	void Start () {
		button = GetComponent<Image>();
		text = GetComponentsInChildren<Text>()[0];

		opacity = 0;
		counter = 0;
		state = States.IDLE;
		setOpacity ();
	}

	public void SetText(string textPopUp){
		text.text = textPopUp;
		state = States.Poping;
	}
		
	void Update () {
		switch (state) {
		case States.Poping:
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

