using UnityEngine;
using System.Collections;

public class SwitchRotationButton : MonoBehaviour {


	bool status = true;
	public TextMesh displayText;
	public RabbitMQDataCatcher rotateController;

	// Use this for initialization
	void Start () {
		SetDisplayText();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton () {
		status = !status;

		rotateController.rotateStatus = status;

		SetDisplayText();
		StartCoroutine( "RotateAround" );
	}

	void SetDisplayText () {

		if( status )
			displayText.text = "Use Rotation";
		else
			displayText.text = "Use LocalRotation";

	}

	IEnumerator RotateAround () {

		for( int i=0; i<10; i++ )
		{
			gameObject.transform.Rotate( new Vector3( 0.0f, 0.0f, 36.0f ) );
			yield return new WaitForSeconds( 0.03f );
		}
	}
}
