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
            } 
        transform.Translate(Vector3.forward * frameDistance);
        orig = transform.position;
        Debug.DrawRay(orig+transform.forward*0.5f*transform.localScale.x,transform.forward*diskSpeed*Time.deltaTime,Color.red);
		}
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
