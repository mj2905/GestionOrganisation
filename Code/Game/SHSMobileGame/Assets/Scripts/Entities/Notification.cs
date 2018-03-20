using System;
using UnityEngine;

public class Notification : MonoBehaviour
	{

	private bool destroy = false;
	private bool destroyed = false;

	private Vector3 initialPosition,targetPosition;
	private int position;
	private CameraController camera;
		public Notification (){}

		public void Start(){
			this.camera = Camera.main.GetComponent<CameraController> ();
		}

		public void SetTargetPosition(Vector3 targetPosition){
			this.targetPosition = targetPosition;
		}

		public void SetInitialPosition(Vector3 initialPosition){
			this.initialPosition = initialPosition;
		}

		public void SetPosition(int position){
			this.position = position;
		}

		public void SetPosition(int position){
			this.position = position;
		}

		public void Start(){
			transform.position = initialPosition - new Vector3(100,((1.2f*(position)))*GetComponent<RectTransform>().rect.height,0);
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
}

