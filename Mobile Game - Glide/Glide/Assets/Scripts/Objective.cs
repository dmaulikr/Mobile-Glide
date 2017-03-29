using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

    private List<Transform> rings = new List<Transform>();

    public Material activeRing;
    public Material inactiveRing;
    public Material finalRing;

    private int ringPassed = 0;

    private void Start()
    {
        // Set the objective field in the game scene
        FindObjectOfType<GameScene>().objective = this;

        // AT the start of level, assign Inactive to all rings
        foreach(Transform t in transform)
        {
            rings.Add(t);
            t.GetComponent<MeshRenderer>().material = inactiveRing;
        }

        // Making sure we're not stupid
        if(rings.Count == 0)
        {
            Debug.Log("THERE IS NO OBJECTIVE; DID YHOU PUT RINGS IN THERE?!?!?!");
            return;
        }

        // Activate the first ring
        rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
        rings[ringPassed].GetComponent<Ring>().ActivateRing();
    }

    public void NextRing()
    {
        // Play FX on the current ring 
        rings[ringPassed].GetComponent<Animator>().SetTrigger("collectionTrigger");

        // Up the in
        ringPassed++;

        // If it is the final ring, call it victory
        if(ringPassed == rings.Count)
        {
            Victory();
            return;
        }

        // If this is the next-to-last ring, give the last ring the "Final Ring" material
        if(ringPassed == rings.Count -1)
        {
            rings[ringPassed].GetComponent<MeshRenderer>().material = finalRing;
        }
        else
        {
            rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
        }

        // In both cases we need to activate the ring
        rings[ringPassed].GetComponent<Ring>().ActivateRing();

    }

    public Transform GetCurrentRing()
    {
        return rings[ringPassed];
    }

    private void Victory()
    {
        FindObjectOfType<GameScene>().CompleteLevel();
    }

}