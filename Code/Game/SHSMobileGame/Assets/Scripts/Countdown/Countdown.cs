using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Countdown : MonoBehaviour {

	public Text countdown;
	private const int day = 14;
	private const int hour = 12;
	private const int minute = 0;
	private const int second = 0;
	public static readonly DateTime start = new DateTime(2018, 5, day, hour - 2, minute, second);

	// Update is called once per frame
	void FixedUpdate () {
		TimeSpan span = start - DateTime.UtcNow;
		if (span.TotalSeconds <= 0) {
			SceneManager.LoadScene ("InitialScene");
		}

		string str = ((span.TotalHours > 1) ? String.Format("{0:000}:", (int)(span.TotalHours)) : "") + 
			((span.TotalMinutes > 1) ? String.Format("{0:00}:", span.Minutes) : "") + 
			((span.TotalSeconds > 1) ? String.Format("{0:00}", span.Seconds) : "");
		countdown.text = str;
	}
}
