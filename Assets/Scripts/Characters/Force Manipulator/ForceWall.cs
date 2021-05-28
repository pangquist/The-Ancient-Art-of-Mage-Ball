using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceWall : NetworkBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

    [Header("Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask hitableLayer;
    [SerializeField] GameObject forceWall;
    [SerializeField] AudioSource soundEffect;

    [Header("Values")]
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
            Vector3 point = ray.origin + (ray.direction * range);
            CmdDoSpell(point, false);
            useAbilities.ReduceAllCooldowns(1, 2);
            useAbilities.SetOnCooldown(2);
            return;
        }

        CmdDoSpell(hit.point, true);
        useAbilities.SetOnCooldown(2);
    }

    [Command]
    void CmdDoSpell(Vector3 hitLocation, bool hitGround)
    {
        RpcDoSpell(hitLocation, hitGround);
    }

    [ClientRpc]
    void RpcDoSpell(Vector3 hitLocation, bool hitGround)
    {
        soundEffect.Play();
        if (NetworkServer.active)
        {
            GameObject instantiatedForceWall;
            if (hitGround)
            {
                instantiatedForceWall = Instantiate(forceWall, hitLocation + new Vector3(0, 2, 0), gameObject.GetComponent<Transform>().transform.rotation) as GameObject;
            }
            else
            {
                instantiatedForceWall = Instantiate(forceWall, hitLocation, gameObject.GetComponent<Transform>().transform.rotation) as GameObject;
            }
            NetworkServer.Spawn(instantiatedForceWall);
        }
    }
}
