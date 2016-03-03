using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NccuWise;
using SimpleJSON;

public class MQPositionApplier : MonoBehaviour {
	
	// 從哪一個 rabbitServer 抓取資料
	public RabbitMQServer rabbitServer;
	public Transform applyTarget;
	
	public bool smoothData = true;

	private List<string> messages;
	public int messageBufferedLimit = 5;
	public int maxLoadPerFrame = 5;

	// for debug
	public int nowMsgAmount = 0;

	// Use this for initialization
	void Start () {
		
		messages = new List<string>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		// 從 rabbitServer 撈取新資料
		// 撈到的資料會先存在 messages 裡面
		SaveNewMessages ();
		
		// 看看 messages 裡面有沒有資料
		// 有的話就讀取資料，並且移動使用者
		int counter = 0;
		while( messages.Count > 0 )
		{
			string msg = messages[0];
			messages.RemoveAt(0);
			LoadJsonPos( msg );

			if( counter > maxLoadPerFrame )
				break;
		}

		nowMsgAmount = messages.Count;
	}
	
	void LoadJsonPos ( string jsonString )
	{
		// convert pure string to json node
		JSONNode nodes = JSON.Parse( jsonString );
		
		// format :
		//	{
		//		"p1x":0,
		//		"p1y":0,
		//		"p2x":0,
		//		"p2y":0,
		//		...
		//		...
		//	}
		
		
		// 檢查是否傳入訊息為 JSON 格式
		// 如果 nodes == null 表示並非 json 格式
		if( nodes == null )
		{
			Debug.Log( "OOps convert fail : not json format" );
			return;
		}

		/* JSON Format :
		 * 
		 * {
		 * 	 Position : 
		 *   {
		 * 		x: 111,
		 * 		y: 222,
		 * 		z: 102
		 * 	 }
		 * }
		 * 
	     */

		if (nodes ["Position"] != null) {
			string indexName = "Position";
			float posX = nodes[indexName]["x"].AsFloat;
			float posY = nodes[indexName]["y"].AsFloat;
			float posZ = nodes[indexName]["z"].AsFloat;

			Vector3 newPos = new Vector3( posX, posY, posZ );

			applyTarget.transform.localPosition = newPos;
		}
	}


	float moveTime = 0.2f;

	IEnumerator MoveSmoothly ( Transform moveTransform, Vector3 toPosition ) {
		int frames = (int)(moveTime / Time.fixedDeltaTime);
		float t = 0.0f;
		float step = 1.0f / (float)frames;
		
		Vector3 fromPos = moveTransform.position;
		for (int i=0; i<= frames; i++) {
			t += step;
			
			moveTransform.position = Vector3.Lerp( fromPos, toPosition, t );
			yield return new WaitForFixedUpdate ();
		}
	}
	
	// 這個 function 是從 rabbitServer 裡面抓取資料
	// 因為 rabbitServer 是在另一個 thread，所以無法主動傳資料給這個 script
	// 所以只好由這隻 script 去向他要資料
	// （這是 unity 程式的設計，只要有 new thread 就必須要這樣做 ... 不懂可以再問我，不過這問題也不重要才是ＸＤ）
	void SaveNewMessages ()
	{
		// 向 rabbitServer 索取新的資料
		// 如果 newMessages = null 表示 rabbitServer 還沒有初始化完成
		// 如果 newMessages.Count = 0 表示這段時間內沒有新資料進來
		List<string> newMessages = rabbitServer.GetNewMessages();
		
		// if server haven't started yet
		if( newMessages == null )
			return;
		
		for( int i=0; i< newMessages.Count; i++ )
		{
			// 這邊我有設定儲存的資料筆數的上限
			// 如果超過設定數值的話就捨棄這筆資料
			// 不過看來 ... 應該不要捨棄才對，所以這個應該要移除
			if( messages.Count < messageBufferedLimit )
			{
				messages.Add( newMessages[i] );
				Debug.Log( newMessages[i] );
			}
		}
	}

}
