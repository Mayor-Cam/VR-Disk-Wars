using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarZoom : MonoBehaviour {
    public float posZ=25f;
    public float  posY=75f;
    public float posx=-500f;
    public float speed=10f;
    float timer;
    public bool dir=true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Zoom();
	}
    public void Zoom(){
        if(dir==true){
            posx+=50f*Time.deltaTime;
            this.transform.position=new Vector3((posx),this.transform.position.y,this.transform.position.z);
            if(posx>=100f){
                posx=-100;
            }  
        }
        else {
            posx-=50f*Time.deltaTime;
            this.transform.position=new Vector3((posx),this.transform.position.y,this.transform.position.z);
            if(posx<=-100f){
                posx=100;
            }
        }
    }  
}
