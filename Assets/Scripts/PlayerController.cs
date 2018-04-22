using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
/*
Player Controller handles all player controls, to include movement and disk manipulation
*/

public class PlayerController : NetworkBehaviour {
  //Left / Right Hand Object
  public class Body {
    public int HEAD = 0;
    public int LEFTHAND = 1;
    public int RIGHTRIGHT = 2;
  }
  Body body;
  //Camera Object for Looking
  public GameObject playerCamera;
  //Disk object and prefab
  public GameObject leftHand;
  public GameObject rightHand;
  public GameObject objDisk;
  public GameObject prefDisk;
  public DiskController diskController;
  // Player Speed Scalar Variable
  public float playerSpeed = 1f;
  public float mouseSensitivity = 10f;
  // Player Rotation Speed Scalar Variable
  public float playerTurnSpeed = 10f;
  public bool leftClicked = false;
  bool rightClicked = false;
  public float catchDistanceThreshold = 1;
  public Vector3 deltaPosition; //Used to calculate movement between frames
  float networkPlayerOldTimestamp;
	//Network synchronization for Player
  [SyncVar]
  public Vector3 networkPlayerNextPosition;
  [SyncVar]
  public Quaternion networkPlayerRotation;
  [SyncVar]
  public Vector3 networkPlayerVelocity;
  [SyncVar]
  public float networkPlayerNewTimestamp;
   //Network synchronization for Disk
  [SyncVar]
  public float networkDiskSpeed = 10;
  [SyncVar]
  public bool networkDiskFired = false;
  [SyncVar]
  public Vector3 networkDiskDeparturePosition;
  [SyncVar]
  public Vector3 networkDiskDirection;
  [SyncVar]
  public float networkDiskMagnitude;

  void Start() {
    //instantiate body 'object'
    body = new Body();
    //instantiate disk and put it in the body's left hand
    objDisk = Instantiate(prefDisk);
    objDisk.transform.SetParent(gameObject.transform.GetChild(body.LEFTHAND).transform);      
    objDisk.transform.localPosition = Vector3.zero;
    //grab disk's controller
    diskController = objDisk.GetComponent<DiskController>();
    if (isServer){
      //Server is put on one side of the room and flipped around
      transform.position = new Vector3(0f, 1f, 2.5f);
      transform.Rotate(Vector3.up * 180);
    }
    else {
    //If client, put player on other side
      transform.position = new Vector3(0f, 1f, -2.5f);
    }
    //Set OTHER player a different color
    if (!isLocalPlayer){
      GetComponent<MeshRenderer>().material.SetColor("_ColorTint", new Color(1.0f, 0.75f, 0.25f, 1f));
      GetComponent<MeshRenderer>().material.SetColor("_RimColor", new Color(1.0f, 1.0f, 0.5f, 1f));
      objDisk.GetComponent<MeshRenderer>().material.SetColor("_ColorTint", new Color(1.0f, 0.75f, 0.25f, 1f));
      objDisk.GetComponent<MeshRenderer>().material.SetColor("_RimColor", new Color(1.0f, 1.0f, 0.5f, 1f));
    }
  }  

  private void Update(){
    if(isLocalPlayer) {
      transform.GetChild(0).gameObject.SetActive(true);
      float horizontalInput = Input.GetAxis("Horizontal");
      float verticalInput = Input.GetAxis("Vertical");
      float mouseX = Input.GetAxis("Mouse X");
      //Forward backward movement
      Vector3 longMovement = transform.forward*verticalInput*Time.deltaTime*playerSpeed;
      //Strafing movement
      Vector3 latMovement =  transform.right*horizontalInput*Time.deltaTime*playerSpeed;
      //Combine the two
      transform.position += longMovement + latMovement;
      //Rotation control
      transform.Rotate(Vector3.up * mouseX * Time.deltaTime * mouseSensitivity);
      if(!isServer)
        //Remote commands server to move
        CmdSyncMove(transform.position, transform.rotation);
      else {
        //else, server moves anyway
        networkPlayerNextPosition = transform.position;
        networkPlayerRotation = transform.rotation;
      }
      if(Input.GetAxis("Fire1") == 1 && !networkDiskFired) {
        //Using CmdFire and Fire cause idk which one to use, and it works as is :) 
        CmdFire(gameObject, playerCamera);
        Fire(gameObject, playerCamera);
        //Tell client to use RpcFire function if server
        if(isServer) RpcFire(gameObject, playerCamera);
      }
    } 
    else { //if not local player go ahead and perform calculations based on network-synced variables
      transform.position = Vector3.Lerp(transform.position, networkPlayerNextPosition + deltaPosition , Time.deltaTime * playerSpeed);
      transform.rotation = Quaternion.Lerp(transform.rotation, networkPlayerRotation, Time.deltaTime*60f);
      if(NetworkUpdated()) { //This boolean checks to see if new packets came in by seeing if the networkPlayerNewTimestamp variable (Time.time) changed
        deltaPosition = Vector3.zero; //if so, we have new positional data, so reset the delta position (for lerping inbetween network frames)
      }
      else {
        deltaPosition += networkPlayerVelocity * Time.deltaTime; //accumulated deltaposition over time (in between network frames)
      }
    }
  }

	[Command]
  void CmdSyncMove(Vector3 playerPos, Quaternion playerRot){ //Command functions are functions that allow the client to command the server to do things, in this case, update player's positional info
    networkPlayerVelocity = (playerPos - networkPlayerNextPosition) / Time.deltaTime;
    networkPlayerNextPosition = playerPos;
    networkPlayerRotation = playerRot;
    networkPlayerNewTimestamp = Time.time;
  }

	[Command]
  void CmdThrow(Vector3 diskPosition, Vector3 discRotation, float throwStrength){ //Throw disk, not used atm
    //TODO
  }

  bool NetworkUpdated() { //Checks if the network has updated via networkPlayerTinmestamp / networkPlayerOldTimestamp
  if(networkPlayerOldTimestamp != networkPlayerNewTimestamp) {
    networkPlayerOldTimestamp = networkPlayerNewTimestamp;
    return true;
    }
  return false;
  }

	void Fire (GameObject player, GameObject mainCam) { //Locally executed Fire function
		print("[SERVER] WE'RE FIRING!");
    objDisk.transform.rotation = playerCamera.transform.rotation;
    networkDiskDirection = transform.forward;
    networkDiskFired = true;
    objDisk.transform.SetParent(null);      
  }

  [Command] 
  void CmdFire (GameObject player, GameObject mainCam) { //Client to server Fire function
    print("[CMD] WE'RE FIRING!");
    objDisk.transform.rotation = playerCamera.transform.rotation;
    networkDiskDirection = transform.forward;
    networkDiskFired = true;
    objDisk.transform.SetParent(null);  
	}

  [ClientRpc]
  void RpcFire (GameObject player, GameObject mainCam) { //Server to client Fire function
    print("[Rpc] WE'RE FIRING!");
    objDisk.transform.rotation = playerCamera.transform.rotation;
    networkDiskDirection = transform.forward;
    networkDiskFired = true;
    objDisk.transform.SetParent(null);  
  }

	[Command]
	public void CmdUpdateVector(Vector3 pos, Vector3 dir, float mag){ //Updates the disk's vector
    print("UPDATING!");
    networkDiskDeparturePosition = pos;
    networkDiskDirection = dir;
    networkDiskMagnitude = mag;
	}
}
