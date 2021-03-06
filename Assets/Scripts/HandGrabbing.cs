﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; //needs to be UnityEngine.VR in version before 2017.2

public class HandGrabbing : MonoBehaviour
{

    public string InputName;
    public XRNode NodeType;
    public Vector3 ObjectGrabOffset;
    public float GrabDistance = 0.1f;
    public string GrabTag = "Grab";
    public float ThrowMultiplier = 1.5f;

    bool grabbing;
    public bool triggerPress;
    public bool triggerHold;
    public bool triggerRelease;
    public bool isLeftHand;  // Identifies which hand this object is.  Using a boolean instead of a string for the sake of efficiency.
    public float triggerInput; // Value of the corresponding hand's input.  Range from 0 to 1.
    public DiskController diskController;
    public Transform anchor;
    public GameObject anchorObj;

    public Transform currentObject;
    private Vector3 _lastFramePosition;
    public GameObject otherHand;

    public bool isLocal;

    public GameObject diskObj;  /// 4/18/2018 --  Cam: Direct DiskObject reference
    DiskController diskcontroller;

    // Use this for initialization
    void Start()
    {
        currentObject = null;
        _lastFramePosition = transform.position;

        grabbing = false;
        triggerPress = false;
        triggerHold = false;
        triggerRelease = false;
        diskObj = transform.parent.GetComponent<PlayerController>().objDisk;
        diskController = diskObj.GetComponent<DiskController>();  /// 4/18/2018 --- Cam: Direct DiskObject reference

        ////////
        ///  Added by Cam -- 3/26/2018
        /// 

        // Determine which hand this object is.
       // if (string.Compare(gameObject.name, "leftHand") == 1)
       // {
       //     isLeftHand = true;
       // }
       // else isLeftHand = false;

        // anchor = transform.GetChild[0].transform;

        //
        ///////
    }

    // Update is called once per frame
    void Update()
    {
        //update hand position and rotation
        if (isLocal)
        {
            transform.localPosition = InputTracking.GetLocalPosition(NodeType);
            transform.localRotation = InputTracking.GetLocalRotation(NodeType);
        }


        ///////////////
        // Added by Cam - 3/26/2018
        // Last updated - 4/2/2018 
        //

        // Open-ended grabbing implementation

        GetInput();

        // Grabbing a disk
        if (triggerPress && !grabbing)
        {
            //check for colliders in proximity
            //Collider[] colliders = Physics.OverlapSphere(transform.position, GrabDistance);
            print(transform.name + ": " + "Disk (" + diskObj.transform.position + ") Hand (" + transform.position + ") :: " + Vector3.Distance(diskObj.transform.position, transform.position) + " <= " + GrabDistance + "?");
            if (Vector3.Distance(diskObj.transform.position, transform.position) <= GrabDistance)
            {
                print("attempting to grab");
                // If we collided with something
                //if (colliders.Length > 0)
                // {
                // If the collided object is a disk
                //if (colliders[0].transform.gameObject.tag == "Disk")
                // {
                //currentObject = colliders[0].transform;
                currentObject = diskObj.transform;
                //diskController = currentObject.gameObject.GetComponent<DiskController>();

                // If it's already being grabbed by your other hand, release it.
                if (currentObject.parent != null)
                {
                    HandGrabbing handScript = currentObject.transform.parent.GetComponent<HandGrabbing>();
                    handScript.Release();
                    // diskController.Release();  // Removed this because handScript.Release() should do this for us.
                }

                // print("HandGrabbing Hand position:" + gameObject.transform.position);
                // print("HandGrabbing Anchor position:" + anchor.position);

                diskController.Grab(anchor); // (second gameObject will be anchor) //gameObject.transform.position, gameObject.transform.eulerAngles);
                grabbing = true;
                //     }
                //}
            }
        }

        // Releasing a disk
        if (grabbing)
        {
            if (triggerRelease)
            {
                print("RELEASING");
                Release();
            }
        }

        //
        /////////

        /*  Original "gimme" code
        
        //if we don't have an active object in hand, look if there is one in proximity
        if (currentObject == null)
        {
            //check for colliders in proximity
            Collider[] colliders = Physics.OverlapSphere(transform.position, GrabDistance);
            if (colliders.Length > 0)
            {
                //if there are colliders, take the first one if we press the grab button and it has the tag for grabbing
                if (Input.GetAxis(InputName) >= 0.01f && colliders[0].transform.CompareTag(GrabTag))
                {
                    //set current object to the object we have picked up
                    currentObject = colliders[0].transform;

                    //if there is no rigidbody to the grabbed object attached, add one
                    if(currentObject.GetComponent<Rigidbody>() == null)
                    {
                        currentObject.gameObject.AddComponent<Rigidbody>();
                    }

                    //set grab object to kinematic (disable physics)
                    currentObject.GetComponent<Rigidbody>().isKinematic = true;


                }
            }
        }
        else
        //we have object in hand, update its position with the current hand position (+defined offset from it)
        {
            currentObject.position = transform.position + ObjectGrabOffset;

            //if we we release grab button, release current object
            if (Input.GetAxis(InputName) < 0.01f)
            {
                //set grab object to non-kinematic (enable physics)
                Rigidbody _objectRGB = currentObject.GetComponent<Rigidbody>();
                _objectRGB.isKinematic = false;

                //calculate the hand's current velocity
                Vector3 CurrentVelocity = (transform.position - _lastFramePosition) / Time.deltaTime;

                //set the grabbed object's velocity to the current velocity of the hand
                _objectRGB.velocity = CurrentVelocity * ThrowMultiplier;

                //release the reference
                currentObject = null;
            }

        }

        //save the current position for calculation of velocity in next frame
        _lastFramePosition = transform.position;

    */
        //diskController.Grab(anchor); // (second gameObject will be anchor) //gameObject.transform.position, gameObject.transform.eulerAngles);
        //grabbing = true;
    }

    ///////////////
    // Added by Cam - 3/26/2018
    // Last updated - 4/2/2018 
    //

    // Records input for the controller that corresponds to this hand object.
    void GetInput()
    {
        // Determining which controller to get input from
        if (isLeftHand)
        {
            triggerInput = Input.GetAxis("TriggerLeft");
        }
        else triggerInput = Input.GetAxis("TriggerRight");

        // If-statements for declaring triggerPress, triggerHold, triggerRelease booleans.
        if (triggerInput == 1.0f)  // Pressed  -- May want to fiddle with the threshold
        {
            if (triggerHold == false)
            {
                triggerPress = true;
                triggerHold = true;
                // print("TriggerPress");
            }
            else triggerPress = false;

            // triggerHold = true;
        }
        else //  if (triggerInput < 1.0f) // Released
        {
            if (triggerHold == true)
            {
                triggerRelease = true;
                triggerHold = false;
                // print("TriggerRelease");
            }
            else triggerRelease = false;

            // triggerHold = false;
        }
    }

    public void Release()
    {
        diskController.Release();
        grabbing = false;
        currentObject = null;
    }

    ////////////////////
    /// Added by Cam -- 4/18/2018
    /// Direct DiskObject reference  

    /*
    // Trigger collider grabbing implementation.
    void OnTriggerStay(Collider other)
    {
        GetInput();

        // If we're not currently grabbing a disk
        print(other.name);

        if (grabbing)
        {
            if (triggerRelease)
            {
                print("RELEASING");
                Release();
            }
        }
        else if (triggerPress) // Not currently grabbing, but pressing the trigger
        {
            // If we're colliding with the disk
            if (GameObject.ReferenceEquals(other.gameObject, diskObj))
            {
                // If it's already being grabbed by your other hand, release it.
                if (diskObj.transform.parent != null)
                {
                    //HandGrabbing handScript = diskObj.transform.parent.GetComponent<HandGrabbing>();
                    otherHand.GetComponent<HandGrabbing>().Release();
                    // diskController.Release();  // Removed this because handScript.Release() should do this for us.
                }

                diskController.Grab(anchor); // (second gameObject will be anchor) //gameObject.transform.position, gameObject.transform.eulerAngles);
                grabbing = true;
            }
        }
    }
    */

    ///
    ///
    //////////////////
}