using UnityEngine;
using System.Collections;

public class MQPuppetObject : MonoBehaviour {
	
	public RabbitMQServer mocapServer;
	public RabbitMQServer positionServer;

	public string inputIP = "wearable.nccu.edu.tw";
	public string mocapTopic = "wise.mocap";
	public string positionTopic = "wise.position";
	public GUIStyle dialogStyle;
	public bool isSettingOk = false;

	// for camera focus on
	public bool isFocus = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {

		if (!isSettingOk) {
			GUILayout.BeginArea (new Rect (Screen.width * 0.25f, Screen.height * 0.35f, Screen.width * 0.5f, Screen.height * 0.3f), 
		                     dialogStyle);

			GUI.skin.label.fontStyle = FontStyle.Bold;
			GUI.skin.label.fontSize = 16;

			GUILayout.Label ("Puppet Setup");

			GUI.skin.label.fontStyle = FontStyle.Normal;
			GUI.skin.label.fontSize = 14;

			GUILayout.Space( 20.0f );

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Server IP:");
			inputIP = GUILayout.TextArea (inputIP);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Mocap Topic:");
			mocapTopic = GUILayout.TextArea (mocapTopic);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Position Topic:");
			positionTopic = GUILayout.TextArea (positionTopic);
			GUILayout.EndHorizontal ();

			GUILayout.Space( 20.0f );
			GUILayout.BeginHorizontal ();
			bool status = GUILayout.Button ("OK, Add Skeleton");
			GUILayout.EndHorizontal ();

			GUILayout.EndArea ();

			if( status )
			{
				isSettingOk = true;
				StartRabbitServers();
			}
		}
	}

	void StartRabbitServers () {
		mocapServer.serverIp = inputIP;
		mocapServer.topicName = mocapTopic;

		positionServer.serverIp = inputIP;
		positionServer.topicName = positionTopic;

		mocapServer.StartServer ();
		positionServer.StartServer ();
	}
}
