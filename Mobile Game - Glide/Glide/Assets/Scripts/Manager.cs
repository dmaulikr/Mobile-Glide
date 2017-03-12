using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	public static Manager Instance { set; get; }

    public Material playerMaterial;
    public Color[] playerColors = new Color[10];
    public GameObject[] playerTrails = new GameObject[10];

    public int currentLevel = 0;    // Used when changing from menu to game scene
    public int menuFocus = 0;       // Used when entering the menu scene to know which menu to focus

    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();

    private void Awake()
    {
        Instance = this;
    }
    

    public Vector3 GetPlayerInput()
    {
        // Are we using the accelerometer
        if(SaveManager.Instance.state.usingAccelerometer)
        {
            // If we use it, replace the Y param by Z, we don't need that Y
            Vector3 a = Input.acceleration;
            a.y = a.z;
            return a;
        }

        // Read all touches from the user
        Vector3 r = Vector3.zero;
        foreach(Touch touch in Input.touches)
        {
            // If we just started pressing on the screen
            if(touch.phase == TouchPhase.Began)
            {
                activeTouches.Add(touch.fingerId, touch.position);
            }
            // If we remove our figer off the screen
            else if(touch.phase == TouchPhase.Ended)
            {
                if (activeTouches.ContainsKey(touch.fingerId))
                {
                    activeTouches.Remove(touch.fingerId);
                }
            }
            // Our finger is either moving or stationary, in both cases, let's use the delta
            else
            {
                float mag = 0;                  // Distance we drag our finger across screen
                r = (touch.position - activeTouches[touch.fingerId]);
                mag = r.magnitude / 300;        // Deadzone for touch control
                r = r.normalized * mag;
            }
        }

        return r;
    }
}