using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class InitialScene : MonoBehaviour
{
	public InputField eMail;
	public InputField password;
	public PopupScript popup;
	public CanvasRenderer spinning;
	public Text connecting;

	public Button signUp;
	public Text noAccountText;
	public Button signIn;

	private string eMailText = "";
	private string passwordText = "";

	private readonly bool countdownEnabled = false;

	private void SwitchSignInButtonActivation() {
		signIn.interactable = eMailText.Length > 0 && passwordText.Length > 0;
	}

	public void showElements(bool show) {

		eMail.gameObject.SetActive (show);
		password.gameObject.SetActive (show);
		signIn.gameObject.SetActive (show);
		noAccountText.gameObject.SetActive (show);
		signUp.gameObject.SetActive (show);

		connecting.gameObject.SetActive (!show);
		spinning.gameObject.SetActive (!show);
	}

	void Awake ()
	{
		if (!Persistency.IsTutorialPassed ()) {
			Persistency.TutorialPassed ();
			SceneManager.LoadScene("TutorialScene");
		}

		FirebaseManager.InitializeFirebase (this);
		FirebaseManager.GetServerTime ();

		if (countdownEnabled && (Countdown.start - DateTime.UtcNow).TotalSeconds >= 0) {
			SceneManager.LoadScene("FinalCountdown");
		}


		showElements (true);
		SwitchSignInButtonActivation ();

		if (Persistency.Exists ()) {
			showElements (false);

			Persistency.Session session = Persistency.Read ();
			FirebaseManager.SignIn (session.username, session.password, popup, () => showElements(true));
		}
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

	public void SignIn()
	{
		if (eMailText == string.Empty) {
			popup.SetText ("No e-mail entered.");
		} else if (passwordText == string.Empty) {
			popup.SetText ("No password entered.");
		} else {
			popup.SetText ("Signing in...");
			FirebaseManager.SignIn (eMailText, Crypto.hash(eMailText, passwordText), popup, () => showElements(true));
		}
	}

	public void SignUp()
	{
		SceneManager.LoadScene (1);
	}
}