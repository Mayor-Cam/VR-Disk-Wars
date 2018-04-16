using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {
    public float diskSpeed = 10;
    bool diskFired = false;
	bool grabbed = false; 	// Added by Cam - 4/9/2918
    SphereCollider collider;
    Rigidbody rb;

    ///////////////////
    // Additions for game functionality and disk movement
    // -- Cam 3/13/2018
    public GameObject gameController;   // Reference to gameController object.  Needed to communicate hits for points.  (may instead implement into player objects)
    public GameObject diskOwner;  // Identifier for which player this disk belongs to.  

    // public PlayerController playerScript;  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
    // public DummyController dummyController;

    Vector3 spawnPoint;
	Vector3 lastPosition;
	Vector3 currentVelocity;
	public float throwThreshold = 1f;
	GameObject anchorObj;

	Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
	public float slerpSpeed = 0.1f;
	public float lerpSpeed = 0.5f;
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
		if (diskFired) {

			Vector3 orig = transform.position;
			RaycastHit hit;            
			float frameDistance = diskSpeed * Time.fixedDeltaTime;
			int counter = 0;

			// Checking for collisions on next frame
			while (rb.SweepTest (transform.forward, out hit, diskSpeed * Time.fixedDeltaTime)) {
				counter++;
				Vector3 reflect = Vector3.Reflect (transform.forward, hit.normal);
				transform.position = hit.point + transform.forward * 0.5f * transform.localScale.x * Mathf.Cos (Vector3.Angle (transform.forward, hit.normal) * Mathf.Deg2Rad);
				transform.forward = reflect;
				frameDistance -= hit.distance;

				///////////////////
				// Additions for player/dummy collision
				// -- Cam 3/13/2018
				if (hit.transform.gameObject.name == "DummyPlayer") {  // If we've hit the dummy
					// Call the hit player's DiskHit method.
					hit.transform.gameObject.GetComponent<DummyController> ().DiskHit ();
					print ("Dummy Hit Confirm");

					// Call the disk's DestroyDisk method
					DestroyDisk ();

					// hit.Transform.gameObject.playerScript.DiskHit();  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
				} else if (hit.transform.gameObject.CompareTag ("Player") && hit.transform.gameObject != diskOwner) { // If we've hit the other player...
					// Call the hit player's DiskHit method.
					// hit.transform.gameObject.GetComponent<PlayerController>().DiskHit();

					// Call the disk's DestroyDisk method
					DestroyDisk ();

					// hit.Transform.gameObject.playerScript.DiskHit();  // Will be implemented later when Dummy collision/scoring methods are implemented into player object.
				}
				//
				///////
			} 
			transform.Translate (Vector3.forward * frameDistance);
			orig = transform.position;
			Debug.DrawRay (orig + transform.forward * 0.5f * transform.localScale.x, transform.forward * diskSpeed * Time.deltaTime, Color.red);
		}

		/////////////
		/// Added by Cam -- 4/9/2018
		/// last edited -- 4/16/2018
        /// 
		else if (grabbed) {

			if (transform.position == anchorObj.transform.position && transform.eulerAngles == anchorObj.transform.eulerAngles) {
				// record motion
				currentVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
				lastPosition = transform.position;
			} else {
				// increment transform.position and transform.eulerAngles toward anchorObj

			}
		} 
		else // disk is idle - level out the eulerAngles (x & z) and maybe a float animation? 
		{
			// Smooth Lerp to level out.
			if (transform.rotation != targetRotation) {
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.time * slerpSpeed);
			}
		}  
			
		///
		///
		/////////////
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
		if(gameObject.transform.parent != null)
		{
			HandGrabbing handScript = gameObject.transform.parent.GetComponent<HandGrabbing>();
			handScript.Release();
		}
		gameObject.transform.parent = newParent.transform;
	}

	public void Grab(GameObject newParent, Transform anchor) // Vector3 newPosition, Vector3 newAngle) 
	{
		grabbed = true;
		diskFired = false;
		
		// If it's already grabbed by a hand...
		if(gameObject.transform.parent != null)
		{
			// Get it to release the disk
			HandGrabbing handScript = gameObject.transform.parent.GetComponent<HandGrabbing>();
			handScript.Release();
		}

		rb.velocity = new Vector3(0f,0f,0f);    // Stop the disk

        // Snap to hand
        transform.position = anchor.position; // newPosition;
        transform.eulerAngles = anchor.eulerAngles; // newAngle;
        gameObject.transform.parent = newParent.transform;	// Set hand as parent

        print("DiskController Hand position:" + newParent.transform.position);
        print("DiskController Anchor position:" + anchor.position);


        lastPosition = transform.position;
	}

    public void Grab(GameObject newParent, GameObject anchor) // Vector3 newPosition, Vector3 newAngle) 
    {
        grabbed = true;
        diskFired = false;

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
        gameObject.transform.parent = newParent.transform;	// Set hand as parent
        anchorObj = anchor;

        print("DiskController Hand position:" + newParent.transform.position);
        print("DiskController Anchor position:" + anchor.transform.position);


        lastPosition = transform.position;
    }

    public void Release() 
	{
		gameObject.transform.parent = null;
		grabbed = false;

		if (currentVelocity.magnitude > throwThreshold) {
			diskFired = true;
			diskSpeed = currentVelocity.magnitude;

			// rb.velocity = currentVelocity;  // option 1: use the vector of the last two recorded points of the disk to impart velocity
			transform.forward = currentVelocity;  // option 2: use currentVelocity to determine the angle the disk should be... but Pat's code might already do this!
		} 
		else
			SetIdle();
	}
	
	public void SetIdle() 
	{
		grabbed = false;
		diskFired = false;
		Vector3 targetEuler = new Vector3(0, transform.eulerAngles.y, 0);

		if (transform.eulerAngles.x > 90 && transform.eulerAngles.x < 270)
			targetRotation.x = 180f;
		if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
			targetRotation.z = 180f;

		targetRotation = Quaternion.Euler(targetEuler);
	}

	//
	///////
}
