using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Probably won't be used, seems to be a lot simpler to use the Inspector for attaching sound
public class MatchMusic : MonoBehaviour {
//Handles music implementation for the array-based method of playing sounds. 

    static bool AudioBegin = false;
    //AudioSource jeffsong1;  // MainWIP
    AudioSource mattsong1;  // Polished Stone (Ambient Mix)


    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        //audio
        AudioSource[] song = GetComponents<AudioSource>();
        mattsong1 = song[0];
        //jeffsong1 = song[1];

        if (!AudioBegin)
        {
            mattsong1.Play();
            DontDestroyOnLoad(gameObject);
            AudioBegin = true;
        }
    }

    void Update()
    {
        
        if (Application.loadedLevelName == "test")
        {
            mattsong1.Stop();
            //jeffsong1.Play();    seems to have trouble starting new song, split scripts
            AudioBegin = false;
        }
    }
}
