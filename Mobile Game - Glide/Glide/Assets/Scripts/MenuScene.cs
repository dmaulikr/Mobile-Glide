using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour {

    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.33f;

    public RectTransform menuContainer;

    public Transform levelPanel;
    public Transform colorPanel;
    public Transform trailPanel;
    public Text goldText;

    public Text colorBuySetText;
    public Text trailBuySetText;

    private int[] colorCost = new int[] { 0, 5, 5, 5, 10, 10, 10, 15, 15, 15 };
    private int[] trailCost = new int[] { 0, 20, 40, 40, 60, 60, 80, 80, 100, 100 };
    private int selectedColorIndex;
    private int selectedTrailIndex;
    private int activeColorIndex;
    private int activeTrailIndex;


    private Vector3 desiredMenuPosition;

    private GameObject currentTrail;

    public AnimationCurve enteringLevelZoomCurve;
    private bool isEnteringLevel = false;
    private float zoomDuration = 3.0f;
    private float zoomTransition;

    private MenuCamera menuCam;

    private void Start()
    {
        // Find the only MenuCamer and assign it
        menuCam = FindObjectOfType<MenuCamera>();

        // $$ TEMPORARY
        SaveManager.Instance.state.gold = 999;

        // Position our camera on the focused menu
        SetCameraTo(Manager.Instance.menuFocus);

        // Tell our gold text what to display
        UpdateGoldText();

        // Grab the only CanvasGroup in the scene
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start with a white screen
        fadeGroup.alpha = 1;

        // Add button on-click events to shop buttons
        InitializeShop();

        // Add buttons on-click events to level-select
        InitializeLevel();

        // Set player's preferences (color & trail)
        OnColorSelect(SaveManager.Instance.state.activeColor);
        SetColor(SaveManager.Instance.state.activeColor);

        OnTrailSelect(SaveManager.Instance.state.activeTrail);
        SetTrail(SaveManager.Instance.state.activeTrail);

        // Make the buttons bigger for the selected items
        colorPanel.GetChild(SaveManager.Instance.state.activeColor).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;

    }

    private void Update()
    {
        // Fade in
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        // Menu navigation (smooth)
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPosition, 0.1f);

        // Entering level zoom
        if(isEnteringLevel)
        {
            // Add to the zoomTransition float
            zoomTransition += (1 / zoomDuration) * Time.deltaTime;

            // Change the scale, following the animation curve
            menuContainer.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 5, enteringLevelZoomCurve.Evaluate(zoomTransition));

            // Change th edesired position of the canvas, so it can follow the scale up
            // This zooms in the center
            Vector3 newDesiredPosition = desiredMenuPosition * 5;
            // This adds to the specific position of the level on the canvas
            RectTransform rt = levelPanel.GetChild(Manager.Instance.currentLevel).GetComponent<RectTransform>();
            newDesiredPosition -= rt.anchoredPosition3D * 5;

            // This line will override the previous position update
            menuContainer.anchoredPosition3D = Vector3.Lerp(desiredMenuPosition, newDesiredPosition, enteringLevelZoomCurve.Evaluate(zoomTransition));

            // Fade to white screen, this will override the first line of the update
            fadeGroup.alpha = zoomTransition;

            // Are we done with the animation
            if(zoomTransition >= 1)
            {
                // Enter level!
                SceneManager.LoadScene("Game");
            }
        }
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

            // Set the color of the image, based on if owned or not (faded/sharp)
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsColorOwned(i) 
                ? Manager.Instance.playerColors[currentIndex] 
                : Color.Lerp(Manager.Instance.playerColors[currentIndex], new Color(0,0,0,1), 0.25f);

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

            // Set the color of the image, based on if owned or not (faded/sharp)
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsTrailOwned(i) ? Color.white : new Color(0.7f, 0.7f, 0.7f);

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

            Image img = t.GetComponent<Image>();

            // Is it unlocked?
            if(i <= SaveManager.Instance.state.completedLevel)
            {
                // It is unlocked!
                if(i == SaveManager.Instance.state.completedLevel)
                {
                    // It's not completed
                    img.color = Color.white;
                }
                else
                {
                    // Level is already unlocked
                    img.color = Color.green;
                }
            }
            else
            {
                // Level isn't unlocked, disable the button
                b.interactable = false;

                // Set to a dark color
                img.color = Color.grey;
            }

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
                menuCam.BackToMainMenu();
                break;

            // 1 = Play Menu
            case 1:
                desiredMenuPosition = Vector3.right * 1280;
                menuCam.MoveToLevel();
                break;
            // 2 = Shop MEnu
            case 2:
                desiredMenuPosition = Vector3.left * 1280;
                menuCam.MoveToShop();
                break;
        }
    }

    private void SetCameraTo(int menuIndex)
    {
        NavigateTo(menuIndex);
        menuContainer.anchoredPosition3D = desiredMenuPosition;
    }

    private void SetColor(int index)
    {
        // Set the active index
        activeColorIndex = index;
        SaveManager.Instance.state.activeColor = index;

        // Change the color on the player model
        Manager.Instance.playerMaterial.color = Manager.Instance.playerColors[index];
        
        // Change buy/set button text
        colorBuySetText.text = "Current";

        // Remember preferences
        SaveManager.Instance.Save();
    }

    private void  SetTrail(int index)
    {
        // Set the active index
        activeTrailIndex = index;
        SaveManager.Instance.state.activeTrail = index;

        // Change the color on the player model
        if(currentTrail != null)
        {
            Destroy(currentTrail);
        }

        // Create the new trail
        currentTrail = Instantiate(Manager.Instance.playerTrails[index]) as GameObject;

        // Set it as a child of the player
        currentTrail.transform.SetParent(FindObjectOfType<MenuPlayer>().transform);

        // Fix the wierd scaling/rotations issues
        currentTrail.transform.localPosition = Vector3.zero;
        currentTrail.transform.localRotation = Quaternion.Euler(0, 0, 90);
        currentTrail.transform.localScale = Vector3.one * 0.01f;

        // Change buy/set trail text
        trailBuySetText.text = "Current";

        // Remember preferences
        SaveManager.Instance.Save();
    }

    private void UpdateGoldText()
    {
        goldText.text = SaveManager.Instance.state.gold.ToString();
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

        // If the button clicked is already selected, exit
        if (selectedColorIndex == currentIndex)
            return;

        // Maker the icon slightly bigger
        colorPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;

        // Put the previous one to normal
        colorPanel.GetChild(selectedColorIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        // Set the selected color
        selectedColorIndex = currentIndex;

        //  Change the content of the buy/set button, depending on the state of the color
        if(SaveManager.Instance.IsColorOwned(currentIndex))
        {
            // Color is owned
            // Is it already our current color?
            if(activeColorIndex == currentIndex)
            {
                colorBuySetText.text = "Current";
            }
            else
            {
                colorBuySetText.text = "Select";
            }
            
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

        // If the button clicked is already selected, exit
        if (selectedTrailIndex == currentIndex)
            return;

        // Maker the icon slightly bigger
        trailPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;

        // Put the previous one to normal
        trailPanel.GetChild(selectedTrailIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        // Set the selected Trail
        selectedTrailIndex = currentIndex;

        //  Change the content of the buy/set button, depending on the state of the trail
        if (SaveManager.Instance.IsTrailOwned(currentIndex))
        {
            // Trail is owned
            // Is it already our current trail?
            if (activeTrailIndex == currentIndex)
            {
                trailBuySetText.text = "Current";
            }
            else
            {
                trailBuySetText.text = "Select";
            }
        }
        else
        {
            // Trail isn't owned
            trailBuySetText.text = "Buy: " + trailCost[currentIndex].ToString();
        }
    }

    private void OnLevelSelect(int currentIndex)
    {
        Manager.Instance.currentLevel = currentIndex;
        //SceneManager.LoadScene("Game");
        isEnteringLevel = true;

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

                // Change the color of the button
                colorPanel.GetChild(selectedColorIndex).GetComponent<Image>().color = Manager.Instance.playerColors[selectedColorIndex];

                // Update the gold text
                UpdateGoldText();
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

                // Change the color of the button
                trailPanel.GetChild(selectedTrailIndex).GetComponent<Image>().color = Color.white;

                // Update the gold text
                UpdateGoldText();
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