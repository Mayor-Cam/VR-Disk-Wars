using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActivate : MonoBehaviour {
        public GameObject menu; //""""Pause"""" menu
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P)){
            menu.SetActive(true);
        }
	}
}
