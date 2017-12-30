using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;



public class FirebaseManager
{
	public static Firebase.Auth.FirebaseAuth auth;
	public static Firebase.Database.DatabaseReference reference;
	public static Firebase.Auth.FirebaseUser user;
	public static int userTeam;

	private static GameManager gameManager;

	public static void InitializeFirebase(object sender) {
		Debug.Log("Setting up Firebase Auth");
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://shsorga.firebaseio.com/");
		reference = FirebaseDatabase.DefaultInstance.RootReference;
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		AuthStateChanged(sender, null);
	}

	// Track state changes of the auth object.
	static void AuthStateChanged(object sender, System.EventArgs eventArgs) {
		if (auth.CurrentUser != user) {
			bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
			if (!signedIn && user != null) {
				Debug.Log("Signed out " + user.UserId);
			}
			user = auth.CurrentUser;
			if (signedIn) {
				Debug.Log("Signed in " + user.UserId);
			}
		}
	}

	void OnDestroy() {
		auth.StateChanged -= AuthStateChanged;
		auth = null;
	}

	public static void SetMainGameRef(GameManager gameManager){
		FirebaseManager.gameManager = gameManager;
	}

	public static void SetListenerCreditScore(){
		reference.Child("Users").Child (FirebaseManager.user.UserId).ValueChanged += HandleScoreCreditChanged;
	}

	public static void SetListenerGame(){
		reference.Child("Game").ValueChanged += HandleGameChanged;
	}


	private static void HandleScoreCreditChanged (object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null) {
			Debug.LogError (args.DatabaseError.Message);
			return;
		}
		DataSnapshot snapshot = args.Snapshot;
		// Do something with snapshot...
		if (snapshot != null) {
			object credit = snapshot.Child("credits").Value;
			object xp = snapshot.Child("xp").Value;
		
			if (credit != null && xp != null) {
				gameManager.UpdateScoreAndCredit (xp.ToString(),credit.ToString());
			}
		}
	}

	private static void HandleGameChanged (object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null) {
			Debug.LogError (args.DatabaseError.Message);
			return;
		}
		DataSnapshot snapshot = args.Snapshot;
		// Do something with snapshot...
		if (snapshot != null) {
			System.Object terminalsObject = snapshot.Child ("Terminals").Value;
			System.Object zonesObject = snapshot.Child ("Zones").Value;
			System.Object teamsObject = snapshot.Child ("Teams").Value;
			gameManager.ChangeGame (new Game (terminalsObject, zonesObject, teamsObject));
		}
	}

	public static void SignIn (string eMailText, string passwordText, int nextSceneNumber)
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

			reference.Child("Users").Child(newUser.UserId).GetValueAsync().ContinueWith(
				task2 => {
					if (task2.IsFaulted) {
						// Handle the error...
					}
					else if (task2.IsCompleted) {
						DataSnapshot snapshot = task2.Result;
						if(snapshot != null){
							FirebaseManager.userTeam = Int32.Parse(snapshot.Child("team").Value.ToString());
							SceneManager.LoadScene(nextSceneNumber);
						}
					}
				});
		});
	}

	public static void SignUp (string eMailText, string passwordText,int teamNumber, int nextSceneNumber)
	{
		FirebaseManager.auth.CreateUserWithEmailAndPasswordAsync (eMailText, passwordText).ContinueWith (task => {
			if (task.IsCanceled) {
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}
			// Firebase user has been created.
			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat ("Firebase user created successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);
			CreateNewUser(newUser.UserId,teamNumber);
			SceneManager.LoadScene(nextSceneNumber);
			//sendVerificationMail (newUser);
		});
	}

	private static void CreateNewUser(string userId,int teamNumber){
		User user = new User(teamNumber);
		string json = JsonUtility.ToJson(user);
		reference.Child("Users").Child(userId).SetRawJsonValueAsync(json);
	}

	public static void AddTerminal(Terminal terminal){
		string json = JsonUtility.ToJson(terminal);
		reference.Child("Game/Terminals").Child(terminal.GetTerminalId()).SetRawJsonValueAsync(json);
	}
}