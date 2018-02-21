using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {
  public float diskSpeed = 10;
  bool diskFired = false;
  SphereCollider collider;
  // Use this for initialization
	void Start () {
    collider = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		if(diskFired) {
      Vector3 orig = transform.position;
      float rad = collider.radius;
      RaycastHit hit;
      
      if(Physics.SphereCast(orig,rad,transform.forward, out hit, diskSpeed* Time.deltaTime)) {
        Debug.Log("B: " + transform.forward);
        Vector3 reflect = Vector3.Reflect(transform.forward, hit.normal);
        Debug.Log("A: " + reflect);
        transform.forward = reflect;
        transform.position = hit.point+ transform.forward*0.5f*transform.localScale.x + (reflect*((diskSpeed*Time.deltaTime)-hit.distance)); 
        
      } 
      else {
        transform.Translate(Vector3.forward * diskSpeed * Time.deltaTime);
        }
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
	

}
