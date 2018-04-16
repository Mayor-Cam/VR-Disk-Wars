using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {
    public GameObject owner;
    public PlayerController ownerController;
    Rigidbody rb;
    public Vector3 diskDeparturePosition;
    
    // Use this for initialization
	void Start () {
    rb = GetComponent<Rigidbody>();
    owner = transform.parent.parent.gameObject;
    ownerController = owner.GetComponent<PlayerController>();
	}

    // Update is called once per frame
  void Update () {
    if(ownerController.networkDiskFired) {
        if(ownerController.isLocalPlayer){
        //Collision detection
        Vector3 startPosition = transform.position; // Position of disk at start of the frame
        RaycastHit hit;           
        float frameDistance = ownerController.networkDiskSpeed * Time.deltaTime; //The distance the disk is projected to travel in one frame
        bool isHit = false;
        while(rb.SweepTest(transform.forward, out hit, frameDistance)) {  //While the Sweeptest actually hits something (Collides)
            isHit = true;
            Vector3 reflect = Vector3.Reflect(transform.forward, hit.normal); //Get the reflect vector from collision point
            transform.position = transform.position + hit.distance * transform.forward;
            transform.forward = reflect;
            frameDistance -= hit.distance;
        }
        transform.Translate(Vector3.forward * frameDistance);
        if(isHit) {
          ownerController.CmdUpdateVector(transform.position, transform.forward, ownerController.networkDiskSpeed);
          ownerController.networkDiskDeparturePosition = transform.position;
          ownerController.networkDiskDirection = transform.forward;
          ownerController.networkDiskMagnitude = ownerController.networkDiskSpeed;
        }
      }
      else {
        if(diskDeparturePosition != ownerController.networkDiskDeparturePosition) {
          diskDeparturePosition = ownerController.networkDiskDeparturePosition;
          transform.position = diskDeparturePosition;
        }
        transform.forward = ownerController.networkDiskDirection;
        transform.Translate(Vector3.forward * ownerController.networkDiskSpeed * Time.deltaTime);
        diskDeparturePosition = ownerController.networkDiskDeparturePosition;
      }
    }
  }


	

	bool GetFired() {
    return ownerController.networkDiskFired;
	}
}
