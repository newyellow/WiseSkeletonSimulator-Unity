using UnityEngine;
using System.Collections;

public class RabbitMQSenderManager : MonoBehaviour {

	public RabbitMQSenderServer mocapServer;
	public RabbitMQSenderServer positionServer;

	string ipAddress = "wearable.nccu.edu.tw";

	string mocapTopic = "wise.mocap";
	string positionTopic = "wise.position";

	bool isStarted = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		// if no auto start, ask user input
		if( isStarted ) return;
			
		GUILayout.BeginArea( new Rect( Screen.width * 0.25f, Screen.height * 0.2f, Screen.width * 0.5f, Screen.height * 0.4f ) );

		GUILayout.Label("Server IP :");
		ipAddress = GUILayout.TextField( ipAddress );
			
		GUILayout.Label("Mocap Topic :" );
		mocapTopic = GUILayout.TextField( mocapTopic );

		GUILayout.Label("Position Topic :" );
		positionTopic = GUILayout.TextField( positionTopic );
			
		if( GUILayout.Button("Connect") )
		{
			isStarted = true;
			mocapServer.serverIP = ipAddress;
			mocapServer.channelName = mocapTopic;
			mocapServer.StartServer();

			positionServer.serverIP = ipAddress;
			positionServer.channelName = positionTopic;
			positionServer.StartServer();
		}
			
		GUILayout.EndArea();
	}
}
