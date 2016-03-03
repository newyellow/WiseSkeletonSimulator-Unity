using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;


namespace NccuWise
{
	public class RabbitMQServerThread
	{
		ConnectionFactory factory;
		public bool runningKey = false; // use to terminate the thread
		int perFrameLimit = 50; // prevent lag
		string rabbitMQtopic = ""; // topic name
		string serverIp = "";

		
		// message buffered
		List<string> messages;
		
		public RabbitMQServerThread ( string inputIp, string topicName )
		{
			serverIp = inputIp;
			rabbitMQtopic = topicName;
			messages = new List<string>();
		}

		public List<string> GetNewMessages () {

			List<string> returnMessages = new List<string>();
			int count = 0;
			while( messages.Count > 0 )
			{
				returnMessages.Add( messages[0] );
				messages.RemoveAt( 0 );

				++count;
				if( count > perFrameLimit )
					break;
			}

			return returnMessages;

		}

		public void Run(){
			
			Debug.Log("Server");
			// init the server, set key to true
			runningKey = true;

			factory = new ConnectionFactory() { Uri = "amqp://admin:admin@" + serverIp };


			IConnection connection = factory.CreateConnection();
			IModel channel = connection.CreateModel();

			channel.ExchangeDeclare( rabbitMQtopic , "fanout");
					
			string queueName = channel.QueueDeclare().QueueName;
					
			channel.QueueBind(queueName, rabbitMQtopic , "");
			QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
			channel.BasicConsume(queueName, true, consumer);
					
			Debug.Log(" [*] Waiting for logs. To exit press CTRL+C");
					
					
			while (true) {

				// terminate the thread
				if( !runningKey )
				{
					Debug.Log( "Thread Terminated" );
					break;
				}

				var ea = (BasicDeliverEventArgs) consumer.Queue.Dequeue ();
				var body = ea.Body;
				var message = Encoding.UTF8.GetString (body);

				// got new message
				messages.Add( (string)message );

			}
					
		}
	}
}