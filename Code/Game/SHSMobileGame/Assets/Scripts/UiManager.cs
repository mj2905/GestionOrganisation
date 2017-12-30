using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : MonoBehaviour
{

	public Text creditText;
	public Text scoreText;

	private PopupScript popup;


	// Use this for initialization
	void Awake ()
	{
		GameObject clone = (GameObject)Instantiate(Resources.Load("Popup"));
		popup = clone.GetComponent<PopupScript>();		
		popup.transform.SetParent (this.transform.parent,false);
		popup.transform.SetAsLastSibling ();
	}

	public void UpdateScoreAndCredit(string xp, string credit){
		scoreText.text = "Xp: " + xp;
		creditText.text = "Credits: " + credit;
	}

	public void SetPopUpText(string text){
		popup.SetText (text);
	}
}
