using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using SimpleJSON;


public class RabbitMQServer : MonoBehaviour {

	public bool StartOnLoad = false;
	public bool AskOnLoad = false;
	public string serverIp = "wearable.nccu.edu.tw";
	public string topicName = "demo.Exchange1";
	public bool serverStarted
	{
		get
		{
			if (mqChannel == null)
				return false;
			else
				return mqChannel.IsOpen;
		}
	}

	// Rabbit MQ things, not really know what they are
	ConnectionFactory factory;
	IModel mqChannel;
	string queueName;

	// storing messages
	List<string> messages;

	// Use this for initialization
	void Start () {

		messages = new List<string> ();

		if( StartOnLoad )
			StartServer();

	}

	public void StartServer () {
		factory = new ConnectionFactory() { Uri = "amqp://admin:admin@" + serverIp };
		factory.AutomaticRecoveryEnabled = true;

		IConnection connection = factory.CreateConnection();
		mqChannel = connection.CreateModel();
		connection.AutoClose = true;

		// setTopic name
		mqChannel.ExchangeDeclare( topicName , ExchangeType.Fanout);
		queueName = mqChannel.QueueDeclare().QueueName;
		mqChannel.QueueBind(queueName, topicName , "");

		Debug.Log ("Server Started [ " + serverIp + " ] ( " + topicName + " )" );

		startPushService ();
	}

	void startPushService () {
		
		EventingBasicConsumer consumer = new EventingBasicConsumer (mqChannel);
		consumer.Received += (ch, ea) =>
		{
			byte[] body = ea.Body;
			messages.Add( System.Text.Encoding.UTF8.GetString( body ) );
			mqChannel.BasicAck(ea.DeliveryTag, false);
		};

		string consumerTag = mqChannel.BasicConsume(queueName, false, consumer);

		// this can be use to canel push service
		// mqChannel.BasicCancel( consumerTag )
	}

	// only query one message
	public string getMessageString () {

		if (serverStarted == false)
			return null;
		
		BasicGetResult result = mqChannel.BasicGet (queueName, false);

		if (result == null)
			return null;
		else {
			byte[] body = result.Body;
			string returnStr = System.Text.Encoding.UTF8.GetString (body);
			mqChannel.BasicAck (result.DeliveryTag, false);

			return returnStr;
		}
	}

	// only query one message
	public JSONNode getMessageJson () {

		if (serverStarted == false)
			return null;
		
		BasicGetResult result = mqChannel.BasicGet (queueName, false);

		if (result == null)
			return null;
		else {
			byte[] body = result.Body;
			string returnStr = System.Text.Encoding.UTF8.GetString (body);
			mqChannel.BasicAck (result.DeliveryTag, false);

			JSONNode nodes = JSON.Parse (returnStr);
			return nodes;
		}
	}

	// give all messages
	public List<string> GetNewMessages() {

		List<string> returnList = new List<string> ();

		if (serverStarted == false)
			return returnList;
		
		int num = messages.Count;

		for (int i = 0; i < num; i++) {
			returnList.Add (messages [i]);
		}
		messages.RemoveRange (0, num);

		return returnList;
	}



	void OnApplicationQuit ()
	{
		mqChannel.Close ();
		Debug.Log ("Connection Close");
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
	
}