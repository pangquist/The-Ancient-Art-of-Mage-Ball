using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDash : NetworkBehaviour
{
    // This script handles the functioning of Archy's ability Force Dash.
    // When the animation is player, the Dash method is called and the ability is performed.
    // A networking part of this script is responsible for making sure that the balls gravity is synced between all the clients.
    // Author: Valter Lindecrantz

    [SerializeField] Camera playerCamera;
    [SerializeField] CharacterController controller;

    [Header("Dash Values")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;

    [Header("Explosion Values")]
    [SerializeField] float pushAmount;
    [SerializeField] float pushRadius;
    [SerializeField] GameObject hitEffect;

    PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public override void OnStartAuthority()
    {
        enabled = true;
    }
    
    // Method that is called every frame for the total duration of the dash. Every frame moves the player forward in a rapid pace. 
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

    [Command]
    void CmdDoChargePush()
    {
        Debug.Log("Doing charge push!");
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * 7, pushRadius);
        CmdSpawnHitEffect(transform.position + transform.forward * 7);
        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                pushedObject.GetComponent<Rigidbody>().AddExplosionForce(pushAmount * 1.5f, transform.position + transform.forward * 7, pushRadius * 1.5f);
            }
        }

    }

    //[Client]
    //void DoChargePush()
    //{
    //    Debug.Log("Doing charge push!");
    //    Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * 7, pushRadius);
    //    CmdSpawnHitEffect(transform.position + transform.forward * 7);
    //    foreach (Collider pushedObject in colliders)
    //    {
    //        if (pushedObject.CompareTag("Enemy"))
    //        {
    //            CmdDoChargePush(pushedObject.gameObject);
    //        }
    //    }
    //}

    [Command]
    void CmdSpawnHitEffect(Vector3 hitLocation)
    {
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);
    }
}
