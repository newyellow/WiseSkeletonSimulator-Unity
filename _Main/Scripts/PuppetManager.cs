using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuppetManager : MonoBehaviour {

	public GameObject puppetPrefab;

	public List<MQPuppetObject> puppets;
	List<MQPuppetObject> puppetsToRemove; // for temp save
	
	public Transform cameraLookatPoint;

	// for display
	public GUIStyle blockStyle;

	bool isAddingNewPuppet = false;
	bool buttonPressed = false;
	// Use this for initialization
	void Start () {
		puppets = new List<MQPuppetObject> ();
		puppetsToRemove = new List<MQPuppetObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (buttonPressed)
			AddSkeleton ();

		// is setting ok, create puppet finish
		if (puppets.Count > 0) {
			if (puppets [puppets.Count - 1].isSettingOk)
				isAddingNewPuppet = false;
		}

		// remove puppetsToRemove
		while( puppetsToRemove.Count > 0 ) {
			MQPuppetObject tempObj = puppetsToRemove[0];
			puppets.Remove( tempObj );
			puppetsToRemove.Remove( tempObj );

			Destroy ( tempObj.gameObject );
		}

		// make camera focus
		UpdateCameraLookAtPoint ();
	}

	void OnGUI () {

		buttonPressed = false;

		GUILayout.BeginArea( new Rect( 0.0f, 0.0f, Screen.width * 0.25f, Screen.height ) );

		// show the button
		if (!isAddingNewPuppet)
			buttonPressed = GUILayout.Button ("Add Skeleton");
		else
			GUILayout.Space (20.0f);

		// show all the skeleton
		for( int i=0; i< puppets.Count; i++ )
		{
			if( !puppets[i].isSettingOk ) continue;

			GUILayout.BeginVertical(blockStyle);

			GUILayout.Label( "Puppet " + i );
			GUILayout.Space( 10.0f );

			GUILayout.Label( "IP: " + puppets[i].inputIP );
			GUILayout.Label( "Mocap Topic: " + puppets[i].mocapTopic );
			GUILayout.Label( "Position Topic: " + puppets[i].positionTopic );

			GUILayout.Space( 10.0f );

			puppets[i].isFocus = GUILayout.Toggle( puppets[i].isFocus, "Camera Focus" );

			if( GUILayout.Button( "Remove Puppet" ) )
				puppetsToRemove.Add( puppets[i] );

			GUILayout.EndVertical();
		}



		GUILayout.EndArea ();
	}

	void AddSkeleton () {
		GameObject tempPuppet = GameObject.Instantiate (puppetPrefab);
		puppets.Add (tempPuppet.GetComponent<MQPuppetObject> ());
		isAddingNewPuppet = true;
	}

	void UpdateCameraLookAtPoint () {
		Vector3 posSum = Vector3.zero;
		int counter = 0;

		for( int i=0; i< puppets.Count; i++ )
		{
			if( !puppets[i].isSettingOk ) continue;

			if( puppets[i].isFocus )
			{
				posSum += puppets[i].transform.position;
				counter++;
			}
		}

		if( counter > 0 )
			cameraLookatPoint.transform.position = posSum / (float)counter;
	}
}
