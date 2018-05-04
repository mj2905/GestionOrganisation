using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ImproveButtonHandler : LocationListener {

	private Button improveButton;
	private Text improveButtonText;
	private bool attackMode = false;
	public enum TARGET {NO_TARGET, TERMINAL, ZONE};
	private TARGET targeting;
	private bool sameTeam;
	private int credits;

	private int terminalHealth;
	private int terminalLevel;
	private int zoneHealth;
	private int zoneLevel;

	private Image cooldownImage;
	private float zoneTimer,terminalTimer;

	void Start() {
		improveButton = GetComponent<Button> ();
		improveButtonText = improveButton.transform.Find ("Text").GetComponent<Text> ();
		cooldownImage = improveButton.transform.Find ("CooldownImage").GetComponent<Image> ();
		targeting = TARGET.NO_TARGET;
		sameTeam = false;
		credits = 0;
	}

	void Update() {
		zoneTimer -= Time.deltaTime;
		terminalTimer -= Time.deltaTime;

		zoneTimer = Mathf.Max (zoneTimer, 0f);
		terminalTimer = Mathf.Max (terminalTimer, 0f);

		if (attackMode || targeting == TARGET.NO_TARGET) {
			improveButtonText.text = "✕";
			improveButton.interactable = false;
			cooldownImage.fillAmount = 0f;
		} else if(targeting == TARGET.TERMINAL) {
			if (sameTeam) {
				if (terminalHealth <= QuantitiesConstants.TERMINAL_MIN_HEALTH) {
					improveButtonText.text = "Dead ✝";
					improveButton.interactable = false;
				} else if (terminalLevel == QuantitiesConstants.TERMINAL_MAX_HEALTH_VALUES.Length - 1) {
					improveButtonText.text = "Heal ✓";
					improveButton.interactable = false;
				} else if (credits < QuantitiesConstants.TERMINAL_MAX_HEALTH_COST[terminalLevel + 1]) {
					improveButtonText.text = "Heal+ "+ QuantitiesConstants.TERMINAL_MAX_HEALTH_COST [terminalLevel + 1] +"$";
					improveButton.interactable = false;
				} else {
					improveButtonText.text = "Heal+ "+ QuantitiesConstants.TERMINAL_MAX_HEALTH_COST [terminalLevel + 1] +"$";
					improveButton.interactable = true;
				}
				if (terminalTimer != 0f) {
					improveButton.interactable = false;
					cooldownImage.fillAmount = terminalTimer / QuantitiesConstants.TERMINAL_IMPROVE_COOLDOWN;
				} else {
					cooldownImage.fillAmount = 0f;
				}
			} else {
				improveButtonText.text = "✕";
				improveButton.interactable = false;
				cooldownImage.fillAmount = 0f;
			}
		} else if(targeting == TARGET.ZONE) {
			if (sameTeam) {
				if (zoneHealth <= QuantitiesConstants.ZONE_MIN_HEALTH) {
					improveButtonText.text = "Improve ✝";
					improveButton.interactable = false;
				} else if (zoneLevel < QuantitiesConstants.ZONE_MAX_HEALTH_VALUES.Length - 1) {
					if(credits >= QuantitiesConstants.ZONE_MAX_HEALTH_COST [zoneLevel + 1]) {
						improveButtonText.text = "Improve "+ QuantitiesConstants.ZONE_MAX_HEALTH_COST [zoneLevel + 1] +"₡";
						improveButton.interactable = true;
					}
					else {
						improveButtonText.text = "Improve "+ QuantitiesConstants.ZONE_MAX_HEALTH_COST [zoneLevel + 1] +"₡";
						improveButton.interactable = false;
					}
				} else {
					improveButtonText.text = "Improve ✓";
					improveButton.interactable = false;
				}
				if (zoneTimer != 0f) {
					improveButton.interactable = false;
					cooldownImage.fillAmount = zoneTimer / QuantitiesConstants.ZONE_IMPROVE_COOLDOWN;
				} else {
					cooldownImage.fillAmount = 0f;
				}
			} else {
				improveButtonText.text = "✕";
				improveButton.interactable = false;
				cooldownImage.fillAmount = 0f;
			}
		}
	}

	public void SetZoneTimer(){
		zoneTimer =  QuantitiesConstants.ZONE_IMPROVE_COOLDOWN;
	}
	public void SetTerminalTimer(){
		terminalTimer =  QuantitiesConstants.TERMINAL_IMPROVE_COOLDOWN;
	}

	public void targetingZone(bool sameTeam, int zoneHealth, int level, int credits) {
		Assert.IsTrue (!attackMode);
		setTargetZoneHealth (zoneHealth, level);
		setCredits (credits);
		this.sameTeam = sameTeam;
		targeting = TARGET.ZONE;
	}

	public void targetingTerminal(bool sameTeam, int terminalHealth, int level, int credits) {
		Assert.IsTrue (!attackMode);
		setTargetTerminalHealth (terminalHealth, level);
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

	public void setTargetTerminalHealth(int terminalHealth, int level) {
		this.terminalHealth = terminalHealth;
		this.terminalLevel = level;
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
