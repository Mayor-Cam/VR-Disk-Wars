using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {
  public float diskSpeed = 10;
  bool diskFired = false;
  // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(diskFired) {
      transform.Translate(Vector3.forward * diskSpeed * Time.deltaTime);
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
