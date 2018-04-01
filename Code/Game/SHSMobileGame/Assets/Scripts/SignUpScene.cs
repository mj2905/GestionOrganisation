using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignUpScene : MonoBehaviour {

	public InputField eMail;
	public InputField password;
	public InputField pseudo;
	public PopupScript popup;

	public Button[] teams;

	private Color[] normalColors;
	private string eMailText = "";
	private string passwordText = "";
	private string pseudoText = "";
	private int teamNumber = -1;

	public Button signup;

	private void SwitchSignInButtonActivation() {
		signup.interactable = eMailText.Length > 0 && passwordText.Length > 0 && pseudoText.Length > 0 && teamNumber != -1;
	}

	void Awake ()
	{
		SwitchSignInButtonActivation ();

		normalColors = new Color[teams.Length];
		for (int i = 0; i < teams.Length; ++i) {
			ColorBlock cb = teams [i].colors;
			normalColors[i] = cb.normalColor;
		}

		GameObject clone = (GameObject)Instantiate(Resources.Load("Popup"));
		popup = clone.GetComponent<PopupScript>();		
		popup.transform.SetParent (this.transform.parent,false);
		popup.transform.SetAsLastSibling ();
	}

	public void UpdateMail ()
	{
		this.eMailText = eMail.text;
		SwitchSignInButtonActivation ();
	}

	public void UpdatePassword ()
	{
		this.passwordText = password.text;
		SwitchSignInButtonActivation ();
	}

	public void UpdatePseudo ()
	{
		this.pseudoText = pseudo.text;
		SwitchSignInButtonActivation ();
	}

	public void UpdateTeam(int teamNumber)
	{
		print (teamNumber);
		ResetCurrentTeam ();
		HighlightTeam (teamNumber);
		this.teamNumber = teamNumber;
		SwitchSignInButtonActivation ();
	}

	private void ResetCurrentTeam(){
		for (int i = 0; i < teams.Length; ++i) {
			ColorBlock cb = teams [i].colors;
			cb.normalColor = normalColors[i];
			teams [i].colors = cb;
		}
	}

	private void HighlightTeam (int teamNumber){
		ColorBlock cb = teams [teamNumber - 1].colors;
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
		} else if (pseudoText == string.Empty) {
			popup.SetText ("The pseudo must be non empty.");
		}else {
			popup.SetText ("Signing up...");
			FirebaseManager.SignUp (eMailText, Crypto.hash(eMailText, passwordText), pseudoText ,teamNumber,popup);
		}
	}
}
