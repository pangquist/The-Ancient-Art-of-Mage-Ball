using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDash : NetworkBehaviour
{
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] Camera playerCamera;
    [SerializeField] CharacterController controller;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float pushAmount;
    [SerializeField] float pushRadius;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject hitEffect;

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
        useAbilities.SetOnCooldownAbility3();
        useAbilities.ReduceAllCooldowns(1, 2);
        float startTime = Time.time;

        while(Time.time <startTime + dashDuration)
        {
            if (playerMovement.isGrounded)
            {
                controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            }
            else
            {
                controller.Move(playerCamera.transform.forward * dashSpeed * 2 * Time.deltaTime); //aerial dash
            }
            yield return null;
        }

        Vector3 hitLocation = transform.position + transform.forward * 3;

        CmdSpawnHitEffect(hitLocation);
        Collider[] colliders = Physics.OverlapSphere(hitLocation, pushRadius);
        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                CmdDoChargePush(pushedObject.gameObject, hitLocation);
            }
        }
    }

    [Command]
    void CmdSpawnHitEffect(Vector3 hitLocation)
    {
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);
    }

    [Command]
    void CmdDoChargePush(GameObject pushedObject, Vector3 _hitLocation)
    {
        pushedObject.GetComponent<Rigidbody>().AddExplosionForce(pushAmount, _hitLocation, pushRadius);
    }
}
