using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

	public static SaveManager Instance { set; get; }
    public SaveState state;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();

        Debug.Log(state.colorOwned);
        UnlockColor(0);
        Debug.Log(state.colorOwned);
        UnlockColor(1);
        Debug.Log(state.colorOwned);
        UnlockColor(2);
        Debug.Log(state.colorOwned);
        UnlockColor(3);
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

    // Reset the whole save-file
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}