using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour {

	public gameObject gameController;
	public GameControllerScript gameControllerScript;
	
	Vector3 spawnPoint;
	
	// Use this for initialization
	void Start () {
		spawnPoint = Transform.parent.Transform.position;  // Could perhaps get rid of spawn object altogether and set initial position as spawnPoint.
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Called when this player is hit by the disk.  Called by the disk upon collision.
	void DiskHit() {
		// Hit animation.  Probably spawn some kind of explosion?
		
		
		// Set inactive
		SetActive(0);
		
		// Send point info to gamecontroller
		gameController.gameControllerScript.Score(this.gameObject);
	}
	
	// Respawns player to spawnpoint position.  Called by gameController object.
	void Respawn() {
		// Reset position to spawn
		Transform.position = spawnPoint;
		
		// Spawn animation.  Reverse explosion?
		
		// Set active (might need to move this to animation method, so it won't appear until animation is over)
		SetActive(1);
	}
}
