using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ActionButtonHandler : LocationListener {

	private Button actionButton;
	private Text actionButtonText;
	private bool attackMode = false;
	public enum TARGET {NO_TARGET, TERMINAL, ZONE};
	private TARGET targeting;
	private bool isSameTerminalTeam;
	private bool isSameZoneTeam;
	private int credits;

	private int terminalHealth;
	private int terminalLevel;
	private int terminalStrength;
	private int zoneHealth;
	private int zoneLevel;

	private Image cooldownImage;
	private float healTimer,buffTimer,smashTimer;

	void Start() {
		actionButton = GetComponent<Button> ();
		actionButtonText = actionButton.transform.Find ("Text").GetComponent<Text> ();
		cooldownImage = actionButton.transform.Find ("CooldownImage").GetComponent<Image> ();

		targeting = TARGET.NO_TARGET;
		isSameTerminalTeam = false;
		isSameZoneTeam = false;
		credits = 0;
	}

	void Update() {
		buffTimer -= Time.deltaTime;
		healTimer -= Time.deltaTime;
		smashTimer -= Time.deltaTime;

		buffTimer = Mathf.Max (buffTimer, 0);
		healTimer = Mathf.Max (healTimer, 0);
		smashTimer = Mathf.Max (smashTimer, 0);

		if (attackMode || targeting == TARGET.NO_TARGET) {
			actionButtonText.text = "No action";
			actionButton.interactable = false;
			cooldownImage.fillAmount = 0f;
		} else if(targeting == TARGET.TERMINAL) {
			if (isSameTerminalTeam) {
				if (terminalStrength >= QuantitiesConstants.STRENGTH_MAX) {
					actionButtonText.text = "Attack ✓";
					actionButton.interactable = false;
				} else if (credits < QuantitiesConstants.getTerminalBuffCost (terminalStrength)) {
					actionButtonText.text = "Attack+ " + (QuantitiesConstants.getTerminalBuffCost (terminalStrength)) + "$";
					actionButton.interactable = false;
				} else {
					actionButtonText.text = "Attack+ " + (QuantitiesConstants.getTerminalBuffCost (terminalStrength)) + "$";
					actionButton.interactable = true;
				}
				if (buffTimer != 0) {
					actionButton.interactable = false;
					cooldownImage.fillAmount = buffTimer / QuantitiesConstants.BUFF_COOLDOWN;
				} else {
					cooldownImage.fillAmount = 0f;
				}
			} else {
				if (isSameZoneTeam) {
					if (terminalHealth <= QuantitiesConstants.TERMINAL_MIN_HEALTH) {
						actionButtonText.text = "Smash ✝";
						actionButton.interactable = false;
					} else if (credits < -QuantitiesConstants.TERMINAL_SMASH_COST) {
						actionButtonText.text = "Smash " + (-QuantitiesConstants.TERMINAL_SMASH_COST) + "$";
						actionButton.interactable = false;
					} else {
						actionButtonText.text = "Smash " + (-QuantitiesConstants.TERMINAL_SMASH_COST) + "$";
						actionButton.interactable = true;
					}
					if (smashTimer != 0) {
						actionButton.interactable = false;
						cooldownImage.fillAmount = smashTimer / QuantitiesConstants.SMASH_COOLDOWN;
					} else {
						cooldownImage.fillAmount = 0f;
					}
				} else {
					actionButtonText.text = "No action";
					actionButton.interactable = false;
					cooldownImage.fillAmount = 0f;
				}
			}
		} else if(targeting == TARGET.ZONE) {
			if (isSameZoneTeam) {
				if (zoneHealth >= QuantitiesConstants.ZONE_MAX_HEALTH_VALUES[zoneLevel]) {
					actionButtonText.text = "Heal ✓";
					actionButton.interactable = false;
				} else if (zoneHealth <= QuantitiesConstants.ZONE_MIN_HEALTH) {
					actionButtonText.text = "Heal ✝";
					actionButton.interactable = false;
				} else if (credits < -QuantitiesConstants.ZONE_HEAL_COST) {
					actionButtonText.text = "Heal "+ (-QuantitiesConstants.ZONE_HEAL_COST) +"$";
					actionButton.interactable = false;
				} else {
					actionButtonText.text = "Heal "+ (-QuantitiesConstants.ZONE_HEAL_COST) +"$";
					actionButton.interactable = true;
				}
				if (healTimer != 0) {
					actionButton.interactable = false;
					cooldownImage.fillAmount = healTimer / QuantitiesConstants.HEAL_COOLDOWN;
				} else {
					cooldownImage.fillAmount = 0f;
				}
			} else {
				actionButtonText.text = "No action";
				actionButton.interactable = false;
				cooldownImage.fillAmount = 0f;
			}
		}
	}

	public void SetBuffTimer(){
		buffTimer =  QuantitiesConstants.BUFF_COOLDOWN;
	}
	public void SetHealTimer(){
		healTimer =  QuantitiesConstants.HEAL_COOLDOWN;
	}
	public void SetSmashTimer(){
		smashTimer =  QuantitiesConstants.SMASH_COOLDOWN;
	}

	public void targetingZone(bool isSameZoneTeam, int zoneHealth, int level, int credits) {
		Assert.IsTrue (!attackMode);
		setTargetZoneHealth (zoneHealth, level);
		setCredits (credits);
		this.isSameZoneTeam = isSameZoneTeam;
		targeting = TARGET.ZONE;
	}

	public void targetingTerminal(bool isSameTerminalTeam, int terminalHealth, int level, int credits, int strength) {
		Assert.IsTrue (!attackMode);
		setTargetTerminalHealth (terminalHealth, level, strength);
		setCredits (credits);
		this.isSameTerminalTeam = isSameTerminalTeam;
		targeting = TARGET.TERMINAL;
	}

	public void targetingNothing() {
		Assert.IsTrue (!attackMode);
		targeting = TARGET.NO_TARGET;
	} 

	public void setCredits(int credits) {
		this.credits = credits;
	}

	public void setTargetTerminalHealth(int terminalHealth, int level, int strength) {
		this.terminalHealth = terminalHealth;
		this.terminalLevel = level;
		this.terminalStrength = strength;
	}

	public void setTargetZoneHealth(int zoneHealth, int level) {
		this.zoneHealth = zoneHealth;
		this.zoneLevel = level;
	}


	override public void CoordinateUpdate(XYCoordinate coords) {}

	override public void StopLocationHandling() {
		attackMode = false;
	}

	override public void FirstLocationSent() {
		attackMode = true;
	}

}
