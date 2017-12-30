using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitialScene : MonoBehaviour
{
	public InputField eMail;
	public InputField password;

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
		FirebaseManager.SignIn (eMailText, passwordText,2);
	}

	public void SignUp()
	{
		SceneManager.LoadScene (1);
	}
}