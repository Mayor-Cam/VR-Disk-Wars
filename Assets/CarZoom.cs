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
            timer += Time.deltaTime;
            posx+=10;
            this.transform.position=new Vector3((posx+speed*(Time.deltaTime)),posY,posZ);
            if(posx>=1000f){
                posx=-1000;
            }  
        }
        else {
            timer += Time.deltaTime;
            posx-=10;
            this.transform.position=new Vector3((posx-(speed*Time.deltaTime)),posY,posZ);
            if(posx<=-1000f){
                posx=1000;
            }
        }
    }  
}
