using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskThrowScript : MonoBehaviour {
	
	float xSpeed;
	float ySpeed;
	float zSpeed;
	
	float speedMod = 0.1f;
	
	Vector3 resetPosition;
	
	// Use this for initialization
	void Start () {
		xSpeed = 0;
		ySpeed = 0;
		zSpeed = 0;
		
		resetPosition = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		// If left mouse button (or wand) is clicked or released...
			// Or for future reference, if being grabbed...
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) {
			
			// Position matches position of mouse (wand).
				// Will need to add angle change when implementing in VR
			xSpeed = speedMod * Input.GetAxis("Mouse X");
			// ySpeed = 
			zSpeed = speedMod * Input.GetAxis("Mouse Y");
		}
		
		// Move the dang disk.
		transform.Translate(xSpeed, 0, zSpeed);
		
		if (Input.GetMouseButtonDown(1)) {
			transform.position = resetPosition;
		}
	}
}
