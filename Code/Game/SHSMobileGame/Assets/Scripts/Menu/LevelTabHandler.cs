using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTabHandler : MonoBehaviour {

	public GameObject levelTab;
	public Text jobText;
	public Text bonusText;

	private string[] jobs = new string[] {
		"MANnish", "Almost student", "Lab slave", "PhDesperate", "Professor-sama"
	};

	private string[] texts = new string[]{
		"Nothing special, no bonus for you.", "Your incredible mental capacities allow your terminals to pew pew more (+5 damages for all terminals), and to better heal your zones (+10 HP per heal).",
		"You are the elite of the elite. You can repair terminals faster (+5 HP per heal) and your zones (+10 HP per heal).",
		"Nobody understands what you are doing, great! Punch harder the zones with terminals (+5 damages for all terminals), and better heal your zones (+10 HP per heal).",
		"Notice me, se-se-sempai. You can repair terminals faster (+5 HP per heal) and your zones (+5 HP per heal)."
	};

	public int level = 1;

	public void setLevel(int level) {
		this.level = level;
		if (this.level >= jobs.Length) {
			this.level = jobs.Length;
		}
	}

	void Update() {
		jobText.text = jobs [level - 1];
		bonusText.text = texts [level - 1];
	}

	public void OpenLevelTabHandler() {
		levelTab.SetActive (true);
	}

	public void CloseLevelTabHandler() {
		levelTab.SetActive (false);
	}
}
