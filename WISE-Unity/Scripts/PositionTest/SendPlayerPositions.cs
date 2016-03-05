using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SendPlayerPositions : MonoBehaviour {

	public RabbitMQSenderServer rabbitSender;
	public Transform[] players;

	public Vector2 senderXrange;
	public Vector2 senderYrange;

	public Vector2 objXrange;
	public Vector2 objYrange;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if ( rabbitSender.IsReady() ) {

			
			JSONClass json = new JSONClass();


			// for testing, only send player0
			for( int i=0; i< players.Length ; i++ )
			{

				Vector2 pos = remappingPositionData( players[i].position.x, players[i].position.z );

				json["p" + (i+1).ToString() + "x"].AsFloat = pos.x;
				json["p" + (i+1).ToString() + "y"].AsFloat = pos.y;


			}
			rabbitSender.SendMessageToServer( json.ToString() );
		}
	}

	Vector2 remappingPositionData ( float inputX, float inputY ) {

//		float senderXdiff = senderXrange.y - senderXrange.x;
//		float senderYdiff = senderYrange.y - senderYrange.x;

		float objXdiff = objXrange.y - objXrange.x;
		float objYdiff = objYrange.y - objYrange.x;


		float returnX = 0.0f;
		float returnY = 0.0f;


		// if out of range
		if (inputX < objXrange.x || inputX > objXrange.y)
			returnX = -1.0f;
		// remap
		else {

			float xRatio = (inputX - objXrange.x) / objXdiff;
			returnX = Mathf.Lerp( senderXrange.x, senderXrange.y, xRatio );
		}


		// if out of range
		if (inputY < objYrange.x || inputY > objYrange.y)
			returnY = -1.0f;
		// remap
		else {
			
			float yRatio = (inputY - objYrange.x) / objYdiff;
			returnY = Mathf.Lerp( senderYrange.x, senderYrange.y, yRatio );
		}

		return new Vector2( returnX, returnY );
	}
}
