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
	private const int day = 14;
	private const int hour = 12;
	private const int minute =0;
	private const int second = 0;
	public static DateTime start = getTime (FirebaseManager.GetBeginTime ());//new DateTime(2018, 5, day, hour, minute, second);

	private bool first;

	private float realTime = 0;
	private long lastServerTime = FirebaseManager.GetServerTime();

	void Start() {
		first = true;
		title.SetActive (false);
		bottom.SetActive (false);
		countdown.gameObject.SetActive (false);
		connecting.SetActive (true);
		spinning.SetActive (true);
		countdown.text = "";
	}

	// Update is called once per frame
	void FixedUpdate () {

		start = getTime (FirebaseManager.GetBeginTime ());

		long serverTime = FirebaseManager.GetServerTime ();
		if (serverTime != lastServerTime) {
			realTime = Time.realtimeSinceStartup;
			lastServerTime = serverTime;
			first = false;
		}

		Debug.Log ("Countdown");
		Debug.Log (start);
		Debug.Log (Countdown.getTime(serverTime));
		Debug.Log (start - Countdown.getTime(serverTime + Mathf.Min((int)(Time.realtimeSinceStartup - realTime), 10)));
		Debug.Log ((start - Countdown.getTime (serverTime)).TotalSeconds);
		TimeSpan span = start - Countdown.getTime(serverTime + Mathf.Min((int)(Time.realtimeSinceStartup - realTime), 10));
		if ((start - Countdown.getTime(serverTime)).TotalSeconds <= 0) {
			SceneManager.LoadScene ("InitialScene");
			return;
		}

		string str = ((span.TotalHours >= 1) ? String.Format("{0:000}:", (int)(span.TotalHours)) : "") + 
			((span.TotalMinutes >= 1) ? String.Format("{0:00}:", span.Minutes) : "") + 
			((span.TotalSeconds >= 1) ? String.Format("{0:00}", span.Seconds) : "");

		if (!first) {
			title.SetActive (true);
			bottom.SetActive (true);
			countdown.gameObject.SetActive (true);
			connecting.SetActive (false);
			spinning.SetActive (false);
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
