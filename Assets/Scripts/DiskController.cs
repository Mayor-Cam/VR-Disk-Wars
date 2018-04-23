﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour
{
    public bool grabbed = false;    // Added by Cam - 4/9/2918
    public GameObject owner;  // Identifier for which player this disk belongs to.  
    public PlayerController ownerController;
    Rigidbody rb;
    public Vector3 diskDeparturePosition;

    ///////////////////
    // Additions for game functionality and disk movement
    // -- Cam 3/13/2018 -- Updated 4/22/2018

    public GameObject gameController;   // Reference to gameController object.  Needed to communicate hits for points.  (may instead implement into player objects)
    // public PlayerController playerScript;  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
    // public DummyController dummyController;
    Vector3 spawnPoint;
    Vector3 lastPosition;
    Vector3 currentVelocity;
    public float throwThreshold = 1.5f;
    GameObject anchorObj;
    Transform anchorTrans;
    Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
    public float slerpSpeed = 0.01f;
    public float lerpSpeed = 0.1f;
    //
    //////////////////

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //Grab rigidbody (Needed for sweeptest)
        //owner = transform.parent.parent.gameObject; //Get parent object
        ownerController = owner.GetComponent<PlayerController>(); //Get the controller for player object
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
                while (rb.SweepTest(transform.forward, out hit, frameDistance) && !hit.collider.isTrigger)
                {  //While the Sweeptest actually hits something (Collides)
                    isHit = true; //hit condition boolean
                    Vector3 reflect = Vector3.Reflect(transform.forward, hit.normal); //Get the reflect vector from collision point
                    transform.position = transform.position + hit.distance * transform.forward; //Move the disk to where it would be during the collision
                    transform.forward = reflect; //change the disk's direction to its reflect vector (collision exit vector)
                    frameDistance = Mathf.Clamp(frameDistance - hit.distance, 0f, frameDistance); //Reduce frameDistance to what is left for calculation

                    ///////////////////
                    // Additions for player/dummy collision
                    // -- Cam 3/13/2018

                    if (hit.transform.gameObject.tag == "Player" && hit.transform.gameObject != owner)  // If we've hit the dummy
                    {

                        if (hit.transform.gameObject.name == "DummyPlayer")     // if we've hit the dummy
                            hit.transform.gameObject.GetComponent<DummyController>().DiskHit();
                        else // We've hit the opposing player
                            hit.transform.gameObject.GetComponent<PlayerController>().DiskHit();
                        DestroyDisk();    // Call the disk's DestroyDisk method
                    }

                    //
                    /////////////

                } //Basically this while loop will exit when the disk doesn't hit anything (or if the frameDistance = 0)
                transform.Translate(Vector3.forward * frameDistance); //Basically, if frameDistance is still positive, but since it's out of the rb.Sweeptest loop (nothing hits), just move the disk forward for the remainder of frameDistance
                if (isHit)
                { //if the disk hit something this frame we can assume the disk's vector changed, so definitely update the ownercontroller's variablesi
                    ownerController.CmdUpdateVector(transform.position, transform.forward, ownerController.networkDiskSpeed);
                    ownerController.networkDiskDeparturePosition = transform.position;
                    ownerController.networkDiskDirection = transform.forward;
                    ownerController.networkDiskMagnitude = ownerController.networkDiskSpeed;
                }
            }
            else
            { //disk doesn't belong to the local player
                if (diskDeparturePosition != ownerController.networkDiskDeparturePosition)
                { //if the diskDeparturePosition has changed (Means the disk has hit something since the last frame, and vector has changed)
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
            // if (transform.position == anchorObj.transform.position && transform.eulerAngles == anchorObj.transform.eulerAngles)  // anchorObj version
            if (transform.position == anchorTrans.position && transform.eulerAngles == anchorTrans.eulerAngles)  // anchorTrans version
            {
                // record motion
                currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
                lastPosition = transform.position;
            }
            else  // increment transform.position and transform.eulerAngles toward anchorObj
            {

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

        ///
        ///
        /////////////
    }
    ///////////////////
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
        ownerController.networkDiskFired = false;

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

    public void Grab(GameObject newParent, Transform anchor) // Vector3 newPosition, Vector3 newAngle) 
    {
        grabbed = true;
        ownerController.networkDiskFired = false;
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
        transform.position = anchor.position; // newPosition;
        transform.eulerAngles = anchor.eulerAngles; // newAngle;
        gameObject.transform.parent = newParent.transform;	// Set hand as parent
        print("GRABBING");
        //print("DiskController Hand position:" + newParent.transform.position);
        //print("DiskController Anchor position:" + anchor.position);


        lastPosition = transform.position;
    }

    public void Grab(GameObject newParent, GameObject anchor) // Vector3 newPosition, Vector3 newAngle) 
    {
        print("Grabbing");
        grabbed = true;
        ownerController.networkDiskFired = false;

        // If it's already grabbed by a hand...
        if (gameObject.transform.parent != null)
        {
            // Get it to release the disk
            HandGrabbing handScript = gameObject.transform.parent.GetComponent<HandGrabbing>();
            handScript.Release();
        }

        rb.velocity = new Vector3(0f, 0f, 0f);  // Stop the disk

        // Snap to hand
        transform.position = anchor.transform.position; // newPosition;
        transform.eulerAngles = anchor.transform.eulerAngles; // newAngle;
        gameObject.transform.parent = newParent.transform;  // Set hand as parent
        anchorObj = anchor;

        print("DiskController Hand position:" + newParent.transform.position);
        print("DiskController Anchor position:" + anchor.transform.position);


        lastPosition = transform.position;
    }

    public void Release()
    {
        gameObject.transform.parent = null;
        grabbed = false;

        if (currentVelocity.magnitude > throwThreshold)
        {
            ownerController.networkDiskFired = true;
            ownerController.networkDiskSpeed = Mathf.Clamp(currentVelocity.magnitude, 0f, 5f);

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
