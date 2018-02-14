using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Player Controller handles all player controls, to include movement and disk manipulation
*/
public class PlayerController : MonoBehaviour {
  //Camera Object for Looking
  public GameObject playerCamera;
  //Disk object and prefab
  public GameObject prefDisk;
  GameObject objDisk;
  public DiskController diskController;
  // Player Speed Scalar Variable
  public float playerSpeed = 1f;
  // Player Rotation Speed Scalar Variable
  public float playerTurnSpeed = 10f;
  
 
  void Start () {
		
	}
	
	void Update () {
    //Shortened variable name for WASD for convenience 
		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");
		//Logic to translate input to ingame movement
		Vector3 playerPosition = gameObject.transform.position;
    gameObject.transform.Translate(new Vector3 (horizontalInput * Time.deltaTime * playerSpeed, 0, verticalInput * Time.deltaTime * playerSpeed));
		gameObject.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * playerTurnSpeed);
		//Player Camera Look
		playerCamera.transform.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * playerTurnSpeed);
		//Logic to translate input to shoot disks
		if(Input.GetAxis("Fire1") == 1 && !diskController.GetFired()) {
      diskController.Fire(gameObject, playerCamera);
		}
		else if(Input.GetAxis("Fire1") == 0 && diskController.GetFired()) {
		}
		
	}
}
