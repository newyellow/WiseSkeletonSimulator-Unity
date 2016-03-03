using UnityEngine;
using System.Collections;
using NccuWise;

using System.Collections.Generic;

public class SimpleDataChecker : MonoBehaviour {

	public RabbitMQServer server;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if( !server.serverStarted ) return;

		List<string> dataGot = server.GetNewMessages();

		for( int i=0; i< dataGot.Count; i++ )
		{
			Debug.Log( gameObject.name + " : " + dataGot[i] );
		}
	}
}
