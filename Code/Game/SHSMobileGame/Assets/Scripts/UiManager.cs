using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : MonoBehaviour
{

	public Text creditText;
	public Text scoreText;


	// Use this for initialization
	void Start ()
	{
		FirebaseManager.SetScoreCredit (creditText, scoreText);
	}

}
