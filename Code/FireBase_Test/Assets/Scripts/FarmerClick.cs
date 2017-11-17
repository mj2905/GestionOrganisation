using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class FarmerClick : MonoBehaviour
{	
	public Text numberOfClicks;
	private Click click;
	private BonusScript bonus;

	void Start ()
	{
		//InitSync ();
		click = new Click(0,numberOfClicks);
		GameObject clone = (GameObject)Instantiate(Resources.Load("Bonus"));
		bonus = clone.GetComponent<BonusScript>();
		bonus.setClick (click);
		bonus.transform.SetParent (this.transform.parent,false);
		bonus.transform.SetAsLastSibling ();
	}

	private void InitSync ()
	{
		FirebaseDatabase.DefaultInstance.GetReference ("clicks").Child (FirebaseManager.auth.CurrentUser.UserId).Child ("currentClick")
			.GetValueAsync ().ContinueWith (task => {
			int temp = 0;
			Debug.Log (temp);
			
			if (task == null) {
			} else {
				if (task.IsFaulted) {
					// Handle the error...
					Debug.Log ("Error");
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					// Do something with snapshot...
					if (snapshot != null) {
						object val = snapshot.Value;
						if (val != null) {
							temp = int.Parse (val.ToString ());
						}
					}
				}
			}

			Debug.Log (temp);
				this.click = new Click (temp,numberOfClicks);
		});
	}

	public void OnClick ()
	{
		click.click ();
	}
}
