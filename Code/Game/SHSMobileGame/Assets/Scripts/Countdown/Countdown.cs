using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Countdown : MonoBehaviour {

	public GameObject title;
	public GameObject bottom;
	public GameObject connecting;
	public GameObject spinning;
	public Text countdown;

	private bool serverTimeChanged;
	private bool serverBegTimeChanged;

	private float realTime;
	private long lastServerTime = FirebaseManager.GetServerTime();

	void Start() {
		realTime = Time.realtimeSinceStartup;
		serverTimeChanged = false;
		serverBegTimeChanged = false;
		title.SetActive (false);
		bottom.SetActive (false);
		connecting.SetActive (true);
		spinning.SetActive (true);
		countdown.text = "";
	}

	// Update is called once per frame
	void FixedUpdate () {

		long serverTime = FirebaseManager.GetServerTime ();
		if (serverTime != FirebaseManager.DEFAULT_TIME) {
			realTime = Time.realtimeSinceStartup;
			lastServerTime = serverTime;
			serverTimeChanged = true;
		}

		long startTime = FirebaseManager.GetBeginTime ();
		serverBegTimeChanged = (startTime  != FirebaseManager.DEFAULT_BEG_TIME);

		if (serverTimeChanged && serverBegTimeChanged) {
			title.SetActive (true);
			bottom.SetActive (true);
			connecting.SetActive (false);
			spinning.SetActive (false);

			TimeSpan span = Countdown.getTime(startTime) - Countdown.getTime(serverTime + Mathf.Min((int)(Time.realtimeSinceStartup - realTime), 10));
			if ((Countdown.getTime(startTime) - Countdown.getTime(serverTime)).TotalSeconds <= 0) {
				SceneManager.LoadScene ("InitialScene");
				return;
			}

			string str = ((span.TotalSeconds >= 3599) ? String.Format("{0:000}:", (int)(span.TotalHours)) : "") + 
				((span.TotalSeconds >= 59) ? String.Format("{0:00}:", span.Minutes) : "") + 
				((span.TotalSeconds >= 1) ? String.Format("{0:00}", span.Seconds) : "");

			countdown.text = str;
		}
	}

	public static DateTime getTime(long time) {
		int secs = (int)(time % 60);
		int minutes = (int)((time / 60) % 60);
		int hours = (int)((time / 60 / 60) % 24);
		int days = (int)((time / 60 / 60 / 24) % 31);
		return new DateTime(2018, 5, days, hours, minutes, secs);
	}
}
