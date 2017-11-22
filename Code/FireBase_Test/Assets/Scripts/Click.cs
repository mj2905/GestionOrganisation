using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class Click
{
	public int currentClick;
	private Text text;

	public Click (int currentClick,Text text)
	{
		this.currentClick = currentClick;
		this.text = text;
		text.text = currentClick.ToString ();
	}

	public void click ()
	{
		addClick (1);
	}

	public void applyBonus(){
		addClick (100);
	}

	private void addClick(int i){
		currentClick += i;
		text.text = currentClick.ToString ();
		string json = JsonUtility.ToJson (this);
		//FirebaseManager.reference.Child ("clicks").Child (FirebaseManager.auth.CurrentUser.UserId).SetRawJsonValueAsync (json);
	}

}
