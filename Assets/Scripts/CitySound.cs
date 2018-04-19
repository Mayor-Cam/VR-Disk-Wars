using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Matthew Spedale

This script handles playing ambient sounds to make the city sound "alive". 
It works by checking if a sound is playing, and if it isn't, 
generating a random number to pick a new sound out of the array of song files, then playing the sound in that element.
It also contains if statements to ensure the same sound is not played twice. 
This allows for randomization in the sound to avoid repetition.
*/

public class CitySound : MonoBehaviour
{

    // Variables for sound files:
    public AudioClip[] sound;  //array containing sounds
    public AudioSource currentSound;

    float[] validIndex = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f };
    public int prevIndex;
    private bool beingPlayed = false;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If nothing is being played, run the PlaySound() method to play one
        if (!beingPlayed)
        {
            StartCoroutine(PlaySound());
        }
    }

    // IEnumerator used to play sounds.
    // This allows for timing between sounds so that they are not constantly playing one after another.
    private IEnumerator PlaySound()
    {
        beingPlayed = true;
        currentSound.volume = 0.10f;  // keep it quiet since it's background noise

        if (!currentSound.isPlaying)
        {
            // Generates a random number to use to pick an element in the sound array
            // use Mathf.Round to ensure we can round up to 2, otherwise only sound[0] and [1] will ever play
            int index = (int)Mathf.Round(Random.Range(0.0f, 6.0f));

            // Three loops that check to see if the same index has been generated twice in a row.
            // If it has, then it picks a new index that will not be the same as the previous one

            if (prevIndex == index && prevIndex == 0)
            {
                index = (int)Mathf.Round(Random.Range(1.0f, 6.0f));
                print("prevIndex = 0, changing...");
                print("index = " + index);
            }

            if (prevIndex == index && prevIndex == 1)
            {
                float[] validIndex = { 0.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f };
                index = (int)validIndex[Random.Range(0, validIndex.Length)];
                print("prevIndex = 1, changing...");
                print("index = " + index);
            }

            if (prevIndex == index && prevIndex == 2)
            {
                float[] validIndex = {0.0f, 1.0f, 3.0f, 4.0f, 5.0f, 6.0f};
                index = (int)validIndex[Random.Range(0, validIndex.Length)];
                print("prevIndex = 2, changing...");
                print("index = " + index);
            }

            if (prevIndex == index && prevIndex == 4)
            {
                float[] validIndex = { 0.0f, 1.0f, 2.0f, 4.0f, 5.0f, 6.0f };
                index = (int)validIndex[Random.Range(0, validIndex.Length)];
                print("prevIndex = 3, changing...");
                print("index = " + index);
            }

            if (prevIndex == index && prevIndex == 5)
            {
                float[] validIndex = { 0.0f, 1.0f, 2.0f, 3.0f, 5.0f, 6.0f };
                index = (int)validIndex[Random.Range(0, validIndex.Length)];
                print("prevIndex = 4, changing...");
                print("index = " + index);
            }

            if (prevIndex == index && prevIndex == 6)
            {
                float[] validIndex = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 6.0f };
                index = (int)validIndex[Random.Range(0, validIndex.Length)];
                print("prevIndex = 5, changing...");
                print("index = " + index);
            }

            if (prevIndex == index && prevIndex == 6)
            {
                float[] validIndex = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f };
                index = (int)validIndex[Random.Range(0, validIndex.Length)];
                print("prevIndex = 6, changing...");
                print("index = " + index);
            }

            // Stores the previous index so we can check if the same number is picked twice, then plays the sound in that element.
            prevIndex = index;
            currentSound.clip = sound[index];
            print("Playing Sound: " + currentSound.clip);
            currentSound.Play();
        }

        var waitTime = new WaitForSeconds((int)Mathf.Round(Random.Range(2.0f, 4.0f)));
        yield return waitTime;
        beingPlayed = false;
    }
}
