using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceWall : NetworkBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask hitableLayer;
    [SerializeField] GameObject forceWall;
    [SerializeField] float range;
    [SerializeField] float duration;
    
    RaycastHit hit;


    [Client]
    public void DoForceWallSpell()
    {
        if (!hasAuthority)
        {
            return;
        }

        Debug.Log("Casting Force Wall");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, range, hitableLayer);

        if (hit.collider == null)
        {
            return;
        }

        CmdDoSpell(hit.point);
    }

    [Command]
    void CmdDoSpell(Vector3 hitLocation)
    {
        RpcDoSpell(hitLocation);
    }

    [ClientRpc]
    void RpcDoSpell(Vector3 hitLocation)
    {
        if (NetworkServer.active)
        {
            GameObject instantiatedForceWall = Instantiate(forceWall, hitLocation + new Vector3(0, 2, 0), gameObject.GetComponent<Transform>().transform.rotation) as GameObject;
            NetworkServer.Spawn(instantiatedForceWall);
        }
    }
}
