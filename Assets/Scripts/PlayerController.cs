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
    //Network synchronization
    [SyncVar]
    public Vector3 networkPlayerNextPosition;
    [SyncVar]
    public Quaternion networkPlayerRotation;
    [SyncVar]
    public Vector3 networkPlayerVelocity;
    [SyncVar]
    public float networkPlayerNewTimestamp;
    void OnStartServer() {
      print("YO");
    }
    void Start() {
      body = new Body();
      if (isServer){
        print("SERVER");
        transform.position = new Vector3(0f, 1f, 2.5f);
        objDisk = Instantiate(prefDisk);
        NetworkServer.Spawn(objDisk);
        objDisk.transform.parent = gameObject.transform.GetChild(body.LEFTHAND).transform;
        objDisk.transform.localPosition = Vector3.zero;
        diskController = objDisk.GetComponent<DiskController>();
      }
      else{
        print("REMOTE");
        transform.position = new Vector3(0f, 1f, -2.5f);
        objDisk = Instantiate(prefDisk);
        CmdInstantiateDisk();
        objDisk.transform.parent = gameObject.transform.GetChild(body.LEFTHAND).transform;
        objDisk.transform.localPosition = Vector3.zero;
        diskController = objDisk.GetComponent<DiskController>();
      }
      if (!isLocalPlayer){
        GetComponent<MeshRenderer>().material.SetColor("_ColorTint", new Color(1.0f, 0.75f, 0.25f, 1f));
        GetComponent<MeshRenderer>().material.SetColor("_RimColor", new Color(1.0f, 1.0f, 0.5f, 1f));
      }
    
    }
    [Command]
    void CmdInstantiateDisk() {

      NetworkServer.Spawn(objDisk);
    }
    
    Vector3 playerPosition;
    public Vector3 deltaPosition;
    private void Update(){
      if(isLocalPlayer) {
        transform.GetChild(0).gameObject.SetActive(true);
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        Vector3 longMovement = transform.forward*verticalInput*Time.deltaTime*playerSpeed;
        Vector3 latMovement =  transform.right*horizontalInput*Time.deltaTime*playerSpeed;
        transform.position += longMovement + latMovement;
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime * mouseSensitivity);
        new Vector3(horizontalInput*playerSpeed*Time.deltaTime,0f,verticalInput*playerSpeed*Time.deltaTime);
        
        if(!isServer)
          CmdSyncMove(transform.position, transform.rotation);
        else {
        networkPlayerNextPosition = transform.position;
        networkPlayerRotation = transform.rotation;
        }
        if(Input.GetAxis("Fire1") == 1) {
          diskController.Fire(gameObject, playerCamera);
        }
      }
      else {
        transform.position = Vector3.Lerp(transform.position, networkPlayerNextPosition + deltaPosition , Time.deltaTime * playerSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, networkPlayerRotation, Time.deltaTime*60f);
        if(NetworkUpdated()) {
          deltaPosition = Vector3.zero;
        }
        else {
          deltaPosition += networkPlayerVelocity * Time.deltaTime;
        }
      }
    }
    [Command]
    void CmdSyncMove(Vector3 playerPos, Quaternion playerRot){
      networkPlayerVelocity = (playerPos - networkPlayerNextPosition) / Time.deltaTime;
      networkPlayerNextPosition = playerPos;
      networkPlayerRotation = playerRot;
      networkPlayerNewTimestamp = Time.time;
    }
    [Command]
    void CmdThrow(Vector3 diskPosition, Vector3 discRotation, float throwStrength){
    }
    float networkPlayerOldTimestamp;
    bool NetworkUpdated() {
    if(networkPlayerOldTimestamp != networkPlayerNewTimestamp) {
      networkPlayerOldTimestamp = networkPlayerNewTimestamp;
      return true;
      }
    return false;
    }
}
