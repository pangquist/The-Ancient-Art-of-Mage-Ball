using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDash : NetworkBehaviour
{
    [SerializeField]
    Camera playerCamera;

    PlayerMovement playerMovement;
    [SerializeField]
    float dashSpeed;
    [SerializeField]
    float dashDuration;

    [SerializeField]
    float cooldown;
    float timeSinceLastCast;
    [SerializeField]
    public static float maxCharges = 2;
    public static float currentNumberOfCharges;
    bool isOnCooldown;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        currentNumberOfCharges = maxCharges;
    }

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentNumberOfCharges != maxCharges)
        {
            timeSinceLastCast += Time.deltaTime;
            if (timeSinceLastCast >= cooldown)
            {
                timeSinceLastCast = 0;
                currentNumberOfCharges++;
            }
        }
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;

        while(Time.time <startTime + dashDuration)
        {
            if (playerMovement.isGrounded)
                playerMovement.controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            else
                playerMovement.controller.Move(playerCamera.transform.forward * dashSpeed * 2 * Time.deltaTime); //aerial dash
            yield return null;
        }
    }
}
