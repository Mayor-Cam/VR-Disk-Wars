using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Cam 3/13/2018
// Last edited 3/13/2018

public class GameControllerScript : MonoBehaviour {
	
	public gameObject hostPlayer;
	public gameObject clientPlayer;
	
	public gameObject hostSpawn;
	public gameObject clientSpawn;
	
	int hostScore;
	int clientScore;
	
	// Use this for initialization
	void Start () {
		hostScore = 0;
		clientScore = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/*
	The player object calls this method when it's hit.  
	It passes itself as a parameter so the gameController 
	can determine which player it is and properly allocate
	the score.
	*/
	void Score(gameObject hitPlayer) {
		if (GameObject.ReferenceEquals(hitPlayer, hostPlayer))  // If the hostPlayer was hit, score 1 for the clientPlayer.
			clientScore++;
		else  hostScore++;  // If not, score 1 for the hostPlayer.
	}
}
