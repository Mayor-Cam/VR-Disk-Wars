using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour
{
    public bool grabbed = false;    // Added by Cam - 4/9/2918
    public GameObject owner;  // Identifier for which player this disk belongs to.  
    public PlayerController ownerController;
    Rigidbody rb;
    public Vector3 diskDeparturePosition;
    public Vector3 diskDeltaPosition;
    ///////////////////
    // Additions for game functionality and disk movement
    // -- Cam 3/13/2018 -- Updated 5/1/2018

    public GameObject gameController;   // Reference to gameController object.  Needed to communicate hits for points.  (may instead implement into player objects)
    // public PlayerController playerScript;  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
    // public DummyController dummyController;
    Vector3 spawnPoint;
    Vector3 lastPosition;
    public Vector3 currentVelocity;
    public float throwThreshold = 1.5f;
    GameObject anchorObj;
    public Transform anchorTrans;
    Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
    public float slerpSpeed = 5f;
    public float lerpSpeed = 20f;
    public float lerpSnap = 0.5f; // The distance the at which the disk will stop lerping and snap to the anchor.

    public float maxSpeed = 15f;  // The disk cannot travel faster than this speed
    public float cruiseSpeed = 6f; // When the disk is faster than terminal speed, it will slow down to match it.
    public float decelSpeed = 1.25f;
    //
    //////////////////

    // Variables for sound FX:
    AudioSource wall_hit_sound1;
    AudioSource wall_hit_sound2;
    AudioSource player_hit_sound1;
    AudioSource[] wallsSoundFX;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //Grab rigidbody (Needed for sweeptest)
        //owner = transform.parent.parent.gameObject; //Get parent object
        ownerController = owner.GetComponent<PlayerController>(); //Get the controller for player object

        //transform.position = new Vector3(0f, 0.75f, -2f);  // temporary hardcode of position
        spawnPoint = transform.position;

        //Queue sounds attached to Game Object
        wallsSoundFX = GetComponents<AudioSource>();
        wall_hit_sound1 = wallsSoundFX[0];
        wall_hit_sound2 = wallsSoundFX[1];
    }

    void Update()
    {
        if (ownerController.networkDiskFired)
        {

            if (ownerController.isLocalPlayer)
            { //if this disk belongs to the local player
                
               //Collision detection
                Vector3 startPosition = transform.position; // Position of disk at start of the frame
                RaycastHit hit;
                float frameDistance = ownerController.networkDiskSpeed * Time.deltaTime; //The distance the disk is projected to travel in one frame
                bool isHit = false;
                bool isTrigger = false;
                while (rb.SweepTest(transform.forward, out hit, frameDistance))
                {  //While the Sweeptest actually hits something (Collides)
                    if(hit.collider.isTrigger)
                    {
                        isTrigger = true;
                        break;
                    }
                    isHit = true; //hit condition boolean
                    Vector3 reflect = Vector3.Reflect(transform.forward, hit.normal); //Get the reflect vector from collision point
                    transform.position = transform.position + hit.distance * transform.forward; //Move the disk to where it would be during the collision
                    transform.forward = reflect; //change the disk's direction to its reflect vector (collision exit vector)
                    frameDistance = Mathf.Clamp(frameDistance - hit.distance, 0f, frameDistance); //Reduce frameDistance to what is left for calculation

                } //Basically this while loop will exit when the disk doesn't hit anything (or if the frameDistance = 0)

                ///////////////////
                // Additions for player/dummy collision
                // -- Cam 3/13/2018 -- edited 5/1/2018
                if (isTrigger)
                { 
                    print(hit.transform.gameObject.tag);
                    if (hit.transform.gameObject.tag == "Player" && hit.transform.gameObject != owner)  // If we've hit the dummy
                    {
                        if (hit.transform.gameObject.name == "DummyPlayer")  // if we've hit the dummy
                            hit.transform.gameObject.GetComponent<DummyController>().DiskHit();
                        else // We've hit the opposing player
                            hit.transform.gameObject.GetComponent<PlayerController>().DiskHit();
                        DestroyDisk();    // Call the disk's DestroyDisk method
                    }
                }
                //
                /////////////

                transform.Translate(Vector3.forward * frameDistance); //Basically, if frameDistance is still positive, but since it's out of the rb.Sweeptest loop (nothing hits), just move the disk forward for the remainder of frameDistance
                if (isHit)
                { //if the disk hit something this frame we can assume the disk's vector changed, so definitely update the ownercontroller's variablesi
                    ownerController.CmdUpdateVector(transform.position, transform.forward, ownerController.networkDiskSpeed);
                    ownerController.networkDiskDeparturePosition = transform.position;
                    ownerController.networkDiskDirection = transform.forward;
                    ownerController.networkDiskMagnitude = ownerController.networkDiskSpeed;
                    // Play sound on collision
                    // Picks random sound from array for variety
                    int index = Random.Range(0, wallsSoundFX.Length);
                    wallsSoundFX[index].Play();
                }
            }
            else
            { //disk doesn't belong to the local player
                if (diskDeparturePosition != ownerController.networkDiskDeparturePosition){ //if the diskDeparturePosition has changed (Means the disk has hit something since the last frame, and vector has changed)
                    diskDeparturePosition = ownerController.networkDiskDeparturePosition; //Change the new diskDeparturePosition, and
                    transform.position = diskDeparturePosition; //move the disk there (basically this also serves to resync the disks over the network, since its fresh data
                }
                transform.forward = ownerController.networkDiskDirection; //use the network info to point the disk in the direction that was given by the network (server)
                transform.Translate(Vector3.forward * ownerController.networkDiskSpeed * Time.deltaTime); //Move disk at the networkDiskSpeed
                diskDeparturePosition = ownerController.networkDiskDeparturePosition; //Update diskDepartuerPosition
            }
        }

        /////////////
        /// Added by Cam -- 4/9/2018
        /// last edited -- 4/16/2018
        /// 
        else if (grabbed)
        {
            if (gameObject.transform.parent != null)  // if we're currently attached to the anchor
            {
                // record motion
                currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
                lastPosition = transform.position;
            }
            // If the disk close enough to being aligned with the anchor...
            else if (Mathf.Abs(transform.position.magnitude - anchorTrans.position.magnitude) < lerpSnap && Mathf.Abs(transform.eulerAngles.magnitude - anchorTrans.eulerAngles.magnitude) < lerpSnap)    // (transform.position == anchorTrans.position && transform.eulerAngles == anchorTrans.eulerAngles)  // anchorTrans version         // if (transform.position == anchorObj.transform.position && transform.eulerAngles == anchorObj.transform.eulerAngles)  // anchorObj version
            {
                transform.position = anchorTrans.position; // newPosition;
                transform.rotation = anchorTrans.rotation; // newAngle
                gameObject.transform.parent = anchorTrans.parent;
                lastPosition = transform.position;
            }
            else  // lerp transform.position and transform.eulerAngles toward anchor position
            {
                transform.position = Vector3.Lerp(transform.position, anchorTrans.position, Time.deltaTime * lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, anchorTrans.rotation, Time.deltaTime * lerpSpeed);
            }
            
        }
        else // disk is idle - level out the eulerAngles (x & z) and maybe a float animation? 
        {
            // Smooth Lerp to level out.
            if (transform.rotation != targetRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * slerpSpeed);
            }
        }
        if(!ownerController.networkDiskFired){           
            if(!ownerController.isLocalPlayer) {
                transform.position = Vector3.Lerp(transform.position, ownerController.networkDiskNextPosition + diskDeltaPosition, Time.deltaTime * 60f);

                if (ownerController.NetworkUpdated())
                { //This boolean checks to see if new packets came in by seeing if the networkPlayerNewTimestamp variable (Time.time) changed
                    diskDeltaPosition = Vector3.zero; //if so, we have new positional data, so reset the delta position (for lerping inbetween network frames)
                    transform.position = ownerController.networkDiskNextPosition;
                }
                else
                {
                    diskDeltaPosition += ownerController.networkDiskVelocity * Time.deltaTime; //accumulated deltaposition over time (in between network frames)
                }
            }
            else {
                ownerController.networkDiskDirection = transform.forward;
                ownerController.networkDiskVelocity = (transform.position - ownerController.networkDiskNextPosition) / Time.deltaTime;
                ownerController.networkDiskNextPosition = transform.position;
            }
        }
    }

    // Additions for player/dummy collision
    // -- Cam 3/13/2018

    public void DestroyDisk()
    {
        // Spawn disk explosion animation

        // Set Inactive
        gameObject.SetActive(false);
    }
    public void Respawn()
    {
        gameObject.SetActive(false);  // Set inactive in case this disk is currently active
        if(ownerController.isServer) ownerController.networkDiskFired = false;
        else ownerController.CmdSetFired(false);

        // Stop, re-orient, and reposition to spawnPoint.
        transform.position = spawnPoint;
        transform.eulerAngles = new Vector3(0, 0, 0);
        targetRotation = Quaternion.Euler(0f, 0f, 0f);
        rb.velocity = new Vector3(0, 0, 0);

        // Set Active
        gameObject.SetActive(true);
    }

    /////////
    // Additions for grab/throw mechanics -- Cam 3/26/2018
    // Last edited -- 4/9/2018
    //

    // Snap moves and re-orients the disk.  Used by hand objects for grabbing.
    public void Snap(Vector3 newPosition, Vector3 newAngle)
    {
        transform.position = newPosition;
        transform.eulerAngles = newAngle;
    }

    public void SetParent(GameObject newParent)
    {
        if (gameObject.transform.parent != null)
        {
            HandGrabbing handScript = gameObject.transform.parent.GetComponent<HandGrabbing>();
            handScript.Release();
        }
        gameObject.transform.parent = newParent.transform;
    }

    public void Grab(Transform anchor) // Vector3 newPosition, Vector3 newAngle) 
    {
        grabbed = true;
        if(ownerController.isServer) ownerController.networkDiskFired = false;
        else ownerController.CmdSetFired(false);
        anchorTrans = anchor;

        // If it's already grabbed by a hand...
        if (gameObject.transform.parent != null)
        {
            // Get it to release the disk
            HandGrabbing handScript = gameObject.transform.parent.GetComponent<HandGrabbing>();
            handScript.Release();
        }

        rb.velocity = new Vector3(0f, 0f, 0f);    // Stop the disk

        // Snap to hand
        // transform.position = anchor.position; // newPosition;
        // transform.eulerAngles = anchor.eulerAngles; // newAngle;
        // gameObject.transform.parent = anchor.parent.transform;	// Set hand as parent
        // lastPosition = transform.position;

        print("GRABBING");
        //print("DiskController Hand position:" + anchor.parent.transform.position);
        //print("DiskController Anchor position:" + anchor.position);
    }

    public void Grab(GameObject anchor) // Vector3 newPosition, Vector3 newAngle) 
    {
        print("Grabbing");
        grabbed = true;
        if(ownerController.isServer) ownerController.networkDiskFired = false;
        else ownerController.CmdSetFired(false);
        anchorObj = anchor;
        anchorTrans = anchor.transform;

        // If it's already grabbed by a hand...
        if (gameObject.transform.parent != null)
        {
            // Get it to release the disk
            HandGrabbing handScript = gameObject.transform.parent.GetComponent<HandGrabbing>();
            handScript.Release();
        }

        rb.velocity = new Vector3(0f, 0f, 0f);  // Stop the disk

        // Snap to hand
        // transform.position = anchor.transform.position; // newPosition;
        // transform.eulerAngles = anchor.transform.eulerAngles; // newAngle;
        // gameObject.transform.parent = anchor.transform.parent.transform;  // Set hand as parent
        // lastPosition = transform.position;

        // print("DiskController Hand position:" + anchor.transform.parent.transform.position);
        // print("DiskController Anchor position:" + anchor.transform.position);
    }

    public void Release()
    {
        gameObject.transform.parent = null;
        grabbed = false;

        if (currentVelocity.magnitude > throwThreshold)
        {
            if(ownerController.isServer) ownerController.networkDiskFired = true;
            else ownerController.CmdSetFired(true);
            ownerController.networkDiskSpeed = Mathf.Clamp(currentVelocity.magnitude, throwThreshold, maxSpeed);

            // rb.velocity = currentVelocity;  // option 1: use the vector of the last two recorded points of the disk to impart velocity
            transform.forward = currentVelocity;  // option 2: use currentVelocity to determine the angle the disk should be... but Pat's code might already do this!

        }
        else
            SetIdle();
    }

    public void SetIdle()
    {
        grabbed = false;
        ownerController.networkDiskFired = false;
        Vector3 targetEuler = new Vector3(0f, transform.eulerAngles.y, 0f);

        if (transform.eulerAngles.x > 90 && transform.eulerAngles.x < 270)
            targetRotation.x = 180f;
        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
            targetRotation.z = 180f;

        targetRotation = Quaternion.Euler(targetEuler);
    }

    //
    ///////

}
