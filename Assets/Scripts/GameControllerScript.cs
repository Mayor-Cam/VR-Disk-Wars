﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Cam 3/13/2018
// Last edited 4/22/2018
public class GameControllerScript : MonoBehaviour {

    public GameObject hostDisk;
    public GameObject clientDisk;
	public GameObject clientPlayer;
    public GameObject hostPlayer;
    public GameObject dummyPlayer;
    GameObject lastHitPlayer;  // The player most recently hit by the disk
    bool hit = false;
    // Scripts
    DiskController clientDiskController;
    public DiskController hostDiskController;
    public PlayerController clientController;
    public PlayerController hostController;
    public DummyController dummyController;

    public int winScore;

	// public GameObject hostSpawn;
	// public GameObject clientSpawn;

	int hostScore;
	int clientScore;
    float respTimer;
    public int timerMax = 1;
	
	// Use this for initialization
	void Start () {
		hostScore = 0;
		clientScore = 0;
        respTimer = -1;


        // Assign Objects
        hostPlayer = GameObject.Find("Player");
        // clientPlayer = GameObject.Find("Player(1)");
        // hostDisk = GameObject.Find("OldDisk");
        // clientDisk = GameObject.Find("OldDisk(1)");


        // Assign scripts
        // diskController = disk.GetComponent<DiskController>();
        hostController = clientPlayer.GetComponent<PlayerController>();
        // clientController = hostPlayer.GetComponent<PlayerController>();
        // dummyController = dummyPlayer.GetComponent<DummyController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(hit)
        {
          if (respTimer > 0)
          {
            respTimer+= -Time.deltaTime;
          }
          else
          {
            hit = false;

            // Restore players/dummies
            if (GameObject.ReferenceEquals(lastHitPlayer, hostPlayer))
                hostController.Restore();
            else if (GameObject.ReferenceEquals(lastHitPlayer, clientPlayer))
                clientController.Restore();
            else
                dummyController.Restore();

            // Respawn disks
            clientDiskController.Respawn();
            hostDiskController.Respawn();
          }
        }
    }
	
	/*
	The player object calls this method when it's hit.  
	It passes itself as a parameter so the gameController 
	can determine which player it is and properly allocate
	the score.
	*/
	public void Score(GameObject hitPlayer)
    {
	    hit = true;
        lastHitPlayer = hitPlayer;

        // Destroy the still active disk.
        if(hostDisk.activeSelf == false)
            hostDiskController.DestroyDisk();
        if (clientDisk != null) {
            if (clientDisk.activeSelf == false)
                clientDiskController.DestroyDisk();
        }

		if (GameObject.ReferenceEquals(hitPlayer, hostPlayer))  // If the hostPlayer was hit, score 1 for the clientPlayer.
			clientScore++;
		else  hostScore++;  // If not, score 1 for the hostPlayer.
        
        if(hostScore == winScore)
        {
            // Host win state
        }
        else if (clientScore == winScore)
        {
            // Client win state
        }

        SetTimer();
	}
    
    // Called when someone is hit by the disk.  
    public void SetTimer()
    {
        respTimer = timerMax;
    }
}
