using UnityEngine;
using System.Collections;

public class MQMapperItem : MonoBehaviour {

	public Transform monitorTransform;
	
	// add an rotation on
	public Vector3 AxisAdjust;
	public Vector3 ModelAdjust;

	// use for mirror
	private Vector3 originScale;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ApplyJsonRot ( Vector3 jsonRot )
	{
		if( monitorTransform == null ) return;

		//Quaternion newRot = Quaternion.Euler( AxisAdjust ) * Quaternion.Euler( jsonRot ) * Quaternion.Euler( ModelAdjust );
		Quaternion newRot = Quaternion.Euler( jsonRot );
		monitorTransform.rotation = newRot;
	}
}
