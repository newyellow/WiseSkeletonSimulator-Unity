using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SendWholeSkeletonData : MonoBehaviour {

	public RabbitMQSenderServer sender;
	public Transform sendingRoot;
	public Transform[] sendingPoints;

	private string messageToShow;

	// Use this for initialization
	void Start () {
		sendingPoints = sendingRoot.GetComponentsInChildren<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if( !sender.IsReady() ) return;

		JSONClass json = new JSONClass();
		messageToShow = "";

		for( int i=0; i< sendingPoints.Length; i++ )
		{
			string pointName = sendingPoints[i].name;
			Vector3 pointValues = sendingPoints[i].transform.rotation.eulerAngles;

			json[ pointName ][ "x" ].AsFloat = pointValues.x;
			json[ pointName ][ "y" ].AsFloat = pointValues.y;
			json[ pointName ][ "z" ].AsFloat = pointValues.z;

			messageToShow += pointName + " :( " + pointValues.x.ToString("F2") + "," + pointValues.y.ToString("F2") + "," + pointValues.z.ToString("F2") + " ) \n";
		}

		sender.SendMessageToServer( json.ToString() );
	}

	void OnGUI ()
	{
		GUILayout.BeginArea( new Rect( 0.0f, 0.0f, Screen.width * 0.6f, Screen.height * 1.0f ) );
		GUILayout.Label( messageToShow );
		GUILayout.EndArea ();
	}
}
