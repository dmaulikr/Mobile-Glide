using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMotor : MonoBehaviour {

    private CharacterController controller;

    [SerializeField]
    private float baseSpeed = 10.0f;
    [SerializeField]
    private float rotSpeedX = 10.0f;
    [SerializeField]
    private float rotSpeedY = -7f;

    private float deathTime;
    private float deathDuration = 2.0f;

    public GameObject deathExplosion;

    private void Start ()
    {
        controller = GetComponent<CharacterController>();

        // Create the trail
        GameObject trail = Instantiate(Manager.Instance.playerTrails[SaveManager.Instance.state.activeTrail]);

        // Set the trail as a child of the player model
        trail.transform.SetParent(transform.GetChild(0));

        // Fix the rotation of the trial
        trail.transform.localEulerAngles = Vector3.forward * -90f;
    }

    private void Update ()
    {
        // If the player is dead (has deathTime)
        if(deathTime != 0)
        {
            // Wait x sexongs then restart the level
            if(Time.time - deathTime > deathDuration)
            {
                SceneManager.LoadScene("Game");
            }

            return;
        }


        // Give the player forward velocity
        Vector3 moveVector = transform.forward * baseSpeed;

        // Gather players input
        Vector3 inputs = Manager.Instance.GetPlayerInput();

        // Get the delta direction
        Vector3 yaw = inputs.x * transform.right * rotSpeedX * Time.deltaTime;
        Vector3 pitch = inputs.y * transform.up * rotSpeedY * Time.deltaTime;
        Vector3 dir = yaw + pitch;

        // Make sure we limit the player from doing a loop
        float maxX = Quaternion.LookRotation(moveVector + dir).eulerAngles.x;

        // If player is not going too far up/down, add direction to moveVector
        if(maxX < 90 && maxX > 70 || maxX > 270 && maxX < 290)
        {
            // Too far, do nothing!
        }
        else
        {
            // Add the direction to the current move
            moveVector += dir;

            // Have the player face where he is going
            transform.rotation = Quaternion.LookRotation(moveVector);
        }

        // Move player!
        controller.Move(moveVector * Time.deltaTime);

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Set a death timestamp
        deathTime = Time.time;

        // Play explosion effect
        GameObject go = Instantiate(deathExplosion) as GameObject;
        go.transform.position = transform.position;

        // Hide the player mesh
        transform.GetChild(0).gameObject.SetActive(false);
    }
}