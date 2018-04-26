using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementMenu : MonoBehaviour {
	
	public List<Text> achievementsStatText;
	private Dictionary<string,Text> achievementsStatDict = new Dictionary<string,Text>();

	public List<Text> achievementsSkinText;
	private Dictionary<string,Text> achievementsSkinDict = new Dictionary<string,Text>();

	public List<Text> achievementsRankText;
	private Dictionary<string,Text> achievementsRankDict = new Dictionary<string,Text>();

	public Text allAchievement;

	void Start () {
		for (int i = 0; i < achievementsStatText.Count; i++) {
			achievementsStatDict.Add (achievementsStatText [i].name, achievementsStatText [i]);
		}
		for (int i = 0; i < achievementsSkinText.Count; i++) {
			achievementsSkinDict.Add (achievementsSkinText [i].name, achievementsSkinText [i]);
		}
		for (int i = 0; i < achievementsRankText.Count; i++) {
			achievementsRankDict.Add (achievementsRankText [i].name, achievementsRankText [i]);
		}
	}

	public void UpdateAchivement(Statistics statistics,SkinsInfo skins,Effects effects){
		UpdateStatAchievement (statistics);
		UpdateSkinsAchievement (skins);
		UpdateRankAchievement (effects);
		UpdateAllAchivement (effects);
	}
		
	private void UpdateStatAchievement (Statistics statistics){
		foreach (KeyValuePair<string, int> entry in statistics.GetDict()) {
			CheckIfAchievementUnlockedFromStat (entry.Key, entry.Value);
		}
	}

	private void UpdateSkinsAchievement(SkinsInfo skins){
		foreach (KeyValuePair<string, Text> entry in achievementsSkinDict) {
			int maxNum = EffectObtentionConstants.achievementSkinsMaxValue [entry.Key];
			int num = skins.boughtPlayer.Count;
			if (num >= maxNum) {
				entry.Value.text = "DONE!";
			} else {
				entry.Value.text = num.ToString() + "/" + maxNum.ToString ();
			}
		}
	}

	private void UpdateRankAchievement(Effects effects){
		foreach (KeyValuePair<string, Text> entry in achievementsRankDict) {
			entry.Value.text = "0/1";
			foreach (var achievement in effects.achievements) {
				if (achievement.GetName () == entry.Key) {
					entry.Value.text = "DONE!";
				}
			}
		}
	}

	private void UpdateAllAchivement(Effects effects){
		allAchievement.text = effects.achievements.Count +"/10";
		foreach (var achievement in effects.achievements) {
			if (achievement.GetName () == EffectsConstants.allAchievement.GetName()) {
				allAchievement.text = "DONE!";
			}
		}
	}

	private void CheckIfAchievementUnlockedFromStat(string name,int num){
		Text text = achievementsStatDict [name];
		int maxNum = EffectObtentionConstants.achievementStatMaxValue [name];
		if (num >= maxNum) {
			text.text = "DONE!";
		} else {
			text.text = num.ToString() + "/" + maxNum.ToString ();
		}
	}
}
