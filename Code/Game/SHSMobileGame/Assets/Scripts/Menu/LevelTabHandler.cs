using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTabHandler : MonoBehaviour {

	public GameObject levelTab;
	public Text jobText;
	public Text bonusText;

	public Text maxText;
	public Text minText;
	public Text currentText;
	public Image fill;
	public Image background;

	private float width;

	public void Start(){
		width = background.rectTransform.rect.width;
	}


	private string[] jobs = new string[] {
		"MANnish", "Almost student", "Lab slave", "PhDesperate", "Professor-sama"
	};

	private string[] texts = new string[]{
		"Nothing special, no bonus for you.", "Your incredible mental capacities allow your terminals to pew pew more, and to better heal your zones (+5 damages for all terminals, +10 HP per zone heal).",
		"You are the elite of the elite. You can repair terminals and your zones faster (+5 damages for all terminals, +5 HP per terminal heal, +20 HP per zone heal).",
		"Nobody understands what you are doing, great! Punch harder the zones with terminals, and better heal your zones (+10 damages for all terminals, +5 HP per terminal heal, +30 HP per zone heal).",
		"Notice me, se-se-sempai. You can repair terminals faster and your zones (+10 damages for all terminals, +10 HP per terminal heal, +35 HP per zone heal)."
	};

	public int level = 0;
	public int xp = 0;

	public void setLevel(int level,int xp) {
		this.level = level;
		this.xp = xp;
		if (this.level >= jobs.Length - 1) {
			this.level = jobs.Length - 1;
		}
	}

	void Update() {
		if(level < QuantitiesConstants.PLAYER_XP_THRESHOLDS.Length - 1){
			fill.fillAmount = (float)(xp - QuantitiesConstants.PLAYER_XP_THRESHOLDS[level]) / (float)(QuantitiesConstants.PLAYER_XP_THRESHOLDS[level+1] - QuantitiesConstants.PLAYER_XP_THRESHOLDS[level]);
		} else {
			fill.fillAmount = 1;
			fill.color = ColorConstants.GRAY; 
		}

		jobText.text = jobs [level];
		bonusText.text = texts [level];
		minText.text = QuantitiesConstants.PLAYER_XP_THRESHOLDS [level].ToString();
		maxText.text = QuantitiesConstants.PLAYER_XP_THRESHOLDS [level+1].ToString();
		currentText.text = xp.ToString();

		currentText.transform.localPosition = new Vector2(width * fill.fillAmount - width*0.5f,currentText.transform.localPosition.y);
	}
}
