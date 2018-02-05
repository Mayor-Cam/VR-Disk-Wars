using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");
		gameObject.transform.position = Vector3(horizontalInput,gameObject.transform.position.x, verticalInput);
		gameObject.transform.Rotate(Vector3.right * Input.GetAxis("Mouse X") * Time.deltaTime);
	}
}
