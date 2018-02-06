using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour {
  float playerSpeed = 1f;
  float playerTurnSpeed = 10f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");
		Vector3 playerPosition = gameObject.transform.position;
    gameObject.transform.Translate(new Vector3 (horizontalInput * Time.deltaTime * playerSpeed, 0, verticalInput * Time.deltaTime * playerSpeed));
		//gameObject.transform.position = new Vector3(playerPosition.x + horizontalInput * Time.deltaTime,gameObject.transform.position.x, playerPosition.z + verticalInput * Time.deltaTime);
		gameObject.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime);
	}
}
