using UnityEngine;
using System.Collections;

public class AlwaysLookAtCamera : MonoBehaviour {

	public Camera theCam;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.LookAt (theCam.transform.position);
	}
}
