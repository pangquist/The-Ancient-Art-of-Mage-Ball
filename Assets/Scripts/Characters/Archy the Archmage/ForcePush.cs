using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePush : NetworkBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask[] hitableLayers;
    
    RaycastHit hit;

    [SerializeField] float range;
    [SerializeField] float pushAmount;
    [SerializeField] float pushRadius;
    [SerializeField] GameObject hitEffect;
    Rigidbody pushedBody;

    #region Client
    
    [Client]
    void DoPush()
    {
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
            return;
        }
        
        CmdSpawnHitEffect(hit.point);

        Collider[] colliders = Physics.OverlapSphere(hit.point, pushRadius);
        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                Debug.Log("Client is pushing!");
                CmdDoPush(pushedObject.gameObject);
            }
        }
    }
    
    #endregion
    #region Server

    [Command]
    void CmdSpawnHitEffect(Vector3 hitLocation)
    {
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);
    }

    [Command]
    void CmdDoPush(GameObject ball)
    {
        //ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //ball.GetComponent<Rigidbody>().AddExplosionForce(pushAmount, hit.point, pushRadius);
        //ServerMoveBall(ball);
        RpcMoveBall(ball);
    }

    [Server]
    void ServerMoveBall(GameObject ball)
    {
        RpcMoveBall(ball);
    }

    [ClientRpc]
    void RpcMoveBall(GameObject ball)
    {
        Debug.Log("Server is moving the ball for the clients!");
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().AddExplosionForce(pushAmount, hit.point, pushRadius);
    }
    #endregion
}
