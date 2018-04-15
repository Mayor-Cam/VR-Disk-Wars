using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DiskController : NetworkBehaviour {
    public GameObject owner;
    public PlayerController ownerController;
    [SyncVar]
    public float networkDiskSpeed = 10;
    [SyncVar]
    public bool networkDiskFired = false;
    SphereCollider collider;
    Rigidbody rb;
    
    [SyncVar]
    public Vector3 networkDiskDeparturePosition;
    
    public Vector3 diskDeparturePosition;
    [SyncVar]
    public Vector3 networkDiskDirection;
    [SyncVar]
    public float networkDiskMagnitude;
    
    // Use this for initialization
	void Start () {
    collider = GetComponent<SphereCollider>();
    rb = GetComponent<Rigidbody>();
    owner = transform.parent.parent.gameObject;
    ownerController = owner.GetComponent<PlayerController>();
	}

    // Update is called once per frame
  void Update () {
    if(networkDiskFired) {
        if(ownerController.isLocalPlayer){
        //Collision detection
        Vector3 startPosition = transform.position; // Position of disk at start of the frame
        RaycastHit hit;           
        float frameDistance = networkDiskSpeed * Time.deltaTime; //The distance the disk is projected to travel in one frame
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
          CmdUpdateVector(transform.position, transform.forward, networkDiskSpeed);
          networkDiskDeparturePosition = transform.position;
          networkDiskDirection = transform.forward;
          networkDiskMagnitude = networkDiskSpeed;
        }
      }
      else {
        print("OH SHIT");
        if(diskDeparturePosition != networkDiskDeparturePosition) {
          diskDeparturePosition = networkDiskDeparturePosition;
          transform.position = diskDeparturePosition;
        }
        transform.forward = networkDiskDirection;
        transform.Translate(Vector3.forward * networkDiskSpeed * Time.deltaTime);
        diskDeparturePosition = networkDiskDeparturePosition;
      }
    }
  }

	[Command]
	void CmdUpdateVector(Vector3 pos, Vector3 dir, float mag){
    networkDiskDeparturePosition = pos;
    networkDiskDirection = dir;
    networkDiskMagnitude = mag;
	}
	

	bool GetFired() {
    return networkDiskFired;
	}
}
