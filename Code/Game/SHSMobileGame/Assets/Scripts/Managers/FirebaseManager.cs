using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class FirebaseManager
{
	public static Firebase.Auth.FirebaseAuth auth;
	public static Firebase.Database.DatabaseReference reference;
	public static Firebase.Auth.FirebaseUser user;
	public static int userTeam = -1;

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
		reference.Child("Users").Child (FirebaseManager.user.UserId).ValueChanged += HandleUserChanged;
	}

	public static void SetListenerGame(){
		reference.Child("Game").ValueChanged += HandleGameChanged;
	}


	private static void HandleUserChanged (object sender, ValueChangedEventArgs args)
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
			object level = snapshot.Child("level").Value;
			Effects effects;
			Statistics statistics;

			if (snapshot.HasChild ("effects")) {
				effects = new Effects (snapshot.Child ("effects").Value);
			} else {
				effects = new Effects (null);
			}

			if (snapshot.HasChild ("stat")) {
				statistics = new Statistics (snapshot.Child ("stat").Value);
			} else {
				statistics = new Statistics (null);
			}

			if (credit != null && xp != null && level != null) {
				gameManager.UpdateUserStat (xp.ToString (), credit.ToString (), level.ToString (), effects,statistics);
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

	public static void SignIn (string eMailText, string passwordText, PopupScript popup, Action executeWhenFails)
	{
		FirebaseManager.auth.SignInWithEmailAndPasswordAsync (eMailText, passwordText).ContinueWith (task => {

			if (task.IsCanceled) {
				executeWhenFails();
				Debug.LogError ("SignInWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				executeWhenFails();
				Debug.LogError ("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				popup.SetText(((FirebaseException)task.Exception.InnerExceptions[0]).Message.ToString());
				return;
			}

			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat ("User signed in successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);
			Persistency.Write(eMailText, passwordText);

			reference.Child("Users").Child(newUser.UserId).GetValueAsync().ContinueWith(
				task2 => {
					if (task2.IsFaulted) {
						executeWhenFails();
						Debug.LogError ("Faulted");
						popup.SetText(((FirebaseException)task.Exception.InnerExceptions[0]).Message.ToString());
					}
					else if (task2.IsCompleted) {
						DataSnapshot snapshot = task2.Result;
						if(snapshot != null){
							FirebaseManager.userTeam = Int32.Parse(snapshot.Child("team").Value.ToString());
							SceneManager.LoadScene(2);
						}
						else {
							executeWhenFails();
							Debug.LogError ("Corrupted account, please contact the coding team");
							popup.SetText("Corrupted account, please contact the coding team");
						}
					}
				});
		});
	}

	public static void SignUp (string eMailText, string passwordText,int teamNumber, PopupScript popup)
	{
		FirebaseManager.auth.CreateUserWithEmailAndPasswordAsync (eMailText, passwordText).ContinueWith (task => {
			if (task.IsCanceled) {
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError ("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				popup.SetText(((FirebaseException)task.Exception.InnerExceptions[0]).Message.ToString());
				return;
			}

			// Firebase user has been created.
			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat ("Firebase user created successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);
			Persistency.Write(eMailText, passwordText);

			CreateNewUser(newUser.UserId,teamNumber);
			SceneManager.LoadScene(0);
			//sendVerificationMail (newUser);
		});
	}

	public static void Logout() {
		FirebaseManager.auth.SignOut (); //always succeeds
	}

	private static void CreateNewUser(string userId,int teamNumber){
		User user = new User(teamNumber);
		string json = JsonUtility.ToJson(user);
		reference.Child("Users").Child(userId).SetRawJsonValueAsync(json);
	}

	private static Func<MutableData, TransactionResult> AddMedalTransaction(Medal m) 
	{
		return mutableData => {
				mutableData.Child (FirebaseManager.user.UserId + "/queue/").Value = m.ToMap();
				return TransactionResult.Success(mutableData);
		};
	}

	public static void AddMedal(Medal medal){
		reference.Child ("Users/").RunTransaction (AddMedalTransaction (medal)).ContinueWith (task => {
		});
	}

	public static Func<MutableData, TransactionResult> AddTerminalTransaction(Terminal t) 
	{
		return mutableData => {
			Assert.AreNotEqual (0, FirebaseManager.userTeam, "Team not set, or error with team numbers (0 was thought as no team)");

			object token_obtained = mutableData.Child ("Teams/" + FirebaseManager.userTeam + "/token").Value;
			Assert.AreNotEqual(token_obtained, null);
			long token_value = (long)token_obtained;

			if (token_value <= 0) {
				return TransactionResult.Abort ();
			} else {
				mutableData.Child ("Teams/" + FirebaseManager.userTeam + "/token").Value = token_value-1;
				mutableData.Child("Terminals/").Child(t.GetTerminalId()).Value = t.ToMap();
				return TransactionResult.Success(mutableData);
			}
		};
	}

	private static Func<MutableData, TransactionResult> UpdateCreditTransaction(long amount){

		return mutableData => {

			object credits_obtained = mutableData.Value;

			if(credits_obtained != null) {

				//Cap credits at max value
				long credit_value = (long)credits_obtained + amount;//Math.Min((long)credits_obtained + amount, User.MAX_CREDITS);

				//The transaction should not go through if the number of credits goes below the minimum threshold
				if(User.MIN_CREDITS <= credit_value){
					mutableData.Value = credit_value;
					return TransactionResult.Success(mutableData);
				}
			}

			return TransactionResult.Abort();
		};
	}

	private static Func<MutableData, TransactionResult> HurtTerminalTransaction(long amount) 
	{
		return mutableData => {

			object health_obtained = mutableData.Value;

			if(health_obtained != null) {

				long health_value = (long)health_obtained;

				if(health_value > 0) {

					mutableData.Value = Math.Max(0, health_value - amount);
					return TransactionResult.Success(mutableData);

				}
			}

			return TransactionResult.Abort();
		};
	}

	private static Func<MutableData, TransactionResult> BuffTerminalTransaction(long amount) 
	{
		return mutableData => {

			object strength_obtained = mutableData.Value;

			if(strength_obtained != null) {

				long strength_value = (long)strength_obtained;

				if(strength_value < QuantitiesConstants.STRENGTH_MAX) {

					mutableData.Value = Math.Min(QuantitiesConstants.STRENGTH_MAX, strength_value + amount);
					return TransactionResult.Success(mutableData);
				}
			}

			return TransactionResult.Abort();
		};
	}

	private static Func<MutableData, TransactionResult> HealZoneTransaction(long amount) 
	{
		return mutableData => {

			object health_obtained = mutableData.Child ("health").Value;
			object level_obtained = mutableData.Child ("level").Value;

			if(health_obtained != null && level_obtained != null) {

				long health_value = (long)health_obtained;
				long max_health_value = (long)QuantitiesConstants.ZONE_MAX_HEALTH_VALUES[(int)level_obtained];

				if(health_value < max_health_value) {
					mutableData.Child("health").Value = Math.Min(max_health_value, health_value + amount);

					return TransactionResult.Success(mutableData);
				}
			}

			return TransactionResult.Abort();
		};
	}

	private static Func<MutableData, TransactionResult> ImproveZoneTransaction(int level) 
	{
		return mutableData => {

			object level_obtained = mutableData.Child("level").Value;
			object health_obtained = mutableData.Child("health").Value;

			if(level_obtained != null && health_obtained != null) {

				long level_value = (long)level_obtained;
				long health_value = (long)health_obtained;

				if(health_value > 0 && level_value == level && level < QuantitiesConstants.ZONE_LEVEL_MAX) {

					mutableData.Child("level").Value = level+1;
					mutableData.Child("health").Value = QuantitiesConstants.ZONE_MAX_HEALTH_VALUES[level+1];

					return TransactionResult.Success(mutableData);
				}
			}

			return TransactionResult.Abort();
		};
	}

	private static Func<MutableData, TransactionResult> ImproveTerminalTransaction(int level) 
	{
		return mutableData => {

			object level_obtained = mutableData.Child("level").Value;
			object health_obtained = mutableData.Child("hp").Value;

			if(level_obtained != null && health_obtained != null) {

				long level_value = (long)level_obtained;
				long health_value = (long)health_obtained;

				if(health_value > 0 && level_value == level && level < QuantitiesConstants.TERMINAL_LEVEL_MAX) {

					mutableData.Child("level").Value = level+1;
					mutableData.Child("hp").Value = QuantitiesConstants.TERMINAL_MAX_HEALTH_VALUES[level+1];

					return TransactionResult.Success(mutableData);
				}
			}

			return TransactionResult.Abort();
		};
	}

	public static void AddTerminal(Terminal terminal, PopupScript messagePopup){
		reference.Child ("Game").RunTransaction (AddTerminalTransaction (terminal)).ContinueWith(task => {
			if (task.Exception != null) {
				messagePopup.SetText("Not enough tokens for the team");
			} else{
				AddTerminalStat();
			}
		});
	}

	public static void HurtTerminal(string terminalID, long amount, PopupScript messagePopup){

		reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.TERMINAL_SMASH_COST)).ContinueWith (task => {

			if (task.Exception == null) {

				reference.Child ("Game/Terminals/").Child(terminalID).Child("hp").RunTransaction (HurtTerminalTransaction (amount)).ContinueWith(innerTask => {

					if (innerTask.Exception != null) {
						messagePopup.SetText("This terminal is already dead");

						reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (-QuantitiesConstants.TERMINAL_SMASH_COST));
					}else{
						AddTerminalDamagedStat();
					}
				});
			} else {
				Debug.Log("Could not deduct enough credit to smash the terminal");
				messagePopup.SetText("You don't have enough credits to smash the terminal");
			}
		});
	}

	public static void BuffTerminal(string terminalID, long amount, PopupScript messagePopup){
		reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.TERMINAL_BUFF_COST)).ContinueWith (task => {

			if (task.Exception == null) {
				reference.Child("Game/Terminals/").Child(terminalID).Child("strength").RunTransaction(BuffTerminalTransaction(amount)).ContinueWith(innerTask =>{
					if (innerTask.Exception != null) {
						reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (-QuantitiesConstants.TERMINAL_BUFF_COST));
						messagePopup.SetText("This terminal is already maximally buffed");
					}else{
						AddTerminalBuffedStat();
					}
				});
			} else {
				Debug.Log("Could not deduct enough credit to buff the terminal");
				messagePopup.SetText("You don't have enough credits to buff the terminal");
			}
		});
	}

	public static void HealZone(string zoneID, long amount, PopupScript messagePopup){
		reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.ZONE_HEAL_COST)).ContinueWith (task => {

			if (task.Exception == null) {
				reference.Child ("Game/Zones/").Child (zoneID).RunTransaction (HealZoneTransaction (amount)).ContinueWith (innerTask => {
					
					if (innerTask.Exception != null) {
						reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (-QuantitiesConstants.ZONE_HEAL_COST));
						messagePopup.SetText("This zone is already at maximum health");
					} else{
						AddZoneHealStat();
					}
				});
			} else {
				Debug.Log("Could not deduct enough credit to heal the zone");
				messagePopup.SetText("You don't have enough credits to heal the zone");
			}
		});
	}

	public static void ImproveZone (string zoneID, int level, PopupScript messagePopup)
	{

		if (level >= 0 && level + 1 < QuantitiesConstants.ZONE_MAX_HEALTH_VALUES.Length) {
			reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (-QuantitiesConstants.ZONE_MAX_HEALTH_COST [level + 1])).ContinueWith (task3 => {
				if (task3.Exception == null) {
					reference.Child ("Game/Zones/").Child (zoneID).RunTransaction (ImproveZoneTransaction (level)).ContinueWith (innerTask => {
						if (innerTask.Exception != null) {
							messagePopup.SetText ("This zone has been improved by someone else");
							reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.ZONE_MAX_HEALTH_COST [level + 1]));
						}
					});
				} else {
					Debug.Log ("Could not deduct enough credit to improve the zone");
					messagePopup.SetText ("You don't have enough credits to improve the zone");
				}
			});
		} else {
			messagePopup.SetText ("Maximal level reached for that zone");
		}
	}
		
	public static void ImproveTerminal (string zoneID, int level, PopupScript messagePopup)
	{

		if (level >= 0 && level + 1 < QuantitiesConstants.TERMINAL_MAX_HEALTH_VALUES.Length) {
			reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (-QuantitiesConstants.TERMINAL_MAX_HEALTH_COST [level + 1])).ContinueWith (task3 => {
				if (task3.Exception == null) {
					reference.Child ("Game/Zones/").Child (zoneID).RunTransaction (ImproveZoneTransaction (level)).ContinueWith (innerTask => {
						if (innerTask.Exception != null) {
							messagePopup.SetText ("This terminal has been improved by someone else");
							reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.TERMINAL_MAX_HEALTH_COST [level + 1]));
						}
					});
				} else {
					Debug.Log ("Could not deduct enough credit to improve the terminal");
					messagePopup.SetText ("You don't have enough credits to improve the terminal");
				}
			});
		} else {
			messagePopup.SetText ("Maximal level reached for that terminal");
		}
	}


	private static Func<MutableData, TransactionResult> AddTerminalStatTransaction() 
	{
		return mutableData => {

			object numberOfTerminal = mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalPlaced").Value;

			if(numberOfTerminal == null){
				mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalPlaced").Value = 1;
			} else{
				long number = (long)numberOfTerminal;
				mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalPlaced").Value = number + 1;

			}
			return TransactionResult.Success(mutableData);
		};
	}

	public static void AddTerminalStat(){
		reference.Child ("Users/").RunTransaction (AddTerminalStatTransaction ()).ContinueWith (task => {
		});
	}


	private static Func<MutableData, TransactionResult> AddBuffedTerminalsStatTransaction() 
	{
		return mutableData => {

			object numberOfTerminal = mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalBuffed").Value;

			if(numberOfTerminal == null){
				mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalBuffed").Value = 1;
			} else{
				long number = (long)numberOfTerminal;
				mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalBuffed").Value = number + 1;

			}
			return TransactionResult.Success(mutableData);
		};
	}

	public static void AddTerminalBuffedStat(){
		reference.Child ("Users/").RunTransaction (AddBuffedTerminalsStatTransaction ()).ContinueWith (task => {
		});
	}


	private static Func<MutableData, TransactionResult> AddDamagedTerminalsStatTransaction() 
	{
		return mutableData => {

			object numberOfTerminal = mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalDamaged").Value;

			if(numberOfTerminal == null){
				mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalDamaged").Value = 1;
			} else{
				long number = (long)numberOfTerminal;
				mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfTerminalDamaged").Value = number + 1;
			}
			return TransactionResult.Success(mutableData);
		};
	}

	public static void AddTerminalDamagedStat(){
		reference.Child ("Users/").RunTransaction (AddDamagedTerminalsStatTransaction ()).ContinueWith (task => {
		});
	}

	private static Func<MutableData, TransactionResult> AddZoneHealStatTransaction() 
	{
		return mutableData => {

			object numberOfTerminal = mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfZoneHeal").Value;

			if(numberOfTerminal == null){
				mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfZoneHeal").Value = 1;
			} else{
				long number = (long)numberOfTerminal;
				mutableData.Child (FirebaseManager.user.UserId + "/stat/numberOfZoneHeal").Value = number + 1;
			}
			return TransactionResult.Success(mutableData);
		};
	}

	public static void AddZoneHealStat(){
		reference.Child ("Users/").RunTransaction (AddDamagedTerminalsStatTransaction ()).ContinueWith (task => {
		});
	}
}