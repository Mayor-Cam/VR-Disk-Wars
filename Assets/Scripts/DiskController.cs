using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {
    public float diskSpeed = 10;
    bool diskFired = false;
    SphereCollider collider;
    Rigidbody rb;
    // Use this for initialization
	void Start () {
    collider = GetComponent<SphereCollider>();
    rb = GetComponent<Rigidbody>();
	
	///////////////////
	// Additions for game functionality
	// -- Cam 3/13/2018
	public GameObject gameController;	// Reference to gameController object.  Needed to communicate hits for points.  (may instead implement into player objects)
	public GameObject diskOwner;  // Identifier for which player this disk belongs to.  
	
	// public PlayerController playerScript;  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
	public DummyController dummyScript;
	
	//
	///////////
	
	}
    // Update is called once per frame
    void FixedUpdate () {
		if(diskFired) {

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
				
				///////////////////
				// Additions for player/dummy collision
				// -- Cam 3/13/2018
					if 	(hit.Transform.gameObject.name == "DummyPlayer" || // If we've hit the dummy OR...
						(hit.Transform.gameObject.tag == "Player" && hit.Transform.gameObject.tag != diskOwner)) // If we've hit the other player...
					{
						// Call the hit player's DiskHit method.
						hit.Transform.gameObject.dummyScript.DiskHit();
						// hit.Transform.gameObject.playerScript.DiskHit();  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
					}
				//
				///////
            } 
			transform.Translate(Vector3.forward * frameDistance);
			orig = transform.position;
			Debug.DrawRay(orig+transform.forward*0.5f*transform.localScale.x,transform.forward*diskSpeed*Time.deltaTime,Color.red);
			
			
		}
		
		
	}
	
	
	/*
	///////////////////
	// Additions for player/dummy collision
	// -- Cam 3/13/2018
	
	void OnTriggerEnter(Collider other) {
        if 	(other.gameObject == "dummyPlayer" || // If we've hit the dummy OR...
			(other.gameObject.tag == "Player" && other.gameObject != diskOwner)) // If we've hit the other player...
		{
			
		}
    
	}
	
	//
	///////
	*/
	
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
	
	public void HitTarget() {
		// 
	}
}
