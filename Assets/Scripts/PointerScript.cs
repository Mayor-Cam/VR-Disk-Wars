using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerScript : MonoBehaviour {
	public Transform selecting;
	public Material matSelected, matUnselected;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider trigger) {
		selecting = trigger.gameObject.transform;
		if(selecting.name == "textBackground"){
			selecting.GetComponent<Renderer>().material = matSelected;
		}
	}
	void OnTriggerExit(Collider trigger) {
		if(selecting.name == "textBackground"){
			selecting.GetComponent<Renderer>().material = matUnselected;
		}
	}
}
