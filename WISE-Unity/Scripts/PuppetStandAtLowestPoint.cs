using UnityEngine;
using System.Collections;

public class PuppetStandAtLowestPoint : MonoBehaviour {

	public Transform transformRoot;
	public Transform hipPoint;
	public float groundY = 0.0f;


	Transform[] standPoints;

	// Use this for initialization
	void Start () {
	
		standPoints = transformRoot.GetComponentsInChildren<Transform> ();

	}
	
	// Update is called once per frame
	void Update () {
	
		UpdateHeight ();
	}

	void UpdateHeight () {

		// get lowest point
		float minY = 1000.0f;

		for (int i=0; i< standPoints.Length; i++) {
			if( standPoints[i].position.y < minY )
			{
				minY = standPoints[i].position.y;
			}
		}

		float dist = Mathf.Abs (minY - groundY);
		Vector3 hipPos = hipPoint.position;

		if (minY > groundY)
			hipPos.y -= dist;
		else
			hipPos.y += dist;

		hipPoint.position = hipPos;
	}
}
