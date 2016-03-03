using UnityEngine;
using System.Collections;


public class AxisCameraRotateFollower : MonoBehaviour {

	public Transform copyTarget;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		UpdatePosition ();

	}

	void UpdatePosition () {
		transform.localRotation = copyTarget.localRotation;
	}


}
