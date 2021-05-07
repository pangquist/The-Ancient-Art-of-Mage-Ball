using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDash : NetworkBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] CharacterController controller;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float cooldown;
    PlayerMovement playerMovement;
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
                controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            else
                controller.Move(playerCamera.transform.forward * dashSpeed * 2 * Time.deltaTime); //aerial dash
            yield return null;
        }
        
    }
}
