using UnityEngine;
using System.Collections;

public class MouseDragBall : MonoBehaviour {

	bool isHolding = false;
	Vector3 diff = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (isHolding) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, 10.0f ) );
			mousePos.y = transform.position.y;
			transform.position = mousePos - diff;
		}
	}

	void OnMouseDown () {
		isHolding = true;

		Vector3 mousePos = Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, 10.0f ) );
		diff = mousePos - transform.position;
		diff.y = 0.0f;
	}

	void OnMouseUp () {
		isHolding = false;
	}
}
