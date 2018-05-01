using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Matthew Spedale
This script handles the music playing for the game. 
It works by checking if a song is playing, and if it isn't, 
generating a random number to pick a new song out of the array of song files, then playing the song in that element.
It also contains if statements to ensure the same song is not played twice. 
This allows for randomization in the music to avoid repetition.
*/

public class MusicPlayer : MonoBehaviour
{

    // Variables for music files:
    public AudioClip[] song;  //array containing songs
    //public AudioClip song2;  
    //public AudioClip song3;  
    public AudioSource currentSong;

    public int prevIndex;
    private bool beingPlayed = false;

    // Use this for initialization
    void Start()
    {

        //songArr = GetComponents<AudioSource>();
        //song1 = songArr[0];
        //song2 = songArr[1];
        //song3 = songArr[2];
    }

    // Update is called once per frame
    void Update()
    {

        // If nothing is being played, run the PlaySong() method to play one
        if (!beingPlayed)
        {
            StartCoroutine(PlaySong());
        }

    }

    private IEnumerator PlaySong()
    {

        beingPlayed = true;

        // Checks if a song is currently playing, and executes to play a new song if one isn't already.
        // Automatically plays a song as soon as the game starts as well
        if (!currentSong.isPlaying)
        {
            // Generates a random number to use to pick an element in the song array
            // use Mathf.Round to ensure we can round up to 2, otherwise only song[0] and [1] will ever play
            int index = (int)Mathf.Round(Random.Range(0.0f, 3.0f));

            // Three loops that check to see if the same index has been generated twice in a row.
            // If it has, then it picks a new index that will not be the same as the previous one
            if (prevIndex == index && prevIndex == 0)
            {
                index = (int)Mathf.Round(Random.Range(1.0f, 2.0f));
                print("prevIndex = 0, changing...");
            }

            if (prevIndex == index && prevIndex == 1)
            {
                float[] validIndex = { 0.0f, 2.0f, 3.0f };
                index = (int)validIndex[Random.Range(0, validIndex.Length)];
                print("prevIndex = 1, changing...");
            }

            if (prevIndex == index && prevIndex == 2)
            {
                index = (int)Mathf.Round(Random.Range(0.0f, 1.0f));
                print("prevIndex = 2, changing...");
            }

            if (prevIndex == index && prevIndex == 3)
            {
                float[] validIndex = { 0.0f, 1.0f, 2.0f };
                index = (int)validIndex[Random.Range(0, validIndex.Length)];
                print("prevIndex = 3, changing...");
            }

            // Stores the previous index so we can check if the same number is picked twice, then plays the song in that element.
            prevIndex = index;
            currentSong.clip = song[index];
            print("Playing Song: " + currentSong.clip);
            currentSong.Play();
        }

        // waits a few seconds before starting the next song
        var waitTime = new WaitForSeconds((int)Mathf.Round(Random.Range(4.0f, 6.0f)));
        yield return waitTime;
        beingPlayed = false;
    }
}