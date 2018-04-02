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
    public GameObject prefDisk;
    public DiskController diskController;
  // Player Speed Scalar Variable
    public float playerSpeed = 1f;
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
    
    void Start()
    {
        if (isServer){
            print("SERVER");
            transform.position = new Vector3(0f, 1f, 2.5f);
        }
        else{
            print("REMOTE");
            transform.position = new Vector3(0f, 1f, -2.5f);
        }
        if (!isLocalPlayer){
          GetComponent<MeshRenderer>().material.SetColor("_ColorTint", new Color(1.0f, 0.75f, 0.25f, 1f));
          GetComponent<MeshRenderer>().material.SetColor("_RimColor", new Color(1.0f, 1.0f, 0.5f, 1f));
        }
    }
    
    Vector3 playerPosition;
    void FixedUpdate () {
    
    }
    public Vector3 cumulativeVelocity;
    private void Update(){
      if(isLocalPlayer) {
        transform.GetChild(0).gameObject.SetActive(true);
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.position += new Vector3(horizontalInput*playerSpeed*Time.deltaTime,0f,verticalInput*playerSpeed*Time.deltaTime);
        networkPlayerNextPosition = transform.position;
        if(!isServer)
          CmdSyncMove(transform.position);
        }
      else {
        transform.position = Vector3.Lerp(transform.position, networkPlayerNextPosition + cumulativeVelocity , Time.deltaTime * playerSpeed);
        if(NetworkUpdated()) {
          cumulativeVelocity = Vector3.zero;
        }
        else {
          cumulativeVelocity += networkPlayerVelocity * Time.deltaTime;
        }
      }
    }
    [Command]
    void CmdSyncMove(Vector3 playerPos){
      networkPlayerVelocity = (playerPos - networkPlayerNextPosition) / Time.deltaTime;
      networkPlayerNextPosition = playerPos;
      networkPlayerNewTimestamp = Time.time;
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
