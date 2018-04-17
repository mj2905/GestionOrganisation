using System;
using UnityEngine;
using UnityEngine.UI;

public class Medal : MonoBehaviour{

	public Image imageTime;
	public Text textMult;

	private MedalInfo medalInfo;

	private float height;
	private RectTransform rectTransform;
	private Vector2 offsetMax;

	private bool destroy = false;
	private bool destroyed = false;

	private Vector3 initialPosition;
	private int position;

	public MedalInfo GetMedalInfo(){
		return medalInfo;
	}

	public void Copy(Medal medal){
		this.medalInfo = medal.GetMedalInfo ();
	}

	public void Copy(MedalInfo medal){
		this.medalInfo = medal;
	}

	public void SetInitialPosition(Vector3 initialPosition){
		this.initialPosition = initialPosition;
	}

	public void SetPosition(int position){
		this.position = position;
	}

	public void Start(){
		textMult.text = "x " + medalInfo.GetMultiplier();

		transform.position = initialPosition - new Vector3(100,((1.2f*(position)))*GetComponent<RectTransform>().rect.height,0);
	}
		
	public void Update(){
		if (!destroyed) {
			if (!destroy) {
				transform.position = Vector3.MoveTowards (transform.position, initialPosition - new Vector3 (0, ((1.2f * (position))) * GetComponent<RectTransform> ().rect.height, 0), 12);
				imageTime.fillAmount = (float)(this.GetMedalInfo().GetTtl ()) / (float)(EffectsConstants.GetMedalByName (this.GetMedalInfo().GetNormalisedName ()).GetTtl ());
			} else {
				transform.localScale = Vector3.MoveTowards (transform.localScale, new Vector3 (0, 0, 0), 0.1f);
			}

			if (transform.localScale == new Vector3 (0, 0, 0)) {
				Destroy (this.gameObject);
				destroyed = true;
			}
		}
	}

	public void DestroyMedal(){
		destroy = true;
	}
}

