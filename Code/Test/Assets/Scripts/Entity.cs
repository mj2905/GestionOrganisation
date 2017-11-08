using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity {
	/*
	private List<Coordinate> coordinates;

	[SerializeField]
	private Material material;

	private Vector3 CLerp(Vector2 v1, Vector2 v2, Vector3 inter) {
		return new Vector3 (Mathf.Lerp (v1.x, v2.x, inter.x), Mathf.Lerp (v1.y, v2.y, inter.y), inter.z);
	}

	public Entity() {
		this.coordinates = new List<Coordinate>();
		//coordinates.Add (new CoordinateMap (0.5f, 0.5f));
		//coordinates.Add (new CoordinateMap (0.8f, 0.8f));
		//coordinates.Add (new CoordinateMap (0.0f, 1));
		//coordinates.Add (new CoordinateMap (0.1f, 0.8f));
	}

	// Use this for initialization
	void Start () {
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer> ();
		lineRenderer.widthMultiplier = 0.1f;
		lineRenderer.positionCount = coordinates.Count+1;
		lineRenderer.material = material;
	}

	// Update is called once per frame
	void Update () {
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		Vector2 upLeft = new Vector2 (transform.position.x - gameObject.GetComponent<Renderer> ().bounds.size.x / 2, 
				transform.position.y - gameObject.GetComponent<Renderer> ().bounds.size.y / 2);
		Vector2 bottomRight = new Vector2 (transform.position.x + gameObject.GetComponent<Renderer> ().bounds.size.x / 2, 
			transform.position.y + gameObject.GetComponent<Renderer> ().bounds.size.y / 2);
		
		for (int i = 0; i < coordinates.Count; i++)
		{
			lineRenderer.SetPosition(i, CLerp(upLeft, bottomRight, coordinates[i].toMapVector()));
		}

		if (coordinates.Count > 0) {
			lineRenderer.SetPosition (coordinates.Count, CLerp (upLeft, bottomRight, coordinates [0].toMapVector ()));
		}
	}
	*/
}
	