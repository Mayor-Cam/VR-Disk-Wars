using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public DiskController clientDiskController;
    public DiskController hostDiskController;
    public PlayerController clientController;
    public PlayerController hostController;
    public DummyController dummyController;

    public TextMesh remoteText;
    public TextMesh localText;
    public int winScore;
    bool gameOver= false;

	// public GameObject hostSpawn;
	// public GameObject clientSpawn;

	public int hostScore;
	public int clientScore;
    float respTimer;
    public int timerMax = 5;
	// Use this for initialization
    bool switchColor = false;
    public float exitTimer = 5f;
	void Start () {
		hostScore = 0;
		clientScore = 0;
        respTimer = -1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(hostController != null && clientController != null){
            if(hostController.isLocalPlayer) {
                localText.text = clientScore.ToString();
                remoteText.text = hostScore.ToString();
            }
            else {
                if(!switchColor) {
                    switchColor = true;
                    Color tempColor = localText.color;
                    localText.color = remoteText.color;
                    localText.transform.parent.GetComponent<TextMesh>().color = remoteText.color;
                    remoteText.color = tempColor;
                    remoteText.transform.parent.GetComponent<TextMesh>().color = tempColor;
                }
                localText.text = clientScore.ToString();
                remoteText.text = hostScore.ToString();
            }
        }
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
            hostDiskController.Respawn();
            if (clientDiskController != null)
                    clientDiskController.Respawn();
            }
        }
        if(gameOver){
            exitTimer += -Time.deltaTime;
            if(exitTimer < 0f) {
                SceneManager.LoadScene("MenuRoom");
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
        if(hostDisk.activeSelf)
            hostDiskController.DestroyDisk();
        if (clientDisk != null) {
            if (clientDisk.activeSelf)
                clientDiskController.DestroyDisk();
        }

		if (GameObject.ReferenceEquals(hitPlayer, hostPlayer))  // If the hostPlayer was hit, score 1 for the clientPlayer.
			clientScore++;
		else  hostScore++;  // If not, score 1 for the hostPlayer.
        if (!gameOver)
        {
            if (hostScore == winScore)
            {
                // Host win state

                // if localplayer is host
                if(hostController.isLocalPlayer)
                    hostController.Win();
                // else localplayer is client
                else
                    clientController.Lose();
                gameOver = true;
            }
            else if (clientScore == winScore)
            {
                // Client win state

                // if localplayer is host
                if(hostController.isLocalPlayer)
                    hostController.Lose();
                // else localplayer is client
                else
                    clientController.Win();

                gameOver = true;
            }
            else
                SetTimer();
        }
	}
    
    // Called when someone is hit by the disk.  
    public void SetTimer()
    {
        respTimer = timerMax;
    }
}
