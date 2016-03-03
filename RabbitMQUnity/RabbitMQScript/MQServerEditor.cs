using UnityEngine;
using System.Collections;

public class MQServerEditor : MonoBehaviour {

	public static string serverIP = "wearable.nccu.edu.tw";
	public static string topicName = "demo.EXCHANGE1";
	private string defaultIp = "192.168.1.161";
	//private string defaultTopic = "demo.EXCHANGE1";
	public bool useDefaultIp = false;

	public GameObject[] RabbitMQServers;
	bool isServerStart = false;


	void Awake ()
	{
		if( useDefaultIp )
			serverIP = defaultIp;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI ()
	{
		if( useDefaultIp ) return;
		if( isServerStart ) return;


		GUILayout.BeginArea( new Rect( Screen.width * 0.2f, Screen.height * 0.3f, Screen.width * 0.6f, Screen.height * 0.4f ) );

		GUILayout.BeginVertical();

		GUILayout.Label( "Setting Server IP : " );
		serverIP = GUILayout.TextField( serverIP );

		GUILayout.Label( "Setting Topic Name : " );
		topicName = GUILayout.TextField( topicName );

		if( GUILayout.Button( "Connect" ) )
		{
			for( int i=0; i< RabbitMQServers.Length; i++ )
			{
				RabbitMQServers[i].SendMessage( "StartServer" );
			}
			isServerStart = true;
		}

		GUILayout.EndVertical();

		GUILayout.EndArea();
	}
}
