using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictCasting : NetworkBehaviour
{
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    [SerializeField] private float maxRange = 25f;

    public override void OnStartAuthority()
    {
        enabled = true;
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
            if(hit.transform.gameObject.tag == "Player")
            {
                CmdCastRestrict(hit.transform.gameObject);
            }
        }
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
