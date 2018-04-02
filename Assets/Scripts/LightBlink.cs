using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlink : MonoBehaviour {
	// Use this for initialization
    Light l;
    bool flag = true;
	void Start () {
        l = GetComponent<Light>();
	}
	
	// Update is called once per frame

 /*   public void Fader() {
        Light l = gameObject.GetComponent(typeof(Light)) as Light;
        bool flag=true;
        float t = 0.0f;
        while(true)
        {
            t += Time.deltaTime;
            if(flag){
                l.intensity=Mathf.Lerp(0.0f, 1.0f, t/4.0f);
                flag=false;
            }
            else{
                l.intensity= Mathf.Lerp(1.0f, 0.0f, t/4.0f);
            }
        }
    }*/
      /*void FadeLight ( Light l ,   float fadeStart ,   float fadeEnd ,   float fadeTime  ){
         float t= 0.0f;
         
         while (t < fadeTime) {
             t += Time.deltaTime;
             
             l.intensity = Mathf.Lerp(fadeStart, fadeEnd, t / fadeTime);

         }
     }*/
     void Update(){
        if(flag){
            l.intensity+=.01f;
            if (l.intensity>=3f){flag=false;} 
        }
        else{
            l.intensity-=.01f;
            if (l.intensity<=0f){flag=true;}
        }
     }

}
