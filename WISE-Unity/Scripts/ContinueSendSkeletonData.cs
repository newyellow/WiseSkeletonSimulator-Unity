using UnityEngine;
using System.Collections;

public class ContinueSendSkeletonData : MonoBehaviour {

	public RabbitMQSenderServer sender;
	public bool start = false;
	public float delayedTime = 0.03f;

	public int counter = 0;
	float angle = 0.0f;


	// Use this for initialization
	void Start () {
	
		StartCoroutine( "SendTestMessage" );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SendTestMessage ()
	{
		bool status = false;
		float addValue = 0.1f;

		while( true )
		{
			// make arm goes up or down
			if( status )
			{
				angle += addValue;
				if( angle >= 180.0f ) status = !status;
			}
			else
			{
				angle -= addValue;
				if( angle <= 0.0f ) status = !status;
			}

			if( sender.IsReady() && start )
			{
				string message = "{ \"LeftArm\":{ x:"+angle+", y:90, z:90 }, \"index\":"+counter+" }";

				// test pos
//				string message = "{ \"Position\":{ x:"+angle*0.01+", y:0, z:0 } }";
				sender.SendMessageToServer( message );
				counter++;
			}

			yield return new WaitForSeconds( delayedTime );
		}
	}
}
