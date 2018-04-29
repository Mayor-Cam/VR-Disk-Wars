using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P)){
            GameObject.Find("PauseMenu").SetActive(false);
        }
	}
    public void play(){
        Application.LoadLevel("Network_Menu");
    }
    public void practice(){
        Application.LoadLevel("PracticeRoom");
    }
    public void options(){
        Application.LoadLevel("Player_Controller");
    }
    public void quit(){
        Application.Quit();
    }
    public void back(){
        Application.LoadLevel("Main_Menu");
    }
    public void host(){
        //Need to understand how the network manager works
    }
    public void find(){
        //Need to understand how the network manager works
    }
    public void menu(){
         Application.LoadLevel("Main_Menu"); // Separate to Afford other player a win?
    }
    
    public void resume(){
        GameObject.Find("PauseMenu").SetActive(false); //Press key, sleep.pause menu.
    }
}
