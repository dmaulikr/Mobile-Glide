using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour {

    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.33f;

    public RectTransform menuContainer;
    public Transform levelPanel;

    public Transform colorPanel;
    public Transform trailPanel;

    private Vector3 desiredMenuPosition;

    private void Start()
    {
        // Grab the only CanvasGroup in the scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start with a white screen
        fadeGroup.alpha = 1;

        // Add button on-click events to shop buttons
        InitializeShop();

        // Add buttons on-click events to level-select
        InitializeLevel();

    }

    private void Update()
    {
        // Fade in
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        // Menu navigation (smooth)
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPosition, 0.1f);
    }

    private void InitializeShop()
    {
        // Just make sure we've assigned the references
        if(colorPanel == null || trailPanel == null)
        {
            Debug.Log("You did not assign the color/trail panel in the inspector");
        }

        // For every children transform under our color panel, find the button and add on-click
        int i = 0;
        foreach(Transform t in colorPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnColorSelect(currentIndex));

            i++;
        }

        // Reset index 
        i = 0;
        // Do the same for the trail panel
        foreach (Transform t in trailPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnTrailSelect(currentIndex));

            i++;
        }
    }

    private void InitializeLevel()
    {
        // Just make sure we've assigned the references
        if (levelPanel == null)
        {
            Debug.Log("You did not assign the level panel in the inspector");
        }

        // For every children transform under our level panel, find the button and add on-click
        int i = 0;
        foreach (Transform t in levelPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnLevelSelect(currentIndex));

            i++;
        }
    }

    private void NavigateTo(int menuIndex)
    {
        switch(menuIndex)
        {
            // 0 and default case = Main Menu
            default:
            case 0:
                desiredMenuPosition = Vector3.zero;
                break;

            // 1 = Play Menu
            case 1:
                desiredMenuPosition = Vector3.right * 1280;
                break;
            // 2 = Shop MEnu
            case 2:
                desiredMenuPosition = Vector3.left * 1280;
                break;
        }
    }

    // Buttons
    public void OnPlayClick()
    {
        NavigateTo(1);
        Debug.Log("Play button pressed");
    }

    public void OnShopClick()
    {
        NavigateTo(2);
        Debug.Log("Shop button was clicked");
    }

    public void OnBackClick()
    {
        NavigateTo(0);
        Debug.Log("Back button has been clicked");
    }

    private void OnTrailSelect(int currentIndex)
    {
        Debug.Log("Selecting trail button : " + currentIndex);
    }

    private void OnColorSelect(int currentIndex)
    {
        Debug.Log("Selecting color button : " + currentIndex);
    }

    private void OnLevelSelect(int currentIndex)
    {
        Debug.Log("Selecting LEvel : " + currentIndex);
    }

    public void OnColorBuySet()
    {
        Debug.Log("Buy/Set color");
    }

    public void OnTrailBuySet()
    {
        Debug.Log("Buy/set trail");
    }
}