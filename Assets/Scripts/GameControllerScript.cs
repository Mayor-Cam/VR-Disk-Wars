﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Cam 3/13/2018
// Last edited 3/14/2018

public class GameControllerScript : MonoBehaviour {

    public GameObject disk;
    public GameObject hostPlayer;
	// public GameObject clientPlayer;
    public GameObject dummyPlayer;

    // Scripts
    DiskController diskController;
    PlayerController playerController;
    PlayerController clientController;
    DummyController dummyController;

	// public GameObject hostSpawn;
	// public GameObject clientSpawn;

	int hostScore;
	int clientScore;
    int respTimer;
    public int timerMax = 10;
	
	// Use this for initialization
	void Start () {
		hostScore = 0;
		clientScore = 0;
        respTimer = -1;

        // Assign scripts
        diskController = disk.GetComponent<DiskController>();
        playerController = hostPlayer.GetComponent<PlayerController>();
        // clientController = clientPlayer.GetComponent<PlayerController>();
        dummyController = dummyPlayer.GetComponent<DummyController>();
    }
	
	// Update is called once per frame
	void Update () {
		if (respTimer > 0)
        {
            respTimer--;
        }
        else if (respTimer == 0)
        {
            respTimer--;

            // Restore players, Respawn disks
            dummyController.Restore();
            // hostPlayer.playerController.Restore();
            // clientPlayer.playerController.Restore();
            diskController.Respawn();
        }

    }
	
	/*
	The player object calls this method when it's hit.  
	It passes itself as a parameter so the gameController 
	can determine which player it is and properly allocate
	the score.
	*/
	public void Score(GameObject hitPlayer) {
		if (GameObject.ReferenceEquals(hitPlayer, hostPlayer))  // If the hostPlayer was hit, score 1 for the clientPlayer.
			clientScore++;
		else  hostScore++;  // If not, score 1 for the hostPlayer.

        // 

        SetTimer();
	}
    
    // Called when someone is hit by the disk.  
    public void SetTimer()
    {
        respTimer = 200;
    }
}