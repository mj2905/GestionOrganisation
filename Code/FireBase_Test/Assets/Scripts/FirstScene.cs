using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{
	public InputField eMail;
	public InputField password;

	public Button signIn;

	string eMailText = "";
	string passwordText = "";

	private PopupScript popup;

	void Awake ()
	{
		FirebaseManager.InitializeFirebase (this);
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

	public void SignUp ()
	{
		FirebaseManager.auth.CreateUserWithEmailAndPasswordAsync (eMailText, passwordText).ContinueWith (task => {
			if (task.IsCanceled) {
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				popup.PopUp("Error while creating the user");
				return;
			}
			// Firebase user has been created.
			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat ("Firebase user created successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);
			sendVerificationMail (newUser);
		});
	}
		
	public void SignIn (int sceneIndex)
	{
			FirebaseManager.auth.SignInWithEmailAndPasswordAsync (eMailText, passwordText).ContinueWith (task => {
				if (task.IsCanceled) {
					Debug.LogError ("SignInWithEmailAndPasswordAsync was canceled.");
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError ("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
					return;
				}

				Firebase.Auth.FirebaseUser newUser = task.Result;
				Debug.LogFormat ("User signed in successfully: {0} ({1})",
					newUser.DisplayName, newUser.UserId);
			if (newUser.IsEmailVerified) {
				SceneManager.LoadScene(sceneIndex);
			} else{
				popup.PopUp("You need to verify your account first. \n Go Check your mail please.");
			}
		});
	}

	private void sendVerificationMail (Firebase.Auth.FirebaseUser user)
	{
		if (user != null) {
			user.SendEmailVerificationAsync ().ContinueWith (task => {
				if (task.IsCanceled) {
					Debug.LogError ("SendEmailVerificationAsync was canceled.");
					return;
				}
				if (task.IsFaulted) {
					Debug.LogError ("SendEmailVerificationAsync encountered an error: " + task.Exception);
					return;
				}
				Debug.Log ("Email sent successfully.");
				popup.PopUp("A mail have been sent to verify your accout.");
			});
		}
	}
}
