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

	public Button signIn;

	string eMailText = "";
	string passwordText = "";

	void Awake ()
	{
		FirebaseManager.InitializeFirebase (this);
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
			FirebaseManager.SignIn (eMailText, passwordText, popup);
		}
	}

	public void SignUp()
	{
		SceneManager.LoadScene (1);
	}
}