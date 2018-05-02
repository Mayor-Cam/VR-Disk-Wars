using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.XR;
/*
Player Controller handles all player controls, to include movement and disk manipulation
*/

public class PlayerController : NetworkBehaviour
{
    //Left / Right Hand Object
    public class Body
    {
        public int HEAD = 0;
        public int LEFTHAND = 1;
        public int RIGHTRIGHT = 2;
    }
    Body body;

    public PlayerController otherPlayerController;
    //Body Objects
    public GameObject prefHead;
    public GameObject prefLHand;
    public GameObject prefRHand;

    public XRNode xrHead = XRNode.Head;
    public XRNode xrLeftHand = XRNode.LeftHand;
    public XRNode xrRightHand = XRNode.RightHand; 
    
    public GameObject playerHead;
    public GameObject playerTorso; // Added by Cam for diskHit function - 4/30/2018
    // public Renderer torsoRenderer; // Added by Cam for diskHit function - 4/30/2018
    //Camera Object for Looking
    public GameObject playerCamera;
    //Disk object and prefab
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject objDisk;
    public GameObject prefDisk;
    public DiskController diskController;

    public GameObject youWin;
    public GameObject youLose;
    
    //Materials for other player
    public Material otherPlayerBaseColor;
    public Material otherPlayerHeadColor;
    public Material otherplayerTorsoColor;
    public Material otherPlayerColor;
    public Material otherDiskColor;
    public Color32 otherLightColor;
    // Player Speed Scalar Variable
    public float playerSpeed = 1f;
    public float mouseSensitivity = 10f;
    // Player Rotation Speed Scalar Variable
    public float playerTurnSpeed = 10f;
    public bool leftClicked = false;
    bool rightClicked = false;
    public float catchDistanceThreshold = 1;
    public Vector3 headDeltaPosition, lHandDeltaPosition, rHandDeltaPosition; //Used to calculate movement between frames
    float networkPlayerOldTimestamp;
    public GameObject gameController;
    GameControllerScript gameControllerScript;
    Vector3 spawnPoint = new Vector3();

    Color playerColor;
    Color inactiveColor = new Color(0.75f, 0.75f, 0.75f, 0.5f);

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


    [SyncVar]
    public Vector3 networkLeftHandNextPosition;
    [SyncVar]
    public Quaternion networkLeftHandRotation;
    [SyncVar]
    public Vector3 networkRightHandNextPosition;
    [SyncVar]
    public Quaternion networkRightHandRotation;
    [SyncVar]
    public Vector3 networkHeadNextPosition;
    [SyncVar]
    public Quaternion networkHeadRotation;

    [SyncVar]
    public Vector3 networkHeadVelocity;
    [SyncVar]
    public Vector3 networkLeftHandVelocity;
    [SyncVar]
    public Vector3 networkRightHandVelocity;
    
    [SyncVar]
    public Vector3 networkDiskNextPosition;
    [SyncVar]
    public Vector3 networkDiskVelocity;
    [SyncVar]
    public Quaternion networkDiskRotation;
    void Start()
    {
        // Instantiate body parts
        leftHand = Instantiate(prefLHand);
        rightHand = Instantiate(prefRHand);
        playerHead = Instantiate(prefHead);
        playerTorso = playerHead.transform.GetChild(0).gameObject;
        playerColor = playerHead.GetComponent<Renderer>().material.GetColor("_ColorTint");

        spawnPoint = transform.position;  // Set initial position as spawnPoint.
        gameController = GameObject.Find("GameController");
        gameControllerScript = gameController.GetComponent<GameControllerScript>();
        playerTorso = playerHead.transform.GetChild(0).gameObject;
        playerTorso.transform.SetParent(gameObject.transform);
        //instantiate body 'object'
        body = new Body();
        //instantiate disk and put it in the body's left hand
        objDisk = Instantiate(prefDisk);
        //objDisk.transform.SetParent(gameObject.transform.GetChild(body.LEFTHAND).transform);      
        objDisk.transform.localPosition = Vector3.zero;
        //grab disk's controller
        diskController = objDisk.GetComponent<DiskController>();
        diskController.owner = this.gameObject;
        diskController.ownerController = this.gameObject.GetComponent<PlayerController>();
        leftHand.GetComponent<HandGrabbing>().isLocal = isLocalPlayer;
        rightHand.GetComponent<HandGrabbing>().isLocal = isLocalPlayer;
        CmdInstantiateBodyParts();
        leftHand.transform.parent = gameObject.transform;
        rightHand.transform.parent = gameObject.transform;
        playerHead.transform.parent = gameObject.transform;
        leftHand.transform.localPosition = InputTracking.GetLocalPosition(xrHead);
        rightHand.transform.localPosition = InputTracking.GetLocalPosition(xrHead);
        playerHead.transform.localPosition = InputTracking.GetLocalPosition(xrHead);
        print("XRDevice: " + (XRDevice.isPresent ? XRDevice.model : "Not Present"));
        if (isServer)
        {
            //Server is put on one side of the room and flipped around
            if(isLocalPlayer){
                transform.position = new Vector3(0f, 0f, -5f);
                objDisk.transform.position = new Vector3(0f, 1f, -4f);
                networkDiskNextPosition = objDisk.transform.position;

                gameControllerScript.hostPlayer = this.gameObject;
                gameControllerScript.hostController = this;
                gameControllerScript.hostDisk = objDisk;
                gameControllerScript.hostDiskController = diskController;
            }
            else {
                transform.position = new Vector3(0f, 0f, 5f);
                transform.Rotate(Vector3.up*180);
                objDisk.transform.position = new Vector3(0f, 1f, 4f);
                networkDiskNextPosition = objDisk.transform.position;

                gameControllerScript.clientPlayer = this.gameObject;
                gameControllerScript.clientController = this;
                gameControllerScript.clientDisk = objDisk;
                gameControllerScript.clientDiskController = diskController;
              }
            
        }
        else
        {
            //If client, put player on other side
            if(isLocalPlayer){
                transform.position = new Vector3(0f, 0f, 5f);
                transform.Rotate(Vector3.up*180);
                objDisk.transform.position = new Vector3(0f, 1f, 4f);

                gameControllerScript.clientPlayer = this.gameObject;
                gameControllerScript.clientController = this;
                gameControllerScript.clientDisk = objDisk;
                gameControllerScript.clientDiskController = diskController;
              }
            else {
                transform.position = new Vector3(0f, 0f, -5f);
                objDisk.transform.position = new Vector3(0f, 1f, -4f);

                gameControllerScript.hostPlayer = this.gameObject;
                gameControllerScript.hostController = this;
                gameControllerScript.hostDisk = objDisk;
                gameControllerScript.hostDiskController = diskController;
            }
            
        }
        if(isLocalPlayer && !isServer) {
            otherPlayerController = gameControllerScript.hostController;
            gameControllerScript.hostController.otherPlayerController = gameControllerScript.clientController;
        }
        if(!isLocalPlayer && isServer) {
            otherPlayerController = gameControllerScript.hostController;
            gameControllerScript.hostController.otherPlayerController = gameControllerScript.clientController;
        }
        //Set OTHER player a different color
        if (isLocalPlayer)
        {
            if(!XRDevice.isPresent) transform.Translate(Vector3.up * 1.5f);
            /*
            Material[] tempMats = playerHead.GetComponent<MeshRenderer>().materials;
            tempMats[0] = otherPlayerBaseColor;
            tempMats[1] = otherPlayerHeadColor;
            playerHead.GetComponent<MeshRenderer>().materials = tempMats;
            tempMats = playerHead.transform.GetChild(0).GetComponent<MeshRenderer>().materials;
            tempMats[0] = otherPlayerBaseColor;
            tempMats[1] = otherplayerTorsoColor;
            playerHead.transform.GetChild(0).GetComponent<MeshRenderer>().materials = tempMats;            
            */
            playerHead.GetComponent<MeshRenderer>().enabled = false;
            playerTorso.GetComponent<MeshRenderer>().enabled = false;
            leftHand.GetComponent<MeshRenderer>().material = otherPlayerColor;
            rightHand.GetComponent<MeshRenderer>().material = otherPlayerColor;
            objDisk.GetComponent<MeshRenderer>().material = otherDiskColor;
            objDisk.GetComponent<Light>().color = otherLightColor;
        }
    }

    private void Update()
    {
        // Slow disk to cruise speed
        if (isServer)
        {
            if (networkDiskSpeed > diskController.cruiseSpeed) networkDiskSpeed = Mathf.Lerp(networkDiskSpeed, diskController.cruiseSpeed, Time.deltaTime * diskController.decelSpeed);
        }

        playerTorso.transform.position = new Vector3(playerHead.transform.position.x,playerHead.transform.position.y-0.65f,playerHead.transform.position.z);
        playerTorso.transform.eulerAngles = new Vector3(playerTorso.transform.eulerAngles.x,playerHead.transform.eulerAngles.y,playerTorso.transform.eulerAngles.z);
        if (isLocalPlayer)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            playerHead.transform.localPosition = InputTracking.GetLocalPosition(xrHead);
            playerHead.transform.localRotation = InputTracking.GetLocalRotation(xrHead);
            //leftHand.transform.position = InputTracking.GetLocalPosition(xrLeftHand);
            //rightHand.transform.position = InputTracking.GetLocalPosition(xrRightHand);
            if(!XRDevice.isPresent) {
              float horizontalInput = Input.GetAxis("Horizontal");
              float verticalInput = Input.GetAxis("Vertical");
              float mouseX = Input.GetAxis("Mouse X");
              //Forward backward movement
              Vector3 longMovement = transform.forward * verticalInput * Time.deltaTime * playerSpeed;
              //Strafing movement
              Vector3 latMovement = transform.right * horizontalInput * Time.deltaTime * playerSpeed;
              //Combine the two
              transform.position += longMovement + latMovement;
              //Rotation control
              transform.Rotate(Vector3.up * mouseX * Time.deltaTime * mouseSensitivity);
            }
            if (!isServer)

            {
                //Remote commands server to move
                CmdSyncMove(
                    transform.position,
                    transform.rotation,
                    InputTracking.GetLocalPosition(xrHead),
                    InputTracking.GetLocalRotation(xrHead),
                    InputTracking.GetLocalPosition(xrLeftHand),
                    InputTracking.GetLocalRotation(xrLeftHand),
                    InputTracking.GetLocalPosition(xrRightHand),
                    InputTracking.GetLocalRotation(xrRightHand)
                    );

            }

            else
            {
                //else, server moves anyway
                networkPlayerVelocity = (transform.position - networkPlayerNextPosition) / Time.deltaTime;
                networkPlayerNextPosition = transform.position;
                networkPlayerRotation = transform.rotation;
                
                networkHeadVelocity = (InputTracking.GetLocalPosition(xrHead) - networkHeadNextPosition ) / Time.deltaTime;
                networkLeftHandVelocity = (InputTracking.GetLocalPosition(xrLeftHand) - networkLeftHandNextPosition) / Time.deltaTime;
                networkRightHandVelocity = (InputTracking.GetLocalPosition(xrRightHand) - networkRightHandNextPosition) / Time.deltaTime;
                
                networkHeadNextPosition = InputTracking.GetLocalPosition(xrHead); //playerHead.transform.localPosition;
                networkLeftHandNextPosition = InputTracking.GetLocalPosition(xrLeftHand); //leftHand.transform.localPosition;
                networkRightHandNextPosition = InputTracking.GetLocalPosition(xrRightHand); //rightHand.transform.localPosition;

                networkHeadRotation = InputTracking.GetLocalRotation(xrHead);//playerHead.transform.rotation;
                networkLeftHandRotation = InputTracking.GetLocalRotation(xrLeftHand);//leftHand.transform.rotation;
                networkRightHandRotation = InputTracking.GetLocalRotation(xrRightHand);//rightHand.transform.rotation;
                networkPlayerNewTimestamp = Time.time;
            }
            if (Input.GetAxis("Fire1") == 1 && !networkDiskFired)
            {
                //Using CmdFire and Fire cause idk which one to use, and it works as is :) 
                CmdFire(gameObject, playerCamera);
                Fire(gameObject, playerCamera);
                //Tell client to use RpcFire function if server
                if (isServer) RpcFire(gameObject, playerCamera);
            }
        }
        else
        { //if not local player go ahead and perform calculations based on network-synced variables
            
            playerHead.transform.localPosition = Vector3.Lerp(playerHead.transform.localPosition, networkHeadNextPosition + headDeltaPosition, Time.deltaTime * 60f);
            playerHead.transform.localRotation = networkHeadRotation;
            leftHand.transform.localPosition = Vector3.Lerp(leftHand.transform.localPosition, networkLeftHandNextPosition + lHandDeltaPosition, Time.deltaTime * 60f);
            leftHand.transform.localRotation = networkLeftHandRotation;
            rightHand.transform.localPosition = Vector3.Lerp(rightHand.transform.localPosition, networkRightHandNextPosition + rHandDeltaPosition, Time.deltaTime * 60f);
            rightHand.transform.localRotation = networkRightHandRotation;
            if (NetworkUpdated())
            { //This boolean checks to see if new packets came in by seeing if the networkPlayerNewTimestamp variable (Time.time) changed
                headDeltaPosition = Vector3.zero; //if so, we have new positional data, so reset the delta position (for lerping inbetween network frames)
                lHandDeltaPosition = Vector3.zero;
                rHandDeltaPosition = Vector3.zero;
                leftHand.transform.localPosition = networkLeftHandNextPosition;
                rightHand.transform.localPosition = networkRightHandNextPosition;
                playerHead.transform.localPosition = networkHeadNextPosition;
            }
            else
            {
                headDeltaPosition += networkHeadVelocity * Time.deltaTime; //accumulated deltaposition over time (in between network frames)
                lHandDeltaPosition += networkLeftHandVelocity * Time.deltaTime;
                rHandDeltaPosition += networkRightHandVelocity * Time.deltaTime;
            }
        }
    }
    [Command]
    public void CmdDiskPosition(Vector3 vel, Vector3 dir,Vector3 pos) {
        networkDiskDirection = dir;
        networkDiskVelocity = vel;
        networkDiskNextPosition = pos;        
    }
    [Command]
    public void CmdSetFired(bool fired) {
        networkDiskFired = fired;
    }
    [Command]
    void CmdInstantiateBodyParts()
    {


        //NetworkServer.Spawn(leftHand);
        //NetworkServer.Spawn(rightHand);
        //NetworkServer.Spawn(playerHead);

        leftHand.GetComponent<HandGrabbing>().diskObj = objDisk;
        rightHand.GetComponent<HandGrabbing>().diskObj = objDisk;
        leftHand.GetComponent<HandGrabbing>().otherHand = rightHand;
        rightHand.GetComponent<HandGrabbing>().otherHand = leftHand;

        playerHead.transform.parent = gameObject.transform;
        leftHand.transform.parent = gameObject.transform;
        rightHand.transform.parent = gameObject.transform;
        
        leftHand.GetComponent<HandGrabbing>().diskObj = objDisk;
        rightHand.GetComponent<HandGrabbing>().diskObj = objDisk;
    }

    [Command]
    void CmdSyncMove(Vector3 playerPos, Quaternion playerRot, Vector3 headPos, Quaternion headRot, Vector3 lHandPos, Quaternion lHandRot, Vector3 rHandPos, Quaternion rHandRot)
    { //Command functions are functions that allow the client to command the server to do things, in this case, update player's positional info
        networkPlayerVelocity = (playerPos - networkPlayerNextPosition) / Time.deltaTime;
        networkPlayerNextPosition = playerPos;
        networkPlayerRotation = playerRot;
        networkPlayerNewTimestamp = Time.time;

        networkHeadVelocity = (headPos - networkHeadNextPosition) / Time.deltaTime;
        networkHeadNextPosition = headPos;
        networkLeftHandVelocity = (lHandPos - networkLeftHandNextPosition) / Time.deltaTime;
        networkLeftHandNextPosition = lHandPos;
        networkRightHandVelocity = (rHandPos - networkRightHandNextPosition) / Time.deltaTime;
        networkRightHandNextPosition = rHandPos;

        networkHeadRotation = headRot;
        networkLeftHandRotation = lHandRot;
        networkRightHandRotation = rHandRot;


    }


    public bool NetworkUpdated()
    { //Checks if the network has updated via networkPlayerTinmestamp / networkPlayerOldTimestamp
        if (networkPlayerOldTimestamp != networkPlayerNewTimestamp)
        {
            networkPlayerOldTimestamp = networkPlayerNewTimestamp;
            return true;
        }
        return false;
    }

    void Fire(GameObject player, GameObject mainCam)
    { //Locally executed Fire function
        print("[SERVER] WE'RE FIRING!");
        objDisk.transform.rotation = playerCamera.transform.rotation;
        networkDiskDirection = transform.forward;
        networkDiskFired = true;
        objDisk.transform.SetParent(null);
    }

    [Command]
    void CmdFire(GameObject player, GameObject mainCam)
    { //Client to server Fire function
        print("[CMD] WE'RE FIRING!");
        objDisk.transform.rotation = playerCamera.transform.rotation;
        networkDiskDirection = transform.forward;
        networkDiskFired = true;
        objDisk.transform.SetParent(null);
    }

    [ClientRpc]
    void RpcFire(GameObject player, GameObject mainCam)
    { //Server to client Fire function
        print("[Rpc] WE'RE FIRING!");
        objDisk.transform.rotation = playerCamera.transform.rotation;
        networkDiskDirection = transform.forward;
        networkDiskFired = true;
        objDisk.transform.SetParent(null);
    }

    [Command]
    public void CmdUpdateVector(Vector3 pos, Vector3 dir, float mag)
    { //Updates the disk's vector
        print("UPDATING!");
        networkDiskDeparturePosition = pos;
        networkDiskDirection = dir;
        networkDiskMagnitude = mag;
    }

    [ClientRpc]
    void RpcRelease(Vector3 dir)
    {
        networkDiskDirection = dir;
        objDisk.transform.SetParent(null);
    }
    [Command]
    public void CmdRelease(Vector3 dir)
    {
        networkDiskDirection = dir;
        objDisk.transform.SetParent(null);
    }

    // Called when this player is hit by the disk.  Called by the disk upon collision.
    public void DiskHit()
    {
        // Hit animation.  Probably spawn some kind of explosion?

        // int index = Random.Range(0, soundFX.Length);
        // soundFX[index].Play();

        // Send point info to gamecontroller
        gameControllerScript.Score(this.gameObject);

        // Set inactive
        playerHead.GetComponent<Renderer>().material.SetColor("_ColorTint", inactiveColor);
        playerTorso.GetComponent<Renderer>().material.SetColor("_ColorTint", inactiveColor);
        leftHand.GetComponent<Renderer>().material.SetColor("_ColorTint", inactiveColor);
        rightHand.GetComponent<Renderer>().material.SetColor("_ColorTint", inactiveColor);
    }

    // Resores player if they are destroyed. Called by gameController object.
    public void Restore()
    {
        // Spawn animation.  Reverse explosion?

        playerHead.GetComponent<Renderer>().material.SetColor("_ColorTint", playerColor);
        playerTorso.GetComponent<Renderer>().material.SetColor("_ColorTint", playerColor);
        leftHand.GetComponent<Renderer>().material.SetColor("_ColorTint", playerColor);
        rightHand.GetComponent<Renderer>().material.SetColor("_ColorTint", playerColor);
    }

    public void Win()
    {
        Instantiate(youWin);
    }

    public void Lose()
    {
        Instantiate(youLose);
    }

    public void SyncHit() {
        otherPlayerController.DiskHit();
        if(isServer) RpcSyncHit();
        else CmdSyncHit();

    }
    [ClientRpc]
    public void RpcSyncHit() {
        otherPlayerController.DiskHit();
    }

    [Command]
    public void CmdSyncHit() {
        otherPlayerController.DiskHit();
    }
}