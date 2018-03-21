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
    public Vector3 networkPlayerPosition;
    [SyncVar]
    public Quaternion networkPlayerRotation;
    [SyncVar]
    public Vector3 networkPlayerVelocity;

    void Start()
    {
        if (!isLocalPlayer)
        {
            if (isServer)
            {
                print("SERVER");
                transform.position = new Vector3(0f, 1f, 2.5f);
            }
            else
            {
                print("REMOTE");
                transform.position = new Vector3(0f, 1f, -2.5f);
            }
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_ColorTint", new Color(1.0f, 0.75f, 0.25f, 1f));
            GetComponent<MeshRenderer>().material.SetColor("_RimColor", new Color(1.0f, 1.0f, 0.5f, 1f));
        }
    }

    private void Update()
    {
        
        if (!isLocalPlayer)
        {
            //transform.position = transform.position + networkPlayerVelocity * Time.deltaTime;
        }
    }
    Vector3 playerPosition;
    void FixedUpdate () {
        if(!isLocalPlayer)
        {
            transform.position = networkPlayerPosition;
            transform.rotation = networkPlayerRotation;
            return;
        }
        
        transform.GetChild(0).GetComponent<Camera>().gameObject.SetActive(true);
        //Shortened variable name for WASD for convenience 
        float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");
        //Logic to translate input to ingame movement
        Vector3 oldPlayerPosition = playerPosition;
		playerPosition = gameObject.transform.position;
        CmdMove(horizontalInput, verticalInput, oldPlayerPosition);
        transform.position = transform.position + new Vector3(horizontalInput * Time.deltaTime * playerSpeed, 0, verticalInput * Time.deltaTime * playerSpeed);

    }
    [Command]
    void CmdMove(float horizontalInput, float verticalInput, Vector3 oldPlayerPosition)
    {
        //gameObject.transform.Translate(new Vector3(horizontalInput * Time.deltaTime * playerSpeed, 0, verticalInput * Time.deltaTime * playerSpeed));
        //gameObject.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * playerTurnSpeed);
        //Player Camera Look
        //playerCamera.transform.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * playerTurnSpeed);
        networkPlayerPosition = transform.position + new Vector3(horizontalInput * Time.deltaTime * playerSpeed, 0, verticalInput * Time.deltaTime * playerSpeed);
        Quaternion rotateQuat = Quaternion.Euler(Vector3.up * Input.GetAxis("Mouse X") * playerTurnSpeed);
        networkPlayerRotation = transform.rotation * rotateQuat;
        networkPlayerVelocity = (playerPosition - oldPlayerPosition) / (float)Time.fixedDeltaTime;
    }
}
