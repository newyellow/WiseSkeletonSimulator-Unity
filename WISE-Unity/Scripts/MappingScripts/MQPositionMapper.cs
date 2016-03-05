using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class MQPositionMapper : MonoBehaviour {
	
	// 從哪一個 rabbitServer 抓取資料
	public RabbitMQServer rabbitServer;
	
	// New : 觀眾的 player 物件
	public Transform userPlayer;
	int playerIndex = 0; // 現在這個是第幾 player
	public bool isPlayerIndexSettingUp = false;
	
	// 其他的 player : 就那些影子人物件
	public Transform[] movePlayers; // use for positions
	
	public bool smoothData = true;
	public float smoothAmount = 10;
	List<Vector2>[] posHistory;
	
	public Vector2 inputXrange;
	public Vector2 inputYrange;
	
	public Vector2 sceneXrange;
	public Vector2 sceneYrange;
	
	
	//public float moveScaleRatio = 1.0f;
	
	// all saved messages
	private List<string> messages;
	public int messageBufferedLimit = 5;
	
	
	// debug options, use for RabbitTestSender
	public bool showDebugPoints = false;
	public GameObject[] debugPointsPrefabs;
	
	void Awake ()
	{
		posHistory = new List<Vector2>[ movePlayers.Length ];
		
		for (int i=0; i< posHistory.Length; i++)
			posHistory [i] = new List<Vector2> ();
	}
	
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
		while( messages.Count > 0 )
		{
			string msg = messages[0];
			messages.RemoveAt(0);
			LoadJsonPos( msg );
			
			// 限制每個 frame 最多讀取幾筆資料
			// 防止一個 frame 裡面有太多比資料，造成程式 lag
			// 不過現在是關掉的狀態
			
			//			// max load 5 jsons per update
			//			if( counter++ > 5 )
			//				break;
		}
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
		
		// 檢查這筆 Json 資料是哪一個 player 的資料
		// 然後把這筆 json 資料挾帶的 position 資料套用到 player 身上
		//
		// 然後這支程式其實很笨，他並不知道這筆 json 是哪一個 player 的資料
		// 所以會從 p1 開啟檢查到 p5，看是哪一個 player 的資料
		for (int i=0; i< movePlayers.Length; i++) {
			
			// 攝影機組會傳入 p1x, p1y, p2x, p2y ... 等參數
			// p1x 表示是 player1 的 x，p2x 則是 player2 的 x，以此類推
			// 由於攝影機組是從 1 開始編號到 5，所以下面 i+1 表示
			string xIndexName = "p" + (i+1).ToString() + "x";
			string yIndexName = "p" + (i+1).ToString() + "y";
			
			// 看看有沒有 pix 這筆資料，沒有的話表示這筆 json 內沒有這個 player 的資料
			// 就繼續檢查下一筆
			if( nodes[xIndexName] == null || nodes[yIndexName] == null )
				continue;
			
			// 暫存攝影機組傳來的 input 資料
			// 現在 inputX, inputY 的數值是攝影機組傳來的座標
			float inputX = nodes[xIndexName].AsFloat;
			float inputY = nodes[yIndexName].AsFloat;
			
			
			// 這是給攝影機組 debug 用的，實際使用時應該把這個關掉
			if( showDebugPoints ) {
				
				Debug.Log( "input: (" + inputX + "," + inputY + ")" );
				Vector2 mappingData = mappingInputPosition( inputX, inputY );
				Debug.Log( "mapped: " + mappingData );
				float posX = mappingData.x;
				float posY = mappingData.y;
				
				Instantiate( debugPointsPrefabs[i] , new Vector3( posX, 11.0f, posY), Quaternion.identity );
			}
			
			
			// posHistory 是用來做平滑化使用的
			// smoothAmount 表示要平滑化的資料筆數
			//
			// 假設 smoothAmount = 5
			// 那就會把攝影組傳來的 近 5 筆資料平均後 套用到 player 身上
			//
			// 如果設定成 smoothAmount = 1 就是完全不平滑， 0 的話就不會動了
			posHistory[i].Add( new Vector2( inputX, inputY ) );
			
			// 超出 smoothAmount 的筆數刪掉
			while( posHistory[i].Count > smoothAmount )
				posHistory[i].RemoveAt(0);
			
			
			// 這邊把 inputX inputY 清空，因為上面已經存到 posHistory 裡面了
			inputX = 0.0f;
			inputY = 0.0f;
			
			// 如果有勾 smoothData 就把資料平均
			if( smoothData )
			{
				for( int j=0; j< posHistory[i].Count; j++ )
				{
					inputX += posHistory[i][j].x;
					inputY += posHistory[i][j].y;
				}
				
				inputX /= smoothAmount;
				inputY /= smoothAmount;
			}
			else
				// 沒勾 smoothData 就直接套用最後一筆
			{
				int lastIndex = posHistory[i].Count-1;
				inputX = posHistory[i][ lastIndex ].x;
				inputY = posHistory[i][ lastIndex ].y;
			}
			
			// 把視訊組傳來的座標轉換成 Unity 的座標
			Vector2 remappingData = mappingInputPosition( inputX, inputY );
			Vector3 newPos = movePlayers[i].position;
			newPos.x = remappingData.x;
			newPos.z = remappingData.y;
			
			// 套用新的座標，呼叫平滑移動 function，讓他不會瞬間移動
			StartCoroutine( MoveSmoothly( movePlayers[i], newPos ) );
			
		}
	}
	
	/*
	這個 function 是用來轉換座標軸的
	把攝影機組那邊的座標轉換成 unity 裡面的座標

	比如說 攝影機偵測使用者 x 軸的範圍是 0 ~ 1000
	而相對 場景中使用者在 unity 裡的位置是 0.0f ~ 20.0f

	這時候視訊組輸入 x=100 時，就會回傳 x= 2.0f
	*/
	Vector2 mappingInputPosition ( float inputX, float inputY ) {
		
		// calculate diff
		float inputXdiff = inputXrange.y - inputXrange.x;
		float inputYdiff = inputYrange.y - inputYrange.x;
		
		// calculate ratio
		float inputXratio = (inputX - inputXrange.x) / inputXdiff;
		float inputYratio = (inputY - inputYrange.x) / inputYdiff;
		
		// move scene objs
		float posX = Mathf.Lerp( sceneXrange.x, sceneXrange.y, inputXratio );
		float posY = Mathf.Lerp( sceneYrange.x, sceneYrange.y, inputYratio );
		
		return new Vector2 (posX, posY);
	}
	
	
	/*
	這個 function 是讓資料傳進來後，移動不要很跳
	因為假設視訊組有點 lag，每秒只有 10 frame
	如果一收到資料就直接把位置 set 過去，看起來會瞬間移動
	所以這個 function 就是讓使用者不要飛過去，而是緩緩移動過去

	而 moveTime 則是這個移動會花多久
	這個 moveTime 要根據攝影機組的傳輸 frame rate 來訂
	假設他們有 10 fps，那每個 frame 之間就有 0.1 秒的間隔
	這時候 moveTime 就應該要設為 0.1f，以此類推
	*/
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
	
	
	// new : 詢問這是第幾 player
	string inputPlayerIndex = ""; // 只是暫存輸入數值用的變數
	void OnGUI () {
		
		if (!isPlayerIndexSettingUp) {
			
			GUILayout.BeginArea( new Rect( Screen.width * 0.3f, Screen.height * 0.2f, Screen.width * 0.4f, Screen.height * 0.6f ) );
			
			GUILayout.Label( "Player Index ( 1 ~ 5 )" );
			inputPlayerIndex = GUILayout.TextField( inputPlayerIndex );
			
			if( GUILayout.Button( "Confirm User Index" ) )
			{
				isPlayerIndexSettingUp = true;
				playerIndex = int.Parse( inputPlayerIndex ) -1; // 輸入時從 1 開始，所以這邊 -1
				SetupPlayerIndex ();
			}
			
			GUILayout.EndArea();
			
		}
	}
	
	
	/*
		程式抓取 json 後會根據 movePlayers 這個變數來移動物件
		比如說抓到 p1x, p1y 後，會移動 movePlayers[0] 的物件座標
		而抓到 p2x, p2y 後，移動 movePlayers[1] 的物件座標

		但 json 並不知道 movePlayers 裡面哪一個是 這個使用者 的物件
		所以開始 scene 時，輸入這名使用者的 index ( 1 ~ 5 )
		然後這個 function 會把 使用者的物件（userPlayer）插入進 movePlayers 這個陣列裡
	*/
	void SetupPlayerIndex () {
		
		Transform[] newMovePlayers = new Transform[ movePlayers.Length +1 ];
		int oldPlayerIndex = 0;
		
		for( int i=0; i< newMovePlayers.Length; i++ )
		{
			if( playerIndex == i )
				newMovePlayers[i] = userPlayer;
			else
			{
				newMovePlayers[i] = movePlayers[oldPlayerIndex];
				oldPlayerIndex++;
			}
		}
		
		movePlayers = newMovePlayers;
		
	}
}
