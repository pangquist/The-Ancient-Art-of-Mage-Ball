using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwap : Ability
{
    // This script handels the functioning of Force Manipulators second ability, Gravity Swap.
    // It inherits from the ability class.
    // It is responsible for turning the gravity off for the ball when the ability is casted through an animation.
    // Author: Valter Lindecrantz.

    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

    [Header("Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask hitableLayer;
    [SerializeField] AudioSource soundEffect;
    [SerializeField] Sprite abilityIcon;

    [Header("Values")]
    [SerializeField] float cooldown;
    [SerializeField] float range;
    [SerializeField] Vector3 appliedForce;
    [SerializeField] float duration;
    
    RaycastHit hit;
    GameObject ball;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    public override void UseAbility(int abilityIndex)
    {
        if (abilityIndex != 2)
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }

        Debug.Log("Casting Gravity Warp");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, range, hitableLayer);

        if (hit.collider == null || hit.collider.gameObject.tag != "Enemy")
        {
            useAbilities.SetCooldownToPercentage(1, cooldown, 50);
            return;
        }

        useAbilities.SetOnCooldown(1, cooldown);
        Debug.Log($"Hit object name: {hit.collider.gameObject}");
        ball = hit.collider.gameObject;
        CmdDoSpell(ball);
    }

    [Command]
    void CmdDoSpell(GameObject ballObject)
    {
        if (ballObject == null)
        {
            return;
        }

        ballObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ballObject.GetComponent<Rigidbody>().AddForce(appliedForce);

        ballObject.GetComponent<BallMovement>().RevertGravity(duration);

        RpcPlaySoundEffect();
    }

    [ClientRpc]
    void RpcPlaySoundEffect()
    {
        soundEffect.Play();
    }

    public override Sprite ReturnIcon()
    {
        return abilityIcon;
    }
}
