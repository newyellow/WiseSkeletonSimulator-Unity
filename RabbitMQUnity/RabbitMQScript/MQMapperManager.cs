using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NccuWise;

using SimpleJSON;

public class MQMapperManager : MonoBehaviour {

	public RabbitMQServer rabbitServer;
	public GameObject skeletonObj;
	public Transform skeletonRoot; // use for positions
	private MQMapperItem[] mappers;

	// all saved messages
	private List<string> messages;
	public int messageBufferedLimit = 50;
	public int maxLoadPerFrame = 5;
	
	public int nowMsgAmount = 0;

	void Awake ()
	{
		mappers = gameObject.GetComponentsInChildren<MQMapperItem>();
	}

	// Use this for initialization
	void Start () {

		messages = new List<string>();

		// find transforms in skeletonObj
		ResetSource ();

	}
	
	// Update is called once per frame
	void Update () {

		SaveNewMessages ();

		// apply server data
		int counter = 0;
		while( messages.Count > 0 )
		{
			string msg = messages[0];
			messages.RemoveAt(0);
			LoadJsonRots( msg );

			if( counter >= maxLoadPerFrame )
				break;
		}

		nowMsgAmount = messages.Count;
	}
	
	void LoadJsonRots ( string jsonString )
	{
		// convert pure string to json node
		JSONNode nodes = JSON.Parse( jsonString );

		// if convert failed : not json format
		if( nodes == null )
		{
			Debug.Log( "OOps convert fail : not json format" );
			return;
		}
		
		// search is there data for skeletonNodes
		for( int i=0; i< mappers.Length; i++ )
		{
			string indexName = mappers[i].name;

			if( nodes[ indexName ] != null )
			{
				Vector3 newRotation = new Vector3( nodes[ indexName ]["x"].AsFloat, nodes[ indexName ]["y"].AsFloat, nodes[ indexName ]["z"].AsFloat );
				mappers[i].ApplyJsonRot( newRotation );
			}
		}

		// check position
		if( nodes[ "Position" ] != null )
		{
			string indexName = "Position";
			Vector3 newPos = new Vector3( nodes[ indexName ]["x"].AsFloat, nodes[ indexName ]["y"].AsFloat, nodes[ indexName ]["z"].AsFloat );
			skeletonRoot.position = newPos;
		}
	}

	void SaveNewMessages ()
	{
		List<string> newMessages = rabbitServer.GetNewMessages();
		
		// if server haven't started yet
		if( newMessages == null )
			return;

		for( int i=0; i< newMessages.Count; i++ )
		{
			// if meet limit, dont save
			if( messages.Count < messageBufferedLimit )
				messages.Add( newMessages[i] );
		}
	}

	void ResetSource ()
	{
		// get scripts from children
		Component[] comps = transform.GetComponentsInChildren<MQMapperItem>();
		mappers = new MQMapperItem[ comps.Length ];
		for( int i=0; i< comps.Length; i++ )
		{
			mappers[i] = (MQMapperItem)comps[i];
			Debug.Log( mappers[i].name );
		}
		
		// search for objs by name in source
		for( int i=0; i< mappers.Length; i++ )
		{
			Transform tempSource = FindChildNested( skeletonObj.transform, mappers[i].name );
			
			if( tempSource != null )
				mappers[i].monitorTransform = tempSource;
		}
		
	}


	// recursive call
	Transform FindChildNested ( Transform obj, string searchName )
	{
		Transform result = null;
		string lowerSearchName = searchName.ToLower();
		
		// lower find
		for( int i=0; i< obj.childCount; i++ )
		{
			if( obj.GetChild(i).name.ToLower() == lowerSearchName )
				result = obj.GetChild(i);
		}
		
		
		
		if( result != null )
		{
			return result;
		}
		else if( obj.childCount == 0 )
		{
			return null;
		}
		else
		{
			for( int i=0; i< obj.childCount; i++ )
			{
				result = FindChildNested( obj.GetChild(i), searchName );
				
				if( result != null )
					break;
			}
			
			return result;
		}
	}
}
