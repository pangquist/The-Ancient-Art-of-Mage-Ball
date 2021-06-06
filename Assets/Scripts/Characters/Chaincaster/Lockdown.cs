using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockdown : Ability
{
    // This script handles the functions of Chaincaster's second ability, Lockdown
    // It inherits from the ability class.
    // It checks if the target is the ball and then spawns three chains that hold it
    // Author: Pär Ängquist

    private GameObject anchor1, anchor2, anchor3;

    [Header("Dependencies")]
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] Jailer jailerPassive;
    [SerializeField] GameObject chain;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform castPoint;

    [Header("Settings")]
    [SerializeField] Sprite abilityIcon;

    [Header("Values")]
    [SerializeField] float cooldown;
    [SerializeField] float maxRange;
    [SerializeField] float area;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    private void Start()
    {
        jailerPassive = gameObject.GetComponent<Jailer>();
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

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxRange))
        {
            Collider[] colliders = Physics.OverlapSphere(hit.point, area);
            foreach (Collider hitObject in colliders)
            {
                if (hitObject.CompareTag("Enemy"))
                {
                    jailerPassive.TriggerBuff();

                    useAbilities.SetOnCooldown(1, cooldown);

                    CmdCastSpell(hitObject.gameObject);
                }
            }
        }
    }

    [Command]
    void CmdCastSpell(GameObject target)
    {
        RpcCastSpell(target);
    }

    [ClientRpc]
    void RpcCastSpell(GameObject target)
    {
        if (NetworkServer.active)
        {
            LockdownBehaviour behave = chain.GetComponent<LockdownBehaviour>();

            anchor1 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;
            anchor2 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;
            anchor3 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;

            NetworkServer.Spawn(anchor1, connectionToClient);
            NetworkServer.Spawn(anchor2, connectionToClient);
            NetworkServer.Spawn(anchor3, connectionToClient);
        }
    }

    public override Sprite ReturnIcon()
    {
        return abilityIcon;
    }
}
