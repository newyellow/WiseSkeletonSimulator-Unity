using UnityEngine;
using System.Collections;


public class CameraTargetFocus : MonoBehaviour {

	public Transform lookAtTarget;
	public Camera cam;

	// translate part
	public float cameraTranslationSpeed = 2.0f;
	bool isTranslateOn = false;

	// look at angle
	public float angleRotateSpeed = 2.0f;
	public Vector3 cameraLookAtRot;
	bool isRotateOn = false;
	Vector3 lastMousePos = Vector3.zero;

	// zoom settings
	public float scrollZoomSpeed = 1.0f;
	public float cameraMinDistance = 5.0f;
	float cameraDistance = 10.0f;


	Vector3 camPos = Vector3.zero;
	

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		UpdatePosition ();

	}

	void UpdatePosition () {
		// follow target pos
		transform.position = lookAtTarget.position;
		
		// adjust cam position
		camPos.z = Mathf.Max (cameraMinDistance, cameraDistance) * -1;

		cam.transform.localPosition = camPos;
		
		// adjust rot
		transform.localRotation = Quaternion.Euler( cameraLookAtRot );
		
		transform.LookAt (lookAtTarget);
	}
	

	void OnGUI () {

		// scroll event
		cameraDistance += -1 * Input.mouseScrollDelta.y * Time.deltaTime * scrollZoomSpeed;


		// camera translate part
		if (Input.GetMouseButtonDown (2))
			isTranslateOn = true;

		if (Input.GetMouseButtonUp (2))
			isTranslateOn = false;

		if (isTranslateOn) {
			Vector3 nowMousePos = Input.mousePosition;
			float xMove = (nowMousePos.x - lastMousePos.x) / Screen.width;
			float yMove = (nowMousePos.y - lastMousePos.y) / Screen.height;
			
			Vector3 newPos = lookAtTarget.position;
			newPos += transform.right * -1 * xMove * Time.deltaTime * cameraTranslationSpeed;
			newPos += transform.up * -1 * yMove * Time.deltaTime * cameraTranslationSpeed;

			lookAtTarget.position = newPos;
		}

		// camera rotate part
		if (Input.GetMouseButtonDown (1))
			isRotateOn = true;

		if (Input.GetMouseButtonUp (1))
			isRotateOn = false;

		if (isRotateOn) {
			Vector3 nowMousePos = Input.mousePosition;
			float xMove = (nowMousePos.x - lastMousePos.x) / Screen.width;
			float yMove = (nowMousePos.y - lastMousePos.y) / Screen.height;

			cameraLookAtRot.x += -1 * yMove * Time.deltaTime * angleRotateSpeed;
			cameraLookAtRot.y += xMove * Time.deltaTime * angleRotateSpeed;

			if( cameraLookAtRot.x > 360.0f )
				cameraLookAtRot.x -= 360.0f;
			else if( cameraLookAtRot.x < 0.0f )
				cameraLookAtRot.x += 360.0f;

			if( cameraLookAtRot.y > 360.0f )
				cameraLookAtRot.y -= 360.0f;
			else if( cameraLookAtRot.x < 0.0f )
				cameraLookAtRot.y += 360.0f;
		}

		lastMousePos = Input.mousePosition;
	}

}
