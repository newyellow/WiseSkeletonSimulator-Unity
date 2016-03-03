using UnityEngine;
using System.Collections;

public class SkeletonAnimationSwitcher : MonoBehaviour {

	public Animator targetAnimator;
	public RuntimeAnimatorController[] animFiles;

	public GUIStyle style;


	float speed = 1.0f;
	int nowIndex = 0;

	int indexAdder = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		nowIndex = (nowIndex + indexAdder) % animFiles.Length;

		// prevent bug
		while (nowIndex < 0)
			nowIndex += animFiles.Length;

		// apply settings
		if( indexAdder != 0 )
			targetAnimator.runtimeAnimatorController = animFiles [nowIndex];

		targetAnimator.speed = speed;
	}

	void OnGUI () {
		indexAdder = 0;

		GUILayout.BeginArea ( new Rect(Screen.width * 0.6f, Screen.height * 0.6f, Screen.width * 0.4f, Screen.height * 0.4f), style );

		GUILayout.Label ("Animation Settings");

		GUILayout.Space (10.0f);

		GUILayout.Label ("Animation Type");
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<"))
			indexAdder--;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUILayout.Label (animFiles [nowIndex].name);
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		if (GUILayout.Button (">"))
			indexAdder++;
		GUILayout.EndHorizontal ();

		GUILayout.Space (10.0f);

		// speed
		GUILayout.Label ("Dancing Speed");
		speed = GUILayout.HorizontalSlider (speed, 0.0f, 4.0f);
		GUILayout.Label (speed.ToString() + "x");


		GUILayout.EndArea ();
	}
}
