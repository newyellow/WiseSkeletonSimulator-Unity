using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NccuWise;

// use json
using SimpleJSON;


public class RabbitMQDataCatcher : MonoBehaviour {

	public Transform skeletonRoot;
	public Transform[] skeletonPoints;
	public RabbitMQServer rabbitServer;
	public GameObject debugDisplayer; // just for displaying raw data on screen

	// use absolute rotation or local rotation
	public bool rotateStatus = false;


	// buffered messages
	List<string> messages;
	public int maxBufferAmount = 10;

	// for doing Lerp
	Quaternion[] lastRots;
	Quaternion[] nextRots;

	// Use this for initialization
	void Start () {

		messages = new List<string>();
		lastRots = new Quaternion[ skeletonPoints.Length ];
		nextRots = new Quaternion[ skeletonPoints.Length ];
	
		// init rots
		for( int i=0; i< skeletonPoints.Length; i++ )
		{
			if( rotateStatus )
			{
				lastRots[i] = skeletonPoints[i].rotation;
				nextRots[i] = skeletonPoints[i].rotation;
			}
			else
			{
				lastRots[i] = skeletonPoints[i].localRotation;
				nextRots[i] = skeletonPoints[i].localRotation;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	

		SaveNewMessages ();


		// non-lerp
		if( messages.Count > 0 )
		{
			// kill data overload
			while( messages.Count > maxBufferAmount )
			{
				messages.RemoveAt(0);
			}

			// show message on screen
			if( debugDisplayer != null )
				debugDisplayer.SendMessage( "ShowMessage", messages[0] );
			MapDataToSkeleton( (string)messages[0] );
			messages.RemoveAt(0);
		}




		// lerp

		// get new data
//		if( lerpCounter >= lerpFrames )
//		{
//			// put in last rots
//			for( int i=0; i< skeletonPoints.Length; i++ )
//			{
//				if( rotateStatus )
//					lastRots[i] = skeletonPoints[i].rotation;
//				else
//					lastRots[i] = skeletonPoints[i].localRotation;
//			}
//
//			// get first message and turn to rots
//			if( messages.Count > 0 )
//			{
//				nextRots = JsonToRots( (string)messages[0] );
//				debugDisplayer.SendMessage( "ShowMessage", messages[0] );
//				messages.RemoveAt(0);
//
//				lerpCounter = 0;
//			}
//		}
//
//		// doing lerp
//		// only skip if no data got
//		if( lerpCounter < lerpFrames )
//		{
//			float t = (float)lerpCounter/(float)(lerpFrames-1);
//
//			for( int i=0; i< skeletonPoints.Length; i++ )
//			{
//				if( rotateStatus )
//					skeletonPoints[i].rotation = Quaternion.Lerp( lastRots[i], nextRots[i], t );
//				else
//					skeletonPoints[i].localRotation = Quaternion.Lerp( lastRots[i], nextRots[i], t );
//			}
//
//			lerpCounter++;
//		}
	}

	void SaveNewMessages ()
	{
		List<string> newMessages = rabbitServer.GetNewMessages();

		// if server haven't started yet
		if( newMessages == null )
			return;

		for( int i=0; i< newMessages.Count; i++ )
			messages.Add( newMessages[i] );
	}


	// map to skeleton Lerp Edition
	Quaternion[] JsonToRots ( string jsonString )
	{
		// convert pure string to json node
		JSONNode nodes = JSON.Parse( jsonString );

		Quaternion[] returnRots = new Quaternion[ skeletonPoints.Length ];
		
		// search is there data for skeletonNodes
		for( int i=0; i< skeletonPoints.Length; i++ )
		{
			string indexName = skeletonPoints[i].name;
			
			if( jsonString.Contains( indexName ) )
			{
				Vector3 newRotation = new Vector3( nodes[ indexName ]["x"].AsFloat, nodes[ indexName ]["y"].AsFloat, nodes[ indexName ]["z"].AsFloat );
				//StartCoroutine( rotateTargetPoint( i, newRotation ) );
				
				returnRots[i] = Quaternion.Euler( newRotation );
				// show rotate event
				if( debugDisplayer != null )
					debugDisplayer.SendMessage( "ShowRotateEvent", new object[]{ indexName, newRotation } );
			}
			else
			{
				// if no data caught, use the original data
				if( rotateStatus )
					returnRots[i] = skeletonPoints[i].rotation;
				else
					returnRots[i] = skeletonPoints[i].localRotation;
			}
		}

		return returnRots;
	}

	// get the json string and call rotation function
	void MapDataToSkeleton ( string jsonString ) {

		// convert pure string to json node
		JSONNode nodes = JSON.Parse( jsonString );


		// search is there data for skeletonNodes
		for( int i=0; i< skeletonPoints.Length; i++ )
		{
			string indexName = skeletonPoints[i].name;

			if( jsonString.Contains( indexName ) )
			{
				Vector3 newRotation = new Vector3( nodes[ indexName ]["x"].AsFloat, nodes[ indexName ]["y"].AsFloat, nodes[ indexName ]["z"].AsFloat );
				//StartCoroutine( rotateTargetPoint( i, newRotation ) );

				if( rotateStatus )
					skeletonPoints[i].rotation = Quaternion.Euler( newRotation );
				else
					skeletonPoints[i].localRotation = Quaternion.Euler( newRotation );

				// show rotate event
				if( debugDisplayer != null )
					debugDisplayer.SendMessage( "ShowRotateEvent", new object[]{ indexName, newRotation } );
			}
		}

		// search is there data for movement
		string posIndexName = "Position";

		if( jsonString.Contains( posIndexName ) )
		{
			Vector3 newPos = new Vector3( nodes[ posIndexName ]["x"].AsFloat, nodes[ posIndexName ]["y"].AsFloat, nodes[ posIndexName ]["z"].AsFloat );

			skeletonRoot.position = newPos;

			// show movement event
			if( debugDisplayer != null )
				debugDisplayer.SendMessage( "ShowRotateEvent", new object[]{ "Movement", newPos } );
		}
	}


	void OnGUI () {

	}
	// this methd is just for display rotation
	// make the bone turn smoothly


//	IEnumerator rotateTargetPoint ( int skeletonPointIndex, Vector3 toRotation )
//	{
//		Quaternion fromRot = skeletonPoints[ skeletonPointIndex ].localRotation;
//		Quaternion toRot = Quaternion.Euler( toRotation );
//
//		float rotateTime = 0.5f;
//		int frames = (int)(rotateTime * 30.0f); // if 30 fps
//
//		for( int i=0; i< frames; i++ )
//		{
//			skeletonPoints[ skeletonPointIndex ].localRotation = Quaternion.Slerp( fromRot, toRot, (float)i/(float)frames );
//			yield return new WaitForSeconds( 0.03f );
//		}
//	}
	
}
