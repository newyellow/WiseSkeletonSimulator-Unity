using UnityEngine;
using System.Collections;

public class DisplayingMessage : MonoBehaviour {


	public TextMesh caughtMessageDisplay;
	public TextMesh rotateEventDisplay;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ShowMessage ( string message ) {

		caughtMessageDisplay.text = message + '\n' + caughtMessageDisplay.text;
		caughtMessageDisplay.text = RemoveMessagesBeyondLine( caughtMessageDisplay.text, 8 );
	}

	void ShowRotateEvent( object[] args ) {

		string skeletonPointName = (string)args[0];
		Vector3 rotateValues = (Vector3)args[1];

		string textToAdd = "Rotate [ " + skeletonPointName + " ] ";
		textToAdd += "( " + rotateValues.x + "," + rotateValues.y + "," + rotateValues.z + ")";

		rotateEventDisplay.text = textToAdd + '\n' + rotateEventDisplay.text;
		rotateEventDisplay.text = RemoveMessagesBeyondLine( rotateEventDisplay.text, 8 );
	}

	string RemoveMessagesBeyondLine ( string textToRemove, int limitLine ) {

		string[] strings = textToRemove.Split( '\n' );

		string returnString = "";

		for( int i=0; i< limitLine; i++ )
		{
			if( i < strings.Length )
				returnString += strings[i] + '\n';
		}

		return returnString;
	}
}
