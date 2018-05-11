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
	public static string userPseudo;

	public const long DEFAULT_TIME = 2800800;
	private static long timer = DEFAULT_TIME;
	private static bool timerSet = false;

	public const long DEFAULT_BEG_TIME = 2887200;
	private static long beg_timer = DEFAULT_BEG_TIME;
	private static bool beg_timerSet = false;

	private static GameManager gameManager;
	private static List<Barrier> glob_barrier = new List<Barrier>();


	public static void InitializeFirebase(object sender) {
		Debug.Log("Setting up Firebase Auth");
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://shsorga.firebaseio.com/");
		reference = FirebaseDatabase.DefaultInstance.RootReference;
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

		glob_barrier.Add(new Barrier ());
		Action<object, System.EventArgs> authStateChanged = AuthStateChanged(glob_barrier.Count - 1);

		auth.StateChanged += (obj, args) => {
			authStateChanged (obj, args);
		};
		authStateChanged(sender, null);
	}

	public static long GetServerTime() {
		return timer;
	}

	public static long GetBeginTime() {
		return beg_timer;
	}

	// Track state changes of the auth object.
	private static Action<object, System.EventArgs> AuthStateChanged (int c)
	{
		return (object sender, System.EventArgs eventArgs) => {
			if (!glob_barrier[c].barrier ()) {
				Debug.Log("auth state changed");
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
		};
	}

	private static void delete() {

		glob_barrier[glob_barrier.Count - 1].close ();

		auth = null;
	}

	void OnDestroy() {
		delete ();
	}

	public static void SetMainGameRef(GameManager gameManager){
		FirebaseManager.gameManager = gameManager;
	}
		
	public static void SetListenerCreditScore(){
		int c = glob_barrier.Count - 1;
		reference.Child ("Users").Child (FirebaseManager.user.UserId).ValueChanged += ((obj, args) => {
			HandleUserChanged (c) (obj, args);
		});
	}

	public static void SetListenerGame(){
		int c = glob_barrier.Count - 1;
		reference.Child ("Game").ValueChanged += ((obj, args) => {
			HandleGameChanged (c) (obj, args);
		});
	}

	public static void SetListenerEnd() {
		int c = glob_barrier.Count - 1;
		reference.Child("End").ValueChanged += ((obj, args) => {
			HandleEndChanged(c)(obj, args);
		});
	}

	public static void SetListenerTimer(){
		if (!timerSet) {
			reference.Child ("/timer/").ValueChanged += ((obj, args) => {
				if (args.DatabaseError != null) {
					Debug.LogError (args.DatabaseError.Message);
					return;
				} else {
					DataSnapshot snapshot = args.Snapshot;
					FirebaseManager.timer = Int64.Parse (snapshot.Value.ToString ());
				}
			});
			timerSet = true;
		}
	}

	public static void SetListenerBegTimer(){
		if (!beg_timerSet) {
			reference.Child ("/beg_time/").ValueChanged += ((obj, args) => {
				if (args.DatabaseError != null) {
					Debug.LogError (args.DatabaseError.Message);
					return;
				} else {
					DataSnapshot snapshot = args.Snapshot;
					FirebaseManager.beg_timer = Int64.Parse (snapshot.Value.ToString ());
				}
			});
			beg_timerSet = true;
		}
	}

	private static Action<object, ValueChangedEventArgs> HandleUserChanged (int c)
	{
		return (object sender, ValueChangedEventArgs args) => {
			if (!glob_barrier[c].barrier ()) {
				if (args.DatabaseError != null) {
					Debug.LogError (args.DatabaseError.Message);
					return;
				}
				DataSnapshot snapshot = args.Snapshot;
				// Do something with snapshot...
				if (snapshot != null) {
					object credit = snapshot.Child ("credits").Value;
					//FirebaseManager.userTeam = Int32.Parse(snapshot.Child("team").Value.ToString());
					object xp = snapshot.Child ("xp").Value;
					object level = snapshot.Child ("level").Value;
					Effects effects;
					Statistics statistics;
					SkinsInfo skins;

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

					if (snapshot.HasChild ("skins")) {
						skins = new SkinsInfo (snapshot.Child ("skins").Value);
					} else {
						skins = new SkinsInfo (null);
					}

					if (credit != null && xp != null && level != null) {
						gameManager.UpdateUserStat (xp.ToString (), credit.ToString (), FirebaseManager.userTeam, level.ToString (), effects, statistics,skins);
					}
				}
			}
		};
	}


	private static Action<object, ValueChangedEventArgs> HandleGameChanged (int c)
	{
		return (object sender, ValueChangedEventArgs args) => {
			if (!glob_barrier[c].barrier ()) {
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
					System.Object bestPlayersObject = snapshot.Child ("Best").Value;
					gameManager.ChangeGame (new Game (terminalsObject, zonesObject, teamsObject, bestPlayersObject));
				}
			}
		};
	}

	private static Action<object, ValueChangedEventArgs> HandleEndChanged (int c)
	{
		return (object sender, ValueChangedEventArgs args) => {
			if (!glob_barrier[c].barrier ()) {
				if (args == null) {
					return;
				} else if (args.DatabaseError != null) {
					Debug.LogError (args.DatabaseError.Message);
					return;
				}
				DataSnapshot snapshot = args.Snapshot;
				// Do something with snapshot...
				if (snapshot != null && snapshot.Value != null) {

					EndGameValues.SCORES = new Dictionary<int, ColorConstants.TEAMS> ();

					EndGameValues.SCORES.Add (Int32.Parse (snapshot.Child ("1/score").Value.ToString ()), ColorConstants.TEAMS.ENAC);

					EndGameValues.SCORES.Add (Int32.Parse (snapshot.Child ("2/score").Value.ToString ()), ColorConstants.TEAMS.STI);

					EndGameValues.SCORES.Add (Int32.Parse (snapshot.Child ("3/score").Value.ToString ()), ColorConstants.TEAMS.FSB);

					EndGameValues.SCORES.Add (Int32.Parse (snapshot.Child ("4/score").Value.ToString ()), ColorConstants.TEAMS.ICSV);

					SceneManager.LoadScene (3);

				}
			}
		};
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

			if(!newUser.IsEmailVerified) {
				executeWhenFails();
				popup.SetText("Please verify your account, by following the link your received by e-mail :)");
				return;
			}

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
							FirebaseManager.userPseudo = snapshot.Child("pseudo").Value.ToString();

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

	public static void SignUp (string eMailText, string passwordText, string pseudoText,int teamNumber, PopupScript popup)
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

			CreateNewUser(newUser.UserId,teamNumber, pseudoText);
			newUser.SendEmailVerificationAsync();
			SceneManager.LoadScene(0);
		});
	}

	public static void Logout() {
		FirebaseManager.auth.SignOut (); //always succeeds
		delete();
	}

	private static void CreateNewUser(string userId,int teamNumber, string pseudo){
		User user = new User(teamNumber, pseudo,userId, QuantitiesConstants.DEFAULT_CREDITS_NEW_PLAYER);
		string json = JsonUtility.ToJson(user);
		reference.Child("Users").Child(userId).SetRawJsonValueAsync(json);
	}

	private static Func<MutableData, TransactionResult> AddMedalTransaction(MedalInfo m) 
	{
		return mutableData => {
				mutableData.Child (FirebaseManager.user.UserId + "/effects/").Value = m.ToMap();
				return TransactionResult.Success(mutableData);
		};
	}

	public static void AddMedal(MedalInfo medal){
		reference.Child ("Users/").RunTransaction (AddMedalTransaction (medal)).ContinueWith (task => {
		});
	}
		
	public static void AddBestPlayerAchivement(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (AddBestPlayerTransation ()).ContinueWith (task => {
		});
	}

	public static void AddTop5PlayerAchivement(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (AddTop5PlayerTransation ()).ContinueWith (task => {
		});
	}

	public static void AddAllAchivement(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (AddAllTransation ()).ContinueWith (task => {
		});
	}

	private static Func<MutableData, TransactionResult> AddBestPlayerTransation(){
		return mutableData => {
			object top5Player = mutableData.Child ("/effects/top5PlayerAchievement").Value;
			object bestPlayer = mutableData.Child ("/effects/bestPlayerAchievement").Value;

			if(top5Player == null){
				mutableData.Child ("/effects/top5PlayerAchievement").Value = EffectsConstants.top5PlayerAchievement.ToMap();
				object xp = mutableData.Child("xp").Value;
				if(xp != null){
					mutableData.Child("xp").Value = (long)xp + (long)EffectsConstants.top5PlayerAchievementXp;
				}

			}
			if(bestPlayer == null){
				mutableData.Child ("/effects/bestPlayerAchievement").Value = EffectsConstants.bestPlayerAchievement.ToMap();

				object xp = mutableData.Child("xp").Value;
				if(xp != null){
					mutableData.Child("xp").Value = (long)xp + (long)EffectsConstants.bestPlayerAchievementXp;
				}

				gameManager.setAchievement("bestPlayerAchievement");
			}
			return TransactionResult.Success(mutableData);
		};
	}

	private static Func<MutableData, TransactionResult> AddTop5PlayerTransation(){
		return mutableData => {
			object top5Player = mutableData.Child ("/effects/top5PlayerAchievement").Value;

			if(top5Player == null){
				mutableData.Child ("/effects/top5PlayerAchievement").Value = EffectsConstants.top5PlayerAchievement.ToMap();

				object xp = mutableData.Child("xp").Value;
				if(xp != null){
					mutableData.Child("xp").Value = (long)xp + (long)EffectsConstants.top5PlayerAchievementXp;
				}

				gameManager.setAchievement("top5PlayerAchievement");
			}
			return TransactionResult.Success(mutableData);
		};
	}

	private static Func<MutableData, TransactionResult> AddAllTransation(){
		return mutableData => {
			object allAchievement = mutableData.Child ("/effects/allAchievement").Value;

			if(allAchievement == null){
				mutableData.Child ("/effects/allAchievement").Value = EffectsConstants.allAchievement.ToMap();

				object xp = mutableData.Child("xp").Value;
				if(xp != null){
					mutableData.Child("xp").Value = (long)xp + (long)EffectsConstants.allAchievementXp;
				}

				gameManager.setAchievement("allAchievement");
			}
			return TransactionResult.Success(mutableData);
		};
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

	private static Func<MutableData, TransactionResult> HurtTerminalTransaction(long amount, int userLevel) 
	{
		return mutableData => {

			object health_obtained = mutableData.Value;

			if(health_obtained != null) {

				long realAmount = amount + QuantitiesConstants.TERMINAL_SMASH_LEVEL_BONUS[userLevel];
				long health_value = (long)health_obtained;

				if(health_value > 0) {

					mutableData.Value = Math.Max(0, health_value - realAmount);
					return TransactionResult.Success(mutableData);

				}
			}

			return TransactionResult.Abort();
		};
	}

	private static Func<MutableData, TransactionResult> BuffTerminalTransaction() 
	{
		return mutableData => {

			object strength_obtained = mutableData.Value;

			if(strength_obtained != null) {

				int strength_value = (int)((long)strength_obtained);
				int realAmount = QuantitiesConstants.getTerminalBuffNext (strength_value);

				if(strength_value < QuantitiesConstants.STRENGTH_MAX) {

					mutableData.Value = Math.Min(QuantitiesConstants.STRENGTH_MAX, realAmount);
					return TransactionResult.Success(mutableData);
				}
			}

			return TransactionResult.Abort();
		};
	}

	private static Func<MutableData, TransactionResult> HealZoneTransaction(long amount, int userLevel) 
	{
		return mutableData => {

			object health_obtained = mutableData.Child ("health").Value;
			object level_obtained = mutableData.Child ("level").Value;

			if(health_obtained != null && level_obtained != null) {

				long realAmount = amount + QuantitiesConstants.ZONE_HEAL_LEVEL_BONUS[userLevel];
				long health_value = (long)health_obtained;
				long level_value = (long)level_obtained;

				int max_health_value = QuantitiesConstants.ZONE_MAX_HEALTH_VALUES[level_value];

				if(health_value < max_health_value) {
					mutableData.Child("health").Value = Math.Min(max_health_value, health_value + realAmount);

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

	private static Func<MutableData, TransactionResult> BuyPlayerSkinTransaction(int price, int number) 
	{
		return mutableData => {

			object credits_obtained = mutableData.Child("credits").Value;

			if(credits_obtained != null) {

				long credit_value = (long)credits_obtained - (long)price;

				if(User.MIN_CREDITS <= credit_value){

					object playerSkins = mutableData.Child("skins/boughtPlayers").Value;

					if(playerSkins == null){
						mutableData.Child("skins/boughtPlayers").Value = number.ToString();

							if( mutableData.Child ("effects/1SkinAchievement").Value == null){
								mutableData.Child ("effects/1SkinAchievement").Value = EffectsConstants.oneSkinAchievement.ToMap();

								object xp = mutableData.Child("xp").Value;
								if(xp != null){
									mutableData.Child("xp").Value = (long)xp + (long)EffectsConstants.oneSkinAchievementXp;
								}

								gameManager.setAchievement("1SkinAchievement");
							}

					} else{
						string playerString = mutableData.Child("skins/boughtPlayers").Value.ToString();
						if(!playerString.Contains(number.ToString())){
							playerString += number.ToString();
							mutableData.Child("skins/boughtPlayers").Value = playerString;
						
							if(playerString.Length == 4){
								if( mutableData.Child ("effects/4SkinAchievement").Value == null){
									mutableData.Child ("effects/4SkinAchievement").Value = EffectsConstants.fourSkinAchievement.ToMap();

									object xp = mutableData.Child("xp").Value;
									if(xp != null){
										mutableData.Child("xp").Value = (long)xp + (long)EffectsConstants.fourSkinAchievementXp;
									}

									gameManager.setAchievement("4SkinAchievement");
								}
							}
							if(playerString.Length == 7){
								if( mutableData.Child ("effects/allSkinAchievement").Value == null){
									mutableData.Child ("effects/allSkinAchievement").Value = EffectsConstants.allSkinAchievement.ToMap();

									object xp = mutableData.Child("xp").Value;
									if(xp != null){
										mutableData.Child("xp").Value = (long)xp + (long)EffectsConstants.allSkinAchievementXp;
									}

									gameManager.setAchievement("allSkinAchievement");
								}
							}
						}
					}

					mutableData.Child("credits").Value = credit_value;
					return TransactionResult.Success(mutableData);
				}
			}

			return TransactionResult.Abort();
		};
	}


	public static void AddTerminal(Terminal terminal, PopupScript messagePopup){

		reference.Child ("Game/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.TERMINAL_PLACE_COST)).ContinueWith (task => {

			if (task.Exception == null) {

				reference.Child ("Game").RunTransaction (AddTerminalTransaction (terminal)).ContinueWith(innerTask => {

					if (innerTask.Exception != null) {
						messagePopup.SetText("Not enough tokens for the team");
						reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (-QuantitiesConstants.TERMINAL_PLACE_COST));
					}else{
						AddTerminalPlacedStat();
					}
				});
			} else {
				Debug.Log("Could not deduct enough credit to put the terminal");
				messagePopup.SetText("You don't have enough credits to put the terminal");
			}
		});
	}

	public static void BuyPlayerSkin(int price, int number, PopupScript messagePopup){

		reference.Child ("Users/").Child (user.UserId).RunTransaction (BuyPlayerSkinTransaction(price,number)).ContinueWith (task => {

			if (task.Exception != null) {
				Debug.Log("Could not deduct enough credit to buy the skin");
				messagePopup.SetText("You don't have enough credits to buy the skin");
			} 
		});
	}


	public static void HurtTerminal(string terminalID, long amount, int userLevel, PopupScript messagePopup){

		reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.TERMINAL_SMASH_COST)).ContinueWith (task => {

			if (task.Exception == null) {

				reference.Child ("Game/Terminals/").Child(terminalID).Child("hp").RunTransaction (HurtTerminalTransaction (amount, userLevel)).ContinueWith(innerTask => {

					if (innerTask.Exception != null) {
						messagePopup.SetText("This terminal is already dead");

						MonoBehaviour.print ("Exception thrown in hurt terminal transaction: " + innerTask.Exception.Message);

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

	public static void BuffTerminal(string terminalID, int terminalStrength, PopupScript messagePopup){
		reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (-QuantitiesConstants.getTerminalBuffCost(terminalStrength))).ContinueWith (task => {

			if (task.Exception == null) {

				reference.Child("Game/Terminals/").Child(terminalID).Child("strength").RunTransaction(BuffTerminalTransaction()).ContinueWith(innerTask =>{
					if (innerTask.Exception != null) {
						reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.getTerminalBuffCost(terminalStrength)));
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

	public static void HealZone(string zoneID, long amount, int userLevel, PopupScript messagePopup){
		reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.ZONE_HEAL_COST)).ContinueWith (task => {

			if (task.Exception == null) {
				reference.Child ("Game/Zones/").Child (zoneID).RunTransaction (HealZoneTransaction (amount, userLevel)).ContinueWith (innerTask => {
					
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
						} else{
							AddZoneImprovedStat();
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
					reference.Child ("Game/Terminals/").Child (zoneID).RunTransaction (ImproveTerminalTransaction (level)).ContinueWith (innerTask => {
						if (innerTask.Exception != null) {
							messagePopup.SetText ("This terminal has been improved by someone else");
							reference.Child ("Users/").Child (user.UserId).Child ("credits").RunTransaction (UpdateCreditTransaction (QuantitiesConstants.TERMINAL_MAX_HEALTH_COST [level + 1]));
						}else{
							AddTerminalImprovedStat();
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
		

	public static void AddTerminalPlacedStat(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (
			AddEffectTransaction ("numberOfTerminalPlaced","numberOfTerminalPlaced","terminalPlacedMedal",
				"terminalPlacedAchievement", EffectsConstants.terminalPlacedMedal, EffectsConstants.terminalPlacedAchievement,
				EffectsConstants.terminalPlacedXp,EffectsConstants.terminalPlacedAchievementXp)
		).ContinueWith (task => {
		});
	}
		
	public static void AddTerminalBuffedStat(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (
			AddEffectTransaction ("numberOfTerminalBuffed","numberOfTerminalBuffed","terminalBuffedMedal",
				"terminalBuffedAchievement", EffectsConstants.terminalBuffedMedal, EffectsConstants.terminalBuffedAchievement,
				EffectsConstants.terminalBuffedXp,EffectsConstants.terminalBuffedAchievementXp)
		).ContinueWith (task => {
		});
	}
		
	public static void AddTerminalDamagedStat(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (
			AddEffectTransaction ("numberOfTerminalDamaged","numberOfTerminalDamaged","terminalDamagedMedal",
				"terminalDamagedAchievement", EffectsConstants.terminalDamagedMedal, EffectsConstants.terminalDamagedAchievement,
				EffectsConstants.terminalDamagedXp,EffectsConstants.terminalPlacedAchievementXp)
		).ContinueWith (task => {
		});
	}
		
	public static void AddTerminalImprovedStat(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (
			AddEffectTransaction ("numberOfTerminalImproved","numberOfTerminalImproved","terminalImprovedMedal",
				"terminalImprovedAchievement", EffectsConstants.terminalImprovedMedal, EffectsConstants.terminalImprovedAchievement,
				EffectsConstants.terminalImprovedXp,EffectsConstants.terminalImprovedAchievementXp)
		).ContinueWith (task => {
		});
	}

	public static void AddZoneHealStat(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (
			AddEffectTransaction ("numberOfZoneHealed","numberOfZoneHealed","zoneHealedMedal",
				"zoneHealedAchievement", EffectsConstants.zoneHealedMedal, EffectsConstants.zoneHealedAchievement,
				EffectsConstants.zoneHealedXp,EffectsConstants.zoneHealedAchievementXp)
		).ContinueWith (task => {
		});
	}

	public static void AddZoneImprovedStat(){
		reference.Child ("Users/").Child (user.UserId).RunTransaction (
			AddEffectTransaction ("numberOfZoneImproved","numberOfZoneImproved","zoneImprovedMedal",
				"zoneImprovedAchievement", EffectsConstants.zoneImprovedMedal, EffectsConstants.zoneImprovedAchievement,
				EffectsConstants.terminalImprovedXp,EffectsConstants.zoneImprovedAchievementXp)
		).ContinueWith (task => {
		});
	}

	private static Func<MutableData, TransactionResult> AddEffectTransaction(String name,String statName,String medalName,String achievementName,MedalInfo medal,Achievement achievement,int xpAdded,int achivementXp) 
	{
		return mutableData => {
			object xp = mutableData.Child("xp").Value;
			if(xp != null){
				mutableData.Child("xp").Value = (long)xp + (long)xpAdded;
			}

			object numberOfTerminal = mutableData.Child ("stat/" + statName).Value;
			if(numberOfTerminal == null){
				mutableData.Child ("stat/" + statName).Value = 1;
			} else{
				long number = (long)numberOfTerminal;
				mutableData.Child ("stat/" + statName).Value = number + 1;
				if((number + 1) >= EffectObtentionConstants.achievementStatMaxValue[name]){
					if( mutableData.Child ("effects/" + achievementName).Value == null){
						mutableData.Child ("effects/" + achievementName).Value = achievement.ToMap();

						xp = mutableData.Child("xp").Value;
						if(xp != null){
							mutableData.Child("xp").Value = (long)xp + (long)achivementXp;
						}

						gameManager.setAchievement(achievementName);
					}
				}

				if(medal != null && medalName != null){
					if(((int)(number + 1) % EffectObtentionConstants.medalNumberObtention[name]) == 0){
						//if(mutableData.Child ("effects/" + medalName).Value == null){
							mutableData.Child ("effects/" + medalName).Value = medal.ToMap();
						//}
					}
				}
			}
			return TransactionResult.Success(mutableData);
		};
	}

	private class Barrier {
		private static int id = 0;
		private bool state;
		public Barrier() {
			id += 1;
			state = false;
		}
		public bool barrier() {
			return state;
		}
		public void close() {
			this.state = true;
		}
	}

}