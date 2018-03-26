using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {
    public float diskSpeed = 10;
    bool diskFired = false;
    SphereCollider collider;
    Rigidbody rb;

    ///////////////////
    // Additions for game functionality
    // -- Cam 3/13/2018
    public GameObject gameController;   // Reference to gameController object.  Needed to communicate hits for points.  (may instead implement into player objects)
    public GameObject diskOwner;  // Identifier for which player this disk belongs to.  

    // public PlayerController playerScript;  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
    // public DummyController dummyController;

    Vector3 spawnPoint;
    //
    //////////////////

    // Use this for initialization
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();

        spawnPoint = transform.position;   // -- Added by Cam 3/14/2018
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
                if (hit.transform.gameObject.name == "DummyPlayer")  // If we've hit the dummy
                {
                    // Call the hit player's DiskHit method.
                    hit.transform.gameObject.GetComponent<DummyController>().DiskHit();
                    print("Dummy Hit Confirm");

                    // Call the disk's DestroyDisk method
                    DestroyDisk();

                    // hit.Transform.gameObject.playerScript.DiskHit();  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
                }

                else if (hit.transform.gameObject.CompareTag("Player") && hit.transform.gameObject != diskOwner) // If we've hit the other player...
                {
                    // Call the hit player's DiskHit method.
                    // hit.transform.gameObject.GetComponent<PlayerController>().DiskHit();

                    // Call the disk's DestroyDisk method
                    DestroyDisk();

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
	
	
	///////////////////
	// Additions for player/dummy collision
	// -- Cam 3/13/2018
	
	public void DestroyDisk() {
        // Spawn disk explosion animation

        // Set Inactive
        gameObject.SetActive(false);
	}

    public void Respawn()
    {
        gameObject.SetActive(false);  // Set inactive in case this disk is currently active
        diskFired = false;

        // Stop, re-orient, and reposition to spawnPoint.
        transform.position = spawnPoint;
        transform.eulerAngles = new Vector3(0, 0, 0);
        rb.velocity = new Vector3(0, 0, 0);

        // Set Active
        gameObject.SetActive(true);
    }
	
	// -- Cam 3/26/2018

    // Snap moves and re-orients the disk.  Used by hand objects for grabbing.
	public void Snap(Vector3 newPosition, Vector3 newAngle) 
	{
        transform.position = newPosition;
        transform.eulerAngles = newAngle;
	}

	//
	///////
}
