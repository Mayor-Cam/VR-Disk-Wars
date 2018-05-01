using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Cam 3/13/2018
// Last edited 3/14/2018

public class DummyController : MonoBehaviour {

	public GameObject gameController;
	GameControllerScript gameControllerScript;

    public float xMin = -2f;
    public float xMax = 2f;
    public float zMin = 0.75f;
    public float zMax = 4.75f;

    public Renderer rend;  // renderer variable
    public PolygonCollider2D poly;

    Vector3 spawnPoint;

    //AudioSource sound1;
    //AudioSource sound2;
    //AudioSource[] soundFX;

    // Use this for initialization
    void Start () {
        spawnPoint = transform.position;  // Set initial position as spawnPoint.

        gameControllerScript = gameController.GetComponent<GameControllerScript>();

        //audio
        //soundFX = GetComponents<AudioSource>();
        //sound1 = soundFX[0];
        //sound2 = soundFX[1];
    }
	
	// Update is called once per frame
	void Update ()
    {
      
    }
	
	// Called when this player is hit by the disk.  Called by the disk upon collision.
	public void DiskHit() {
        // Hit animation.  Probably spawn some kind of explosion?

        //int index = Random.Range(0, soundFX.Length);
        //soundFX[index].Play();
        //print("Playing hit sound");

        // Send point info to gamecontroller
        gameControllerScript.Score(this.gameObject);
        
        // Set inactive
        gameObject.SetActive(false);

	}
	
	// Resores player if they are destroyed. Called by gameController object.
	public void Restore() {
        // Reset position to spawn
        // 
        // transform.position = spawnPoint;
        transform.position = new Vector3(Random.Range(xMin, xMax), spawnPoint.y, Random.Range(zMin, zMax));
		
		// Spawn animation.  Reverse explosion?
		
		// Set active (might need to move this to animation method, so it won't appear until animation is over)
		gameObject.SetActive(true);
	}
}
