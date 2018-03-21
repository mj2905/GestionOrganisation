using System;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
	{

	private bool destroy = false;
	private bool destroyed = false;

	private Vector3 initialPosition;
	private Zone targetBuilding;
	private int position;
	private CameraController camera;
	private UiManager ui;
	private string name;

	public Text text;
	public Image image;

	public enum Type{AllyTerminal,EnemyTerminal}
	private Type currentType;

		public Notification (){}

		public void SetTargetPosition(Zone targetBuilding){
			this.targetBuilding = targetBuilding;
		}

		public void SetInitialPosition(Vector3 initialPosition){
			this.initialPosition = initialPosition;
		}

		public void SetPosition(int position){
			this.position = position;
		}

		public void Start(){
			this.camera = Camera.main.GetComponent<CameraController> ();
			Debug.Log (position);
			transform.position = initialPosition - new Vector3(-100,((1.2f*(position)))*GetComponent<RectTransform>().rect.height,0);
		}

		public void Update(){
			if (!destroyed) {
				if (!destroy) {
					transform.position = Vector3.MoveTowards (transform.position, initialPosition - new Vector3 (0, ((1.2f * (position))) * GetComponent<RectTransform> ().rect.height, 0), 12);
				} else {
					transform.localScale = Vector3.MoveTowards (transform.localScale, new Vector3 (0, 0, 0), 0.1f);
				}

				if (transform.localScale == new Vector3 (0, 0, 0)) {
					Destroy (this.gameObject);
					destroyed = true;
				}
			}
		}

	public void GoToNotification(){
		ui.removeNotification (this.name);
		this.camera.GoToBuilding (targetBuilding);
		destroy = true;
	}

	public void SetName(string name){
		this.name = name;
	}

	public void DestroyNotification(){
		destroy = true;
	}

	public void SetType(Type type){
		currentType = type;
		switch(type){
		case Type.AllyTerminal:
			image.color = ColorConstants.colorNotificationAlly;
			break;
		case Type.EnemyTerminal:
			image.color = ColorConstants.colorNotificationEnemy;
			break;
		}
	}

	public void SetText(string name){
		text.text = name.ToUpper();
	}

	public void SetUi(UiManager ui){
		this.ui = ui;
	}

	public string GetName(){
		return name;
	}
}

