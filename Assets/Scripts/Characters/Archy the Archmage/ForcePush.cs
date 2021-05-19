using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePush : NetworkBehaviour
{
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask[] hitableLayers;
    
    RaycastHit hit;

    [SerializeField] float range;
    [SerializeField] float pushAmount;
    [SerializeField] float pushRadius;
    [SerializeField] GameObject hitEffect;
    Rigidbody pushedBody;

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

        useAbilities.ReduceAllCooldowns(1, 0);

        if (hit.collider == null)
        {
            return;
        }


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

        useAbilities.SetOnCooldownAbility1();
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
