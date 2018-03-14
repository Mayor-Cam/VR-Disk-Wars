using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Cam 3/13/2018
// Last edited 3/14/2018

public class DummyController : MonoBehaviour {

	public GameObject gameController;
	GameControllerScript gameControllerScript;
	
	Vector3 spawnPoint;
	
	// Use this for initialization
	void Start () {
		spawnPoint = transform.position;  // Set initial position as spawnPoint.

        gameControllerScript = gameController.GetComponent<GameControllerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Called when this player is hit by the disk.  Called by the disk upon collision.
	public void DiskHit() {
        // Hit animation.  Probably spawn some kind of explosion?

        // Send point info to gamecontroller
        gameControllerScript.Score(this.gameObject);

        // Set inactive
        gameObject.SetActive(false);
	}
	
	// Resores player if they are destroyed. Called by gameController object.
	public void Restore() {
        // Reset position to spawn
        // 
        transform.position = spawnPoint;
		
		// Spawn animation.  Reverse explosion?
		
		// Set active (might need to move this to animation method, so it won't appear until animation is over)
		gameObject.SetActive(true);
	}
}
