using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusScript : MonoBehaviour {

	private Button bonus;
	private float opacity;
	private float counter;

	private Click click;
	private float randomOffset;

	private enum States {Disappearing,Displaying,IDLE};
	private States state;

	float width;
	float height;

	// Use this for initialization
	void Start () {
		bonus = GetComponent<Button>();
		opacity = 0;
		counter = 0;
		setOpacity ();
		bonus.onClick.AddListener(TaskOnClick);
		SetNewRandomOffset ();

		width = GetComponent<RectTransform>().rect.width;
		height = GetComponent<RectTransform>().rect.height;

		state = States.IDLE;
	}

	public void setClick(Click click){
		this.click = click;
	}
		
	private void spawnBonus(){
		bonus.transform.position = new Vector2 (Random.Range(width,Screen.width-width), Random.Range(height,Screen.height-height));
		state = States.Displaying;
		opacity = 1;
		setOpacity ();
		SetNewRandomOffset ();
	}

	private void TaskOnClick()
	{
		if (state == States.Disappearing || state == States.Displaying) {
			opacity = 0;
			setOpacity ();
			click.applyBonus ();
		}
	}

	private void setOpacity ()
	{
		Color color = GetComponent<Image> ().color;
		GetComponent<Image> ().color = new Color (color.r, color.g, color.b, opacity);
	}

	private void SetNewRandomOffset(){
		randomOffset = Random.Range (0f, 5f);
	}

	void Update(){
		counter += Time.deltaTime;
		switch(state){
		case States.IDLE:
			if (counter > 0 + randomOffset) {
				counter = 0;
				spawnBonus ();
			}
			break;
		case States.Disappearing:
			opacity -= 0.4f * Time.deltaTime;
			setOpacity ();
			if (opacity < 0) {
				counter = 0;
				state = States.IDLE;
			}
			break;
		case States.Displaying:
			if (counter > 2) {
				counter = 0;
				state = States.Disappearing;
			}
			break;
		default:
			break;
		}
	}
}
