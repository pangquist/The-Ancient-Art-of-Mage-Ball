using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictCasting : NetworkBehaviour
{
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] Transform castPoint, camera, player;
    [SerializeField] private float maxRange = 25f;
    [SerializeField] GameObject hitEffect;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    private void Start()
    {

    }

    [Client]
    void CastRestrict()
    {
        if (!hasAuthority)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxRange))
        {
            bool hitPlayer = false;

            Collider[] colliders = Physics.OverlapSphere(hit.point, 2.5f);
            foreach (Collider hitObject in colliders)
            {
                if (hitObject.CompareTag("Player"))
                {
                    CmdCastRestrict(hitObject.gameObject);

                    hitPlayer = true;
                }
            }

            if (hitPlayer)
            {
                useAbilities.SetOnCooldownAbility3();

                CmdSpawnHitEffect(hit.point);
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
}
