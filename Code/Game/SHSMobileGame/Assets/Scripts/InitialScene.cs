using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitialScene : MonoBehaviour
{
	public InputField eMail;
	public InputField password;
	public PopupScript popup;
	public CanvasRenderer spinning;
	public Text connecting;

	public Button signUp;

	public Button signIn;

	string eMailText = "";
	string passwordText = "";

	public void showElements(bool show) {

		eMail.gameObject.SetActive (show);
		password.gameObject.SetActive (show);
		signIn.gameObject.SetActive (show);
		signUp.gameObject.SetActive (show);

		connecting.gameObject.SetActive (!show);
		spinning.gameObject.SetActive (!show);
	}

	void Awake ()
	{
		showElements (true);
		FirebaseManager.InitializeFirebase (this);
		if (Persistency.Exists ()) {
			showElements (false);

			Persistency.Session session = Persistency.Read ();
			FirebaseManager.SignIn (session.username, session.password, popup, () => showElements(true));
		}
	}

	public void UpdateMail ()
	{
		this.eMailText = eMail.text;
	}

	public void UpdatePassword ()
	{
		this.passwordText = password.text;
	}

	public void SignIn()
	{
		if (eMailText == string.Empty) {
			popup.SetText ("No e-mail entered.");
		} else if (passwordText == string.Empty) {
			popup.SetText ("No password entered.");
		} else {
			popup.SetText ("Signing in...");
			FirebaseManager.SignIn (eMailText, passwordText, popup, () => showElements(true));
		}
	}

	public void SignUp()
	{
		SceneManager.LoadScene (1);
	}
}