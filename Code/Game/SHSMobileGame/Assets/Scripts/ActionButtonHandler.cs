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
	private bool sameTeam;
	private int credits;

	private int terminalHealth;
	private int terminalLevel;
	private int terminalStrength;
	private int zoneHealth;
	private int zoneLevel;

	void Start() {
		actionButton = GetComponent<Button> ();
		actionButtonText = actionButton.transform.Find ("Text").GetComponent<Text> ();
		targeting = TARGET.NO_TARGET;
		sameTeam = false;
		credits = 0;
	}

	void Update() {
		if (attackMode || targeting == TARGET.NO_TARGET) {
			actionButtonText.text = "No action";
			actionButton.interactable = false;
		} else if(targeting == TARGET.TERMINAL) {
			if (sameTeam) {
				if (terminalStrength >= QuantitiesConstants.STRENGTH_MAX) {
					actionButtonText.text = "Buff ✓";
					actionButton.interactable = false;
				} else if (credits < QuantitiesConstants.getTerminalBuffCost(terminalStrength)) {
					actionButtonText.text = "Buff "+ (QuantitiesConstants.getTerminalBuffCost(terminalStrength)) +"$";
					actionButton.interactable = false;
				} else {
					actionButtonText.text = "Buff "+ (QuantitiesConstants.getTerminalBuffCost(terminalStrength)) +"$";
					actionButton.interactable = true;
				}
			} else {
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
			}
		} else if(targeting == TARGET.ZONE) {
			if (sameTeam) {
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
			} else {
				actionButtonText.text = "No action";
				actionButton.interactable = false;
			}
		}
	}

	public void targetingZone(bool sameTeam, int zoneHealth, int level, int credits) {
		Assert.IsTrue (!attackMode);
		setTargetZoneHealth (zoneHealth, level);
		setCredits (credits);
		this.sameTeam = sameTeam;
		targeting = TARGET.ZONE;
	}

	public void targetingTerminal(bool sameTeam, int terminalHealth, int level, int credits, int strength) {
		Assert.IsTrue (!attackMode);
		setTargetTerminalHealth (terminalHealth, level, strength);
		setCredits (credits);
		this.sameTeam = sameTeam;
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
