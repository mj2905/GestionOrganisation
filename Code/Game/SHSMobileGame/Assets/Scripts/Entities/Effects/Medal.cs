using System;
using UnityEngine;
using UnityEngine.UI;

public class Medal : MonoBehaviour{

	public Image imageTime;
	public Text textMult;

	private int ttl;
	private int multiplier;
	private string name;
	private float height;
	private RectTransform rectTransform;
	private Vector2 offsetMax;

	private bool destroy = false;
	private bool destroyed = false;

	private Vector3 initialPosition;
	private int position;

	public Medal (string name, int multiplier,int ttl)
	{
		this.name = name;
		this.ttl = ttl;
		this.multiplier = multiplier;
	}	

	public string GetName(){
		return name;
	}

	public int GetTtl(){
		return ttl;
	}

	public int GetMultiplier(){
		return multiplier;
	}

	public void Copy(Medal medal){
		ttl = medal.GetTtl ();
		multiplier = medal.GetMultiplier ();
		name = medal.GetName ();
	}

	public void SetInitialPosition(Vector3 initialPosition){
		this.initialPosition = initialPosition;
	}

	public void SetPosition(int position){
		this.position = position;
	}

	public void Start(){
		rectTransform = imageTime.GetComponent<RectTransform> ();
		height = rectTransform.rect.height;
		offsetMax = rectTransform.offsetMax;

		textMult.text = "x " + multiplier;

		transform.position = initialPosition - new Vector3(100,((1.2f*(position)))*GetComponent<RectTransform>().rect.height,0);
	}
		
	public void Update(){
		if (!destroyed) {
			if (!destroy) {
				transform.position = Vector3.MoveTowards (transform.position, initialPosition - new Vector3 (0, ((1.2f * (position))) * GetComponent<RectTransform> ().rect.height, 0), 12);
				rectTransform.offsetMax = new Vector2 (0, (-1 + (float)(this.GetTtl ()) / (float)(EffectsConstants.GetMedalByName (this.GetName ()).GetTtl ())) * height);
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

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Medal))
			return false;
		Medal other = (Medal)obj;
		return name == other.name;
	}
	

	public override int GetHashCode ()
	{
		unchecked {
			return (name != null ? name.GetHashCode () : 0);
		}
	}
	
}

