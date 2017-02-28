using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoader : MonoBehaviour {

    private CanvasGroup fadeGroup;
    private float loadTime;
    private float minimumLogoTime = 3.0f; // Minimum time of that scene

    private void Start()
    {
        // Grab the only CanvasGroup in the scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start with a white screen
        fadeGroup.alpha = 1;

        // Preload the game
        // $$ - > Put assets here that needs importing from outside game

        // Get a timestamp of the completion time
        // if loadtime is superfast, give it a small buffer time so that we can appreciate the logo
        if(Time.time < minimumLogoTime)
        {
            loadTime = minimumLogoTime;
        }
        else
        {
            loadTime = Time.time;
        }
    }

    private void Update()
    {
        // Fade in
        if(Time.time < minimumLogoTime)
        {
            fadeGroup.alpha = 1 - Time.time;
        }

        // Fade out
        if(Time.time > minimumLogoTime && loadTime != 0)
        {
            fadeGroup.alpha = Time.time - minimumLogoTime;
            if(fadeGroup.alpha >= 1)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}