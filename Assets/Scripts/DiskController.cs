using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DiskController : NetworkBehaviour {
    public float diskSpeed = 10;
    bool diskFired = false;
    SphereCollider collider;
    Rigidbody rb;
    
    [SyncVar]
    Vector3 networkDiskDeparturePosition;
    [SyncVar]
    Vector3 networkDiskDirection;
    [SyncVar]
    float networkDiskMagnitude;
    
    // Use this for initialization
	void Start () {
    collider = GetComponent<SphereCollider>();
    rb = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void FixedUpdate () {
      if(diskFired) {
          if(isLocalPlayer){
              Vector3 orig = transform.position;
              RaycastHit hit;            
              float frameDistance = diskSpeed * Time.fixedDeltaTime;
              int counter = 0;
              while(rb.SweepTest(transform.forward, out hit, diskSpeed*Time.fixedDeltaTime) ) {
                  counter++;
                  Vector3 reflect = Vector3.Reflect(transform.forward, hit.normal);
                  transform.position = hit.point + transform.forward * 0.5f * transform.localScale.x * Mathf.Cos(Vector3.Angle(transform.forward, hit.normal)* Mathf.Deg2Rad);
                  transform.forward = reflect;
                  frameDistance -= hit.distance;
                  if(frameDistance <= 0) {
                    if(!isServer)
                      CmdUpdateVector(transform.position,transform.forward,diskSpeed /* <-- needs to change with the VR implementation*/);
                    else {
                        networkDiskDirection = transform.forward;
                        networkDiskMagnitude = diskSpeed;
                        networkDiskDeparturePosition = transform.position;
                    }
                    break;
                  }
              } 
          transform.Translate(Vector3.forward * frameDistance);
          rb.velocity = transform.forward * diskSpeed;
          orig = transform.position;
          Debug.DrawRay(orig+transform.forward*0.5f*transform.localScale.x,transform.forward*diskSpeed*Time.deltaTime,Color.red);
          
       }
      else {
        if(transform.forward != networkDiskDirection) 
          transform.forward = networkDiskDirection;
          transform.Translate(networkDiskDirection * diskSpeed * Time.fixedDeltaTime);
      }
		}
	}
	
	[Command]
	void CmdUpdateVector(Vector3 pos, Vector3 dir, float mag){
    transform.position = pos;
    networkDiskDeparturePosition = pos;
    networkDiskDirection = dir;
    networkDiskMagnitude = mag;
	}
	
	public bool GetFired() {
    return diskFired;
	}
	
	public void Fire(GameObject player, GameObject playerCamera) {
    transform.rotation = playerCamera.transform.rotation;
    diskFired = true;
    gameObject.transform.parent = null;
	}
	
    public void Catch(GameObject hand)
    {
        transform.position = hand.transform.position;
        transform.rotation = hand.transform.rotation;
        gameObject.transform.parent = hand.transform;
        diskFired = false;
    }
}
