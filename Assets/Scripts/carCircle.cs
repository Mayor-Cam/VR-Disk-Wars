using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carCircle : MonoBehaviour {
    public float cy = 79.00f;
    public float cx = 0 ;
    public float cz = 0;
    //Quaternion rotate = New Quaternion;
    public int radius = 25;
    float angle;
    float timer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Orbit();
	}
    public void Orbit() {
        timer += Time.deltaTime;
 
        this.transform.position=new Vector3((cx+Mathf.Sin(timer)*radius),cy,(cz+Mathf.Cos(timer)*radius));
        
        
    }
}
