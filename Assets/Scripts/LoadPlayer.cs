using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	
    public void OnMouseClick(){
        Application.LoadLevel("Player_Controller");
    }
	
}
