using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using NccuWise;

public class RabbitMQServer : MonoBehaviour {

	public bool StartOnLoad = false;
	public bool AskOnLoad = false;
	public string serverIp = "wearable.nccu.edu.tw";
	public string topicName = "demo.Exchange1";
	public bool showMessageLog = false;
	RabbitMQServerThread serverThread;
	Thread t;
	public bool serverStarted = false;
	
	// Use this for initialization
	void Start () {

		if( StartOnLoad )
			StartServer();

	}

	public void StartServer () {

		serverThread = new RabbitMQServerThread( serverIp, topicName );
		t = new Thread (serverThread.Run);
		t.Start ();
		Debug.Log(" [*] Waiting for logs. To exit press CTRL+C");

		serverStarted = true;
	}


	// Update is called once per frame
	void Update () {
		
	}

	void OnApplicationQuit ()
	{
		serverThread.runningKey = false;
	}

	void OnGUI () {

		if (AskOnLoad && !serverStarted) {

			GUILayout.BeginArea( new Rect( Screen.width * 0.3f, Screen.height * 0.2f, Screen.width * 0.4f, Screen.height * 0.6f ) );

			GUILayout.Label( "Server IP :" );
			serverIp = GUILayout.TextField( serverIp );

			GUILayout.Label( "Topic Name :" );
			topicName = GUILayout.TextField( topicName );

			if( GUILayout.Button( "Connect" ) )
				StartServer();

			GUILayout.EndArea();
		}
	}

	public List<string> GetNewMessages () {

		// for testing
		if( showMessageLog )
		{
			List<String> msgs = serverThread.GetNewMessages();
			for( int i=0; i< msgs.Count; i++ )
				Debug.Log( "Got Msg:" + msgs[i] );
		}

		//Debug.Log( "get message request" );
		if( !serverStarted )
			return null;
		else
			return serverThread.GetNewMessages();
	}
	
}