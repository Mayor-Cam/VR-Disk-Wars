using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {
  public GameObject owner;
  public PlayerController ownerController;
  Rigidbody rb;
  public Vector3 diskDeparturePosition;
	
	void Start () {
    rb = GetComponent<Rigidbody>(); //Grab rigidbody (Needed for sweeptest)
    owner = transform.parent.parent.gameObject; //Get parent object
    ownerController = owner.GetComponent<PlayerController>(); //Get the controller for parent object
	}

  void Update () {
    if(ownerController.networkDiskFired) {
      if(ownerController.isLocalPlayer){ //if this disk belongs to the local player
      //Collision detection
				Vector3 startPosition = transform.position; // Position of disk at start of the frame
				RaycastHit hit;           
	      float frameDistance = ownerController.networkDiskSpeed * Time.deltaTime; //The distance the disk is projected to travel in one frame
	      bool isHit = false;
	      while(rb.SweepTest(transform.forward, out hit, frameDistance)) {  //While the Sweeptest actually hits something (Collides)
	        isHit = true; //hit condition boolean
	        Vector3 reflect = Vector3.Reflect(transform.forward, hit.normal); //Get the reflect vector from collision point
	        transform.position = transform.position + hit.distance * transform.forward; //Move the disk to where it would be during the collision
	        transform.forward = reflect; //change the disk's direction to its reflect vector (collision exit vector)
	        frameDistance = Mathf.Clamp(frameDistance - hit.distance,0f, frameDistance); //Reduce frameDistance to what is left for calculation
	      } //Basically this while loop will exit when the disk doesn't hit anything (or if the frameDistance = 0)
	      transform.Translate(Vector3.forward * frameDistance); //Basically, if frameDistance is still positive, but since it's out of the rb.Sweeptest loop (nothing hits), just move the disk forward for the remainder of frameDistance
	      if(isHit) { //if the disk hit something this frame we can assume the disk's vector changed, so definitely update the ownercontroller's variablesi
	        ownerController.CmdUpdateVector(transform.position, transform.forward, ownerController.networkDiskSpeed); 
	        ownerController.networkDiskDeparturePosition = transform.position;
	        ownerController.networkDiskDirection = transform.forward;
	        ownerController.networkDiskMagnitude = ownerController.networkDiskSpeed;
	      }
	    }
	    else { //disk doesn't belong to the local player
				if(diskDeparturePosition != ownerController.networkDiskDeparturePosition) { //if the diskDeparturePosition has changed (Means the disk has hit something since the last frame, and vector has changed)
	        diskDeparturePosition = ownerController.networkDiskDeparturePosition; //Change the new diskDeparturePosition, and
	        transform.position = diskDeparturePosition; //move the disk there (basically this also serves to resync the disks over the network, since its fresh data
	      }
	      transform.forward = ownerController.networkDiskDirection; //use the network info to point the disk in the direction that was given by the network (server)
	      transform.Translate(Vector3.forward * ownerController.networkDiskSpeed * Time.deltaTime); //Move disk at the networkDiskSpeed
	      diskDeparturePosition = ownerController.networkDiskDeparturePosition; //Update diskDepartuerPosition
	    }
		}
	}

	bool GetFired() { //Returns if the disk is fired
    return ownerController.networkDiskFired;
	}
}
