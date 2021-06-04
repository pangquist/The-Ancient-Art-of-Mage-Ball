using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDash : Ability
{
    [Header("Script Dependencies")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] SpellSlinging spellSlinging;

    [Header("settings")]
    [SerializeField] Sprite abilityIcon;

    [Header("Values")]
    [SerializeField] float cooldown;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float pushAmount;
    [SerializeField] float pushRadius;

    [Header("Visual Effect")]
    [SerializeField] GameObject hitEffect;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    public override void UseAbility(int abilityIndex)
    {
        if (abilityIndex != 3)
        {
            return;
        }

        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        useAbilities.SetOnCooldown(2, cooldown);

        if (spellSlinging.enabled == true)
        {
            spellSlinging.SpellSling(1, 2);
        }
        float startTime = Time.time;
        bool isCloseToBall = false;

        Debug.Log($"Dash duration: {dashDuration}");

        while(Time.time < startTime + dashDuration && isCloseToBall == false)
        {
            if (playerMovement.isGrounded)
            {
                controller.Move(transform.forward * dashSpeed * Time.deltaTime);
                isCloseToBall = BallIsNear();
            }
            else
            {
                controller.Move(playerCamera.transform.forward * dashSpeed * 2 * Time.deltaTime); //aerial dash
                isCloseToBall = BallIsNear();
            }
            yield return null;
        }

        Vector3 hitLocation = transform.position + transform.forward;

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

    bool BallIsNear()
    {
        Collider[] checkForBall = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider pushedObject in checkForBall)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                return true;
            }
        }

        return false;
    }

    public override Sprite ReturnIcon()
    {
        return abilityIcon;
    }
}
