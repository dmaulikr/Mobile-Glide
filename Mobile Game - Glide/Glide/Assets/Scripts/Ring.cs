using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour {

    private Objective objectiveScript;
    private bool ringActive = false;

    private void Start()
    {
        objectiveScript = FindObjectOfType<Objective>();
    }

    public void ActivateRing()
    {
        ringActive = true;
    }

 	private void OnTriggerEnter(Collider other)
    {
        // If the ring is active
        // Tell the objective we collected it
        if(ringActive)
        {
            objectiveScript.NextRing();
            Destroy(gameObject, 5.0f);
        }
    }

    

}