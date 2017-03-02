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

    public Text colorBuySetText;
    public Text trailBuySetText;

    private int[] colorCost = new int[] { 0, 5, 5, 5, 10, 10, 10, 15, 15, 15 };
    private int[] trailCost = new int[] { 0, 20, 40, 40, 60, 60, 80, 80, 100, 100 };
    private int selectedColorIndex;
    private int selectedTrailIndex;


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

    private void SetColor(int index)
    {
        // Change the color on the player model

        // Change buy/set button text
        colorBuySetText.text = "Current";
    }

    private void  SetTrail(int index)
    {
        // Change the color on the player model

        // Change buy/set trail text
        trailBuySetText.text = "Current";
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

    private void OnColorSelect(int currentIndex)
    {
        Debug.Log("Selecting color button : " + currentIndex);

        // Set the selected color
        selectedColorIndex = currentIndex;

        //  Change the content of the buy/set button, depending on the state of the color
        if(SaveManager.Instance.IsColorOwned(currentIndex))
        {
            // Color is owned
            colorBuySetText.text = "Select";
        }
        else
        {
            // Color isn't owned
            colorBuySetText.text = "Buy: " + colorCost[currentIndex].ToString();
        }

    }

    private void OnTrailSelect(int currentIndex)
    {
        Debug.Log("Selecting trail button : " + currentIndex);

        // Set the selected Trail
        selectedTrailIndex = currentIndex;

        //  Change the content of the buy/set button, depending on the state of the trail
        if (SaveManager.Instance.IsTrailOwned(currentIndex))
        {
            // Trail is owned
            trailBuySetText.text = "Select";
        }
        else
        {
            // Trail isn't owned
            trailBuySetText.text = "Buy: " + trailCost[currentIndex].ToString();
        }
    }

    private void OnLevelSelect(int currentIndex)
    {
        Debug.Log("Selecting LEvel : " + currentIndex);
    }

    public void OnColorBuySet()
    {
        Debug.Log("Buy/Set color");

        // Is the selected color owned
        if(SaveManager.Instance.IsColorOwned(selectedColorIndex))
        {
            // Set the color!
            SetColor(selectedColorIndex);
        }
        else
        {
            // Attempt to buy the color
            if(SaveManager.Instance.BuyColor(selectedColorIndex, colorCost[selectedColorIndex]))
            {
                // Success! 
                SetColor(selectedColorIndex);
            }
            else
            {
                // Do not have enough gold! 
                // Play sound feedback
                Debug.Log("Not enough gold.");
            }
        }
    }

    public void OnTrailBuySet()
    {
        Debug.Log("Buy/set trail");

        // Is the selected trail owned
        if (SaveManager.Instance.IsTrailOwned(selectedTrailIndex))
        {
            // Set the trail!
            SetTrail(selectedTrailIndex);
        }
        else
        {
            // Attempt to buy the trail
            if (SaveManager.Instance.BuyTrail(selectedTrailIndex, trailCost[selectedTrailIndex]))
            {
                // Success! 
                SetTrail(selectedTrailIndex);
            }
            else
            {
                // Do not have enough gold! 
                // Play sound feedback
                Debug.Log("Not enough gold.");
            }
        }
    }
}