using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockdown : NetworkBehaviour
{
    private GameObject anchor1, anchor2, anchor3;

    [SerializeField] GameObject chain;
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    [SerializeField] private float maxRange = 25f;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    void CastLockdown()
    {
        if(!hasAuthority)
        {
            return;
        }

        Debug.Log("Casting Lockdown");

        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxRange))
        {
            if(hit.transform.gameObject.tag != "Enemy")
            {
                return;
            }
           
            CmdCastSpell(hit.transform.gameObject);
        }
    }

    [Command]
    void CmdCastSpell(GameObject target)
    {
        Debug.Log("Casting Lockdown 2");
        RpcCastSpell(target);
    }

    [ClientRpc]
    void RpcCastSpell(GameObject target)
    {
        Debug.Log("Casting Lockdown 3");
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
}
