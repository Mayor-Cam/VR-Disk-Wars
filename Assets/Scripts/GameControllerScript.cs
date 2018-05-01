using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Cam 3/13/2018
// Last edited 4/22/2018
public class GameControllerScript : MonoBehaviour {

    public GameObject disk;
	//public GameObject clientPlayer;
    public GameObject hostPlayer;
    public GameObject dummyPlayer;
    bool hit = false;
    // Scripts
    DiskController clientDiskController;
    public DiskController hostDiskController;
    public PlayerController clientController;
    public PlayerController hostController;
    public DummyController dummyController;

	// public GameObject hostSpawn;
	// public GameObject clientSpawn;

	int hostScore;
	//int clientScore;
    float respTimer;
    public int timerMax = 10;

    // audio
    AudioSource sound1;
    AudioSource sound2;
    AudioSource sound3;
    AudioSource sound4;
    AudioSource[] soundFX;
    private bool beingPlayed = false;

    // Use this for initialization
    void Start () {
		hostScore = 0;
		//clientScore = 0;
        respTimer = -1;

        // Assign scripts
        //diskController = disk.GetComponent<DiskController>();
        //clientController = hostPlayer.GetComponent<PlayerController>();
        //hostController = clientPlayer.GetComponent<PlayerController>();
        //dummyController = dummyPlayer.GetComponent<DummyController>();

        //audio
        soundFX = GetComponents<AudioSource>();
        sound1 = soundFX[0]; //playerhit1
        sound2 = soundFX[1]; //playerhit2
        sound3 = soundFX[2]; //crowdcheer1
        sound4 = soundFX[3]; //crowdcheer2
    }

    // Update is called once per frame
    void Update () {
    if(hit) {
          if (respTimer > 0)
          {
              respTimer+= -Time.deltaTime;
          }
          else
          {
              //respTimer+=  -Time.deltaTime;
              hit = false;
              // Restore players
              dummyController.Restore();
              //clientController.Restore();
              //hostController.Restore();

              // Respawn disks
              //clientDiskController.Respawn();
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
	public void Score(GameObject hitPlayer) {
	    hit = true;

        //audio
        int index = Random.Range(0, 1);
        soundFX[index].Play();

        // this prevents the same sound from being played multiple times for a single action
        if (!beingPlayed)
        {
            StartCoroutine(PlaySound());
        }

        if (GameObject.ReferenceEquals(hitPlayer, hostPlayer)){}  // If the hostPlayer was hit, score 1 for the clientPlayer.
			//clientScore++;
		else  hostScore++;  // If not, score 1 for the hostPlayer.
        // 
        SetTimer();
	}
    
    // Called when someone is hit by the disk.  
    public void SetTimer()
    {
        respTimer = 1;
    }

    private IEnumerator PlaySound()
    {

        beingPlayed = true;

        // Checks if a sound is currently playing, and executes to play a new song if one isn't already.
        if (!soundFX[2].isPlaying && !soundFX[3].isPlaying)
        {
            int index = Random.Range(2, 3);

            // Generates a random number to use to pick an element in the sound array

            if (index == 0)
            {
                soundFX[2].Play();
            }
            else
            {
                soundFX[3].Play();
            }
        }
        var waitTime = new WaitForSeconds((int)Mathf.Round(Random.Range(2.0f, 5.0f)));
        yield return waitTime;
        beingPlayed = false;
    }
}
