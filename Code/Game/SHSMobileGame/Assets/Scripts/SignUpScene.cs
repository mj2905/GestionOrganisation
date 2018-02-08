using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignUpScene : MonoBehaviour {

	public InputField eMail;
	public InputField password;
	public PopupScript popup;

	public Button[] teams;

	private Color currentNormalColor;
	private string eMailText = "";
	private string passwordText = "";
	private int teamNumber = -1;
	void Awake ()
	{
		GameObject clone = (GameObject)Instantiate(Resources.Load("Popup"));
		popup = clone.GetComponent<PopupScript>();		
		popup.transform.SetParent (this.transform.parent,false);
		popup.transform.SetAsLastSibling ();
	}

	public void UpdateMail ()
	{
		this.eMailText = eMail.text;
	}

	public void UpdatePassword ()
	{
		this.passwordText = password.text;
	}

	public void UpdateTeam(int teamNumber)
	{
		ResetCurrentTeam ();
		HighlightTeam (teamNumber);
		this.teamNumber = teamNumber;
	}

	private void ResetCurrentTeam(){
		if (this.teamNumber > 0) {
			ColorBlock cb = teams [this.teamNumber - 1].colors;
			cb.normalColor = currentNormalColor;
			teams [this.teamNumber - 1].colors = cb;
		}
	}

	private void HighlightTeam (int teamNumber){
		ColorBlock cb = teams [teamNumber - 1].colors;
		currentNormalColor = cb.normalColor;
		cb.normalColor = cb.highlightedColor;
		teams [teamNumber - 1].colors = cb;
	}


	public void LoadScene(int sceneNumber)
	{
		SceneManager.LoadScene(sceneNumber);
	}

	public void SignUp()
	{
		if (eMailText == string.Empty) {
			popup.SetText ("No e-mail entered.");
		} else if (passwordText == string.Empty) {
			popup.SetText ("No password entered.");
		} else if (teamNumber == -1) {
			popup.SetText ("No team selected.");
		} else if (passwordText.Length < 6) {
			popup.SetText ("The password must be at least of length 6.");
		} else {
			popup.SetText ("Signing up...");
			FirebaseManager.SignUp (eMailText, passwordText,teamNumber,popup);
		}
	}
}
