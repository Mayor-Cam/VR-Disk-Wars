using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
/*
Player Controller handles all player controls, to include movement and disk manipulation
*/
public class PlayerController : NetworkBehaviour {
  //Camera Object for Looking
  public GameObject playerCamera;
    //Disk object and prefab
    public GameObject leftHand;
    public GameObject rightHand;
  public GameObject objDisk;
  public DiskController diskController;
  // Player Speed Scalar Variable
  public float playerSpeed = 1f;
  // Player Rotation Speed Scalar Variable
  public float playerTurnSpeed = 10f;
    public bool leftClicked = false;
    bool rightClicked = false;
    public float catchDistanceThreshold = 1;
  void Start () {
		
	}
	
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }
        transform.GetChild(0).GetComponent<Camera>().gameObject.SetActive(true);
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
		if(Input.GetAxis("Fire1") == 1) {
            if (!leftClicked)
            {
                leftClicked = true;
                Debug.Log(diskController.GetFired());
                if (!diskController.GetFired())
                {
                    Debug.Log("THROW!");
                    diskController.Fire(gameObject, playerCamera);
                }
                else if (Vector3.Distance(leftHand.transform.position, objDisk.transform.position) <= catchDistanceThreshold)
                {
                    Debug.Log("CATCH!");
                    diskController.Catch(leftHand);
                }
            }
		}
        else
        {
            leftClicked = false;
        }
	}
}
