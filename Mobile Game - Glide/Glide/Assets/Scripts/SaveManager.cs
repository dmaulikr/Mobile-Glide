using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

	public static SaveManager Instance { set; get; }
    public SaveState state;

    private void Awake()
    {
        //ResetSave();

        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();

        // Are we using the accelereomter AND can we us it
        if(state.usingAccelerometer && !SystemInfo.supportsAccelerometer)
        {
            // If we cant, make sure we're not trying next time
            state.usingAccelerometer = false;
            Save();
        }
    }

    // Save the whole state of this saveState script to the player pref
    public void Save()
    {
        PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state));
    }

    // Load the previous save state from the player pref
    public void Load()
    {
        if(PlayerPrefs.HasKey("save"))
        {
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else
        {
            state = new SaveState();
            Save();
            Debug.Log("No save state found, creating new one");
        }
    }

    // Check if the color is owned
    public bool IsColorOwned(int index)
    {
        // Check if the bit is set, if so the color is owned
        return(state.colorOwned & (1 << index)) != 0;
    }

    // Check if the trail is owned
    public bool IsTrailOwned(int index)
    {
        // Check if the bit is set, if so the trail is owned
        return (state.trailOwned & (1 << index)) != 0;
    }

    // Attempt buying a color
    public bool BuyColor(int index, int cost)
    {
        if(state.gold >= cost)
        {
            // Enough money, remove from current gold stack
            state.gold -= cost;
            UnlockColor(index);

            // Save progress
            Save();

            return true;
        }
        else
        {
            // Not enough money, return false
            return false;
        }
    }

    // Attempt buying a trail
    public bool BuyTrail(int index, int cost)
    {
        if (state.gold >= cost)
        {
            // Enough money, remove from current gold stack
            state.gold -= cost;
            UnlockTrail(index);

            // Save progress
            Save();

            return true;
        }
        else
        {
            // Not enough money, return false
            return false;
        }
    }

    // Unlock a color in the "colorOwned" int
    public void UnlockColor(int index)
    {
        // Toggle on the bit at index
        state.colorOwned |= 1 << index; // |= is the toggle on
    }

    // Unlock a color in the "colorOwned" int
    public void UnlockTrail(int index)
    {
        // Toggle on the bit at index
        state.trailOwned |= 1 << index; // |= is the toggle on
    }

    // Complete Level
    public void CompleteLevel(int index)
    {
        // if this is the current active level
        if(state.completedLevel == index)
        {
            state.completedLevel++;
            Save();
        }
    }

    // Reset the whole save-file
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}