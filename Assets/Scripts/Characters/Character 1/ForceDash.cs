using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDash : NetworkBehaviour
{
    [SerializeField]
    Camera playerCamera;

    PlayerMovement playerMovement;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float cooldown;
    float timeSinceLastCast;
    bool isOnCooldown;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public override void OnStartAuthority()
    {
        enabled = true;
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
