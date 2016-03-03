using UnityEngine;
using System.Collections;

public class ManyHandsControlGUI : MonoBehaviour {


	public GameObject RotateStatusController;
	public GameObject MessageDebugger;
	public SkinnedMeshRenderer MainSkeleton;

	// for adjusting
	float xPos = 0.0f;
	float yPos = 1.0f;
	float cameraSize = 2.0f;


	// edit buffered num
	public RabbitMQDataCatcher catcher;

	public bool status = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if( Input.GetKeyDown( KeyCode.E ) )
		{
			status = !status;

			if( status )
				OpenGUIs();
			else
				CloseGUIs();
		}


	}

	void OnGUI ()
	{
		if( !status ) return;

		GUI.color = Color.black;

		GUILayout.BeginArea( new Rect( Screen.width * 0.1f, Screen.height * 0.5f, Screen.width*0.8f, Screen.height * 0.3f ) );
		GUILayout.BeginVertical();


		GUILayout.Label( "PosX : " + xPos );
		xPos = GUILayout.HorizontalScrollbar( xPos, 0.5f, -10.0f, 10.0f );

		GUILayout.Label( "PosY : " + yPos );
		yPos = GUILayout.HorizontalScrollbar( yPos, 0.5f, -10.0f, 10.0f );

		GUILayout.Label( "Size : " + cameraSize );
		cameraSize = GUILayout.HorizontalScrollbar( cameraSize, 0.5f, 0.0f, 3.0f );

		GUILayout.Label( "Max Buffered Num : " + catcher.maxBufferAmount );
		catcher.maxBufferAmount = (int)GUILayout.HorizontalScrollbar( catcher.maxBufferAmount, 0.5f, 0.0f, 100.0f );
		GUILayout.EndVertical();
		GUILayout.EndArea();

		// apply values on camera
		Camera.main.orthographicSize = cameraSize;
		Camera.main.gameObject.transform.position = new Vector3( xPos, yPos, -10.0f );
	}

	void CloseGUIs()
	{
		// hide skeleton
		//MainSkeleton.enabled = false;

		// hide all textmeshes
		MeshRenderer[] textRenderers = MessageDebugger.GetComponentsInChildren<MeshRenderer>();
		for( int i=0; i< textRenderers.Length; i++ )
			textRenderers[i].enabled = false;

		// hide button and text
		MeshRenderer[] controllerRenderers = RotateStatusController.GetComponentsInChildren<MeshRenderer>();
		for( int i=0; i< controllerRenderers.Length; i++ )
			controllerRenderers[i].enabled = false;

	}

	void OpenGUIs()
	{

		// show skeleton
		MainSkeleton.enabled = true;


		// show all textmeshes
		MeshRenderer[] textRenderers = MessageDebugger.GetComponentsInChildren<MeshRenderer>();
		for( int i=0; i< textRenderers.Length; i++ )
			textRenderers[i].enabled = true;

		// show button and text
		MeshRenderer[] controllerRenderers = RotateStatusController.GetComponentsInChildren<MeshRenderer>();
		for( int i=0; i< controllerRenderers.Length; i++ )
			controllerRenderers[i].enabled = true;
	}
}
