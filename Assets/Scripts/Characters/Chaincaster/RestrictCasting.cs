using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictCasting : Ability
{
    // This script handles the functions of Chaincaster's third ability, Restrict Magic
    // It inherits from the ability class.
    // It checks if any player's are within the area of the ability and then set all their abilities on their full respective cooldowns
    // Players on the same team as the casting player won't be affected
    // Author: P?r ?ngquist

    [Header("Dependencies")]
    [SerializeField] MyNetworkPlayer playerInfo;
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] Jailer jailerPassive;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform castPoint;

    [Header("Settings")]
    [SerializeField] Sprite abilityIcon;

    [Header("Values")]
    [SerializeField] float cooldown;
    [SerializeField] float maxRange;
    [SerializeField] float area;

    [Header("Visual Effect")]
    [SerializeField] GameObject hitEffect;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    private void Start()
    {
        jailerPassive = gameObject.GetComponent<Jailer>();
        playerInfo = gameObject.GetComponent<MyNetworkPlayer>();
    }

    [Client]
    public override void UseAbility(int abilityIndex)
    {
        if (abilityIndex != 3)
        {
            return;
        }


        if (!hasAuthority)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxRange))
        {
            bool hitPlayer = false;

            Collider[] colliders = Physics.OverlapSphere(hit.point, area);
            foreach (Collider hitObject in colliders)
            {
                if (hitObject.CompareTag("Player"))
                {
                    MyNetworkPlayer targetInfo = hitObject.gameObject.GetComponent<MyNetworkPlayer>();

                    if(targetInfo.TeamName != playerInfo.TeamName)
                    {
                        jailerPassive.TriggerBuff();

                        CmdCastRestrict(hitObject.gameObject);

                        hitPlayer = true;
                    }
                }
            }

            if (hitPlayer)
            {
                useAbilities.SetOnCooldown(2, cooldown);

                CmdSpawnHitEffect(hit.point);
            }
            else
            {
                useAbilities.SetCooldownToPercentage(2, cooldown, 50);
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
    void CmdCastRestrict(GameObject target)
    {
        RpcCastRestrict(target);
    }

    [ClientRpc]
    void RpcCastRestrict(GameObject target)
    {
        UseAbilities targetAbilities = target.GetComponent<UseAbilities>();

        targetAbilities.StartAllCooldowns();
    }

    public override Sprite ReturnIcon()
    {
        return abilityIcon;
    }
}
