using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SendObjPosition : MonoBehaviour {

	public RabbitMQSenderServer sender;
	public Transform sendingObj;

	string messageToShow = "";

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		// if server started
		if( !sender.IsReady() ) return;

		JSONClass json = new JSONClass();

		// send position
		Vector3 objPos = sendingObj.position;

		string indexName = "Position";
		json [indexName] ["x"].AsFloat = objPos.x;
		json [indexName] ["y"].AsFloat = objPos.y;
		json [indexName] ["z"].AsFloat = objPos.z;

		messageToShow = "Position : ( " + objPos.x.ToString ("F2") + ", " + objPos.y.ToString ("F2") + ", " + objPos.z.ToString ("F2") + " )";

		sender.SendMessageToServer( json.ToString() );
	}

	void OnGUI () {
		GUILayout.BeginArea( new Rect( Screen.width * 0.5f, 0.0f, Screen.width * 0.5f, Screen.height * 0.2f ) );
		GUILayout.Label( messageToShow );
		GUILayout.EndArea ();
	}

}
