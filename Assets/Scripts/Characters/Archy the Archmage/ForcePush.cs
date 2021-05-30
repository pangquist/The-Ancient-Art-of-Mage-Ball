using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePush : NetworkBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

    [Header("Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask[] hitableLayers;
    [SerializeField] GameObject hitEffect;
    
    [Header("Values")]
    [SerializeField] float range;
    [SerializeField] float pushAmount;
    [SerializeField] float pushRadius;

    Rigidbody pushedBody;
    RaycastHit hit;

    #region Client

    private void Start()
    {
        
    }

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    void DoPush()
    {
        if (!hasAuthority)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        foreach (LayerMask hitableLayer in hitableLayers)
        {
            Physics.Raycast(ray, out hit, range, hitableLayer);
           
            if (hit.collider != null)
            {
                break;
            }
        }


        if (hit.collider == null)
        {
            Vector3 point = ray.origin + (ray.direction * range);
            CmdSpawnHitEffect(point);

            Collider[] missedColliders = Physics.OverlapSphere(point, pushRadius);
            foreach (Collider pushedObject in missedColliders)
            {
                if (pushedObject.CompareTag("Enemy"))
                {
                    Debug.Log("Client is pushing!");
                    CmdMoveBall(pushedObject.gameObject, hit.point);
                }
            }
            useAbilities.ReduceAllCooldowns(1, 0);
            useAbilities.SetOnCooldown(0);
            return;
        }

        useAbilities.ReduceAllCooldowns(1, 0);

        CmdSpawnHitEffect(hit.point);
        Collider[] colliders = Physics.OverlapSphere(hit.point, pushRadius);
        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                Debug.Log("Client is pushing!");
                CmdMoveBall(pushedObject.gameObject, hit.point);
            }
        }

        useAbilities.SetOnCooldown(0);
    }

    #endregion
    #region Server
    
    [Command]
    void CmdMoveBall(GameObject pushedObject, Vector3 location)
    {
        pushedObject.GetComponent<Rigidbody>().AddExplosionForce(pushAmount, location, pushRadius);
    }

    [Command]
    void CmdSpawnHitEffect(Vector3 hitLocation)
    {
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);
        Debug.Log($"Instantiating the hit effect: {magicExplosion}");
    }

    //[ClientRpc]
    //void RpcMoveBall(Vector3 location)
    //{
    //    Collider[] colliders = Physics.OverlapSphere(location, pushRadius);
    //    foreach (Collider pushedObject in colliders)
    //    {
    //        if (pushedObject.CompareTag("Enemy"))
    //        {
    //            Debug.Log($"Player: {gameObject.GetComponent<MyNetworkPlayer>().GetDisplayName()}, Ball: {pushedObject.gameObject.name}, Location: {location}, Velocity: {pushedObject.gameObject.GetComponent<Rigidbody>().velocity}, Push Amount: {pushAmount}");
    //            pushedObject.GetComponent<Rigidbody>().AddForce(new Vector3(0,200,0));
    //        }
    //    }

    //}
    #endregion
}
