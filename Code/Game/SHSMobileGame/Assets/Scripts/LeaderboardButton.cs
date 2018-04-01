using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LeaderboardButton : MonoBehaviour {

	public List<RectTransform> lines;

	private float maxWidth;
	private List<int> scores;
	private List<float> position = new List<float>();

	private List<float> newOffset = new List<float>();
	private List<float> newPosition  = new List<float>();


	// Use this for initialization
	void Start () {
		for (int i = 0; i < lines.Count; i++) {
			lines[i].GetComponentInParent<Image> ().color = ColorConstants.getColor (i+1);
		}

		for (int i = 0; i < lines.Count; i++) {
			position.Add(lines[i].position.y);
		}	
		maxWidth = lines[0].rect.width;
	}

	public void SetScores(List<int> scores){
		this.scores = scores;
		UpdateButton();
	}

	public void Update(){
		for (int i = 0; i < newOffset.Count; i++) {
			lines [i].offsetMin = new Vector2 (Mathf.MoveTowards(lines [i].offsetMin.x,newOffset[i],10),lines[i].offsetMin.y);
			lines [i].position = new Vector3 (lines [i].position.x,Mathf.MoveTowards(lines [i].position.y,newPosition[i],10),lines [i].position.z);
		}
	}

	private void UpdateButton(){
		newOffset = new List<float> ();
		newPosition = new List<float> ();

		float maxScore = 0f;
		for (int i = 0; i < scores.Count; i++) {
			maxScore = Mathf.Max (maxScore, scores [i]);
		}
		var sorted = scores.Select((x, i) => new KeyValuePair<int, int>(x, i))
			.OrderByDescending(x => x.Key)
			.ToList();
		List<int> idx = sorted.Select(x => x.Value).ToList();

		for (int i = 0; i < scores.Count; i++) {
			newOffset.Add(maxWidth*(1 - (float)(scores[i]) / maxScore));
			newPosition.Add(position[idx.IndexOf(i)]);
		}
	}
}
