using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
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
    //Body Objects
    public GameObject prefHead;
    public GameObject prefLHand;
    public GameObject prefRHand;

    public GameObject playerHead;
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
    public GameObject gameController;
    GameControllerScript gameControllerScript;
    Vector3 spawnPoint = new Vector3();

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

    void Start()
    {
        leftHand = Instantiate(prefLHand);
        rightHand = Instantiate(prefRHand);
        playerHead = Instantiate(prefHead);
        spawnPoint = transform.position;  // Set initial position as spawnPoint.
        gameController = GameObject.Find("GameController");
        gameControllerScript = gameController.GetComponent<GameControllerScript>();
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
        if (isServer)
        {
            gameControllerScript.hostPlayer = this.gameObject;
            gameControllerScript.hostController = this;
            gameControllerScript.hostDiskController = diskController;
            //Server is put on one side of the room and flipped around
            transform.position = new Vector3(0f, 0f, -3f);
            transform.Rotate(Vector3.up * 180);
        }
        else
        {
            //gameControllerScript.clientPlayer = this.gameObject;
            gameControllerScript.clientController = this;

            //If client, put player on other side
            transform.position = new Vector3(0f, 0f, -2.5f);
        }
        //Set OTHER player a different color
        if (!isLocalPlayer)
        {
            GetComponent<MeshRenderer>().material.SetColor("_ColorTint", new Color(1.0f, 0.75f, 0.25f, 1f));
            GetComponent<MeshRenderer>().material.SetColor("_RimColor", new Color(1.0f, 1.0f, 0.5f, 1f));
            objDisk.GetComponent<MeshRenderer>().material.SetColor("_ColorTint", new Color(1.0f, 0.75f, 0.25f, 1f));
            objDisk.GetComponent<MeshRenderer>().material.SetColor("_RimColor", new Color(1.0f, 1.0f, 0.5f, 1f));
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            playerHead.transform.position = playerCamera.transform.position;
            transform.GetChild(0).gameObject.SetActive(true);
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
            if (!isServer)
            {
                //Remote commands server to move
                CmdSyncMove(
                    transform.position,
                    transform.rotation,
                    playerHead.transform.localPosition,
                    playerHead.transform.rotation,
                    leftHand.transform.localPosition,
                    leftHand.transform.rotation,
                    rightHand.transform.localPosition,
                    rightHand.transform.rotation
                    );
            }

            else
            {
                //else, server moves anyway
                networkPlayerNextPosition = transform.position;
                networkPlayerRotation = transform.rotation;

                networkHeadNextPosition = playerHead.transform.localPosition;
                networkLeftHandNextPosition = leftHand.transform.localPosition;
                networkRightHandNextPosition = rightHand.transform.localPosition;

                networkHeadRotation = playerHead.transform.rotation;
                networkLeftHandRotation = leftHand.transform.rotation;
                networkRightHandRotation = rightHand.transform.rotation;

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
            transform.position = Vector3.Lerp(transform.position, networkPlayerNextPosition + deltaPosition, Time.deltaTime * playerSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkPlayerRotation, Time.deltaTime * 60f);
            if (NetworkUpdated())
            { //This boolean checks to see if new packets came in by seeing if the networkPlayerNewTimestamp variable (Time.time) changed
                deltaPosition = Vector3.zero; //if so, we have new positional data, so reset the delta position (for lerping inbetween network frames)
            }
            else
            {
                deltaPosition += networkPlayerVelocity * Time.deltaTime; //accumulated deltaposition over time (in between network frames)
            }
        }
    }

    [Command]
    void CmdInstantiateBodyParts()
    {


        NetworkServer.Spawn(leftHand);
        NetworkServer.Spawn(rightHand);
        NetworkServer.Spawn(playerHead);

        leftHand.GetComponent<HandGrabbing>().diskObj = objDisk;
        rightHand.GetComponent<HandGrabbing>().diskObj = objDisk;
        leftHand.GetComponent<HandGrabbing>().otherHand = rightHand;
        rightHand.GetComponent<HandGrabbing>().otherHand = leftHand;

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

        //networkHeadNextPosition = headPos;
        //networkLeftHandNextPosition = lHandPos;
        //networkRightHandNextPosition = rHandPos;

        //networkHeadRotation = headRot;
        //networkLeftHandRotation = lHandRot;
        //networkRightHandRotation = rHandRot;


    }

    [Command]
    void CmdThrow(Vector3 diskPosition, Vector3 discRotation, float throwStrength)
    { //Throw disk, not used atm
      //TODO
    }

    bool NetworkUpdated()
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

    public void Release(Vector3 dir)
    {
        networkDiskDirection = dir;
        objDisk.transform.SetParent(null);
        RpcRelease(dir);
        CmdRelease(dir);
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
    // Called when this player is hit by the disk.  Called by the disk upon collision. -- Added by Cam 4/22/2018
    public void DiskHit()
    {
        // Hit animation.  Probably spawn some kind of explosion?

        // Send point info to gamecontroller
        gameControllerScript.Score(this.gameObject);

        // Set inactive
        gameObject.SetActive(false);
    }

    // Resores player if they are destroyed. Called by gameController object. -- Added by Cam 4/22/2018
    public void Restore()
    {
        // Reset position to spawn
        // 
        transform.position = spawnPoint;

        // Spawn animation.  Reverse explosion?

        // Set active (might need to move this to animation method, so it won't appear until animation is over)
        gameObject.SetActive(true);
    }
}