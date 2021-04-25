using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePush : NetworkBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] LayerMask hitableLayer;
    
    RaycastHit hit;

    [SerializeField] float range;
    [SerializeField] float pushAmount;
    [SerializeField] float pushRadius;
    [SerializeField] float forceJumpHeight;
    [SerializeField] GameObject hitEffect;
    Rigidbody pushedBody;

    #region Client

    public override void OnStartAuthority()
    {
        enabled = true;
    }
    
    [Client]
    void DoPush()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, range, hitableLayer);

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
    
    

    [Client]
    void DoForceJump()
    {
         playerMovement.velocity.y += forceJumpHeight;
    }
    #endregion
    #region Server

    [Command]
    void CmdSpawnHitEffect(Vector3 hitLocation)
    {
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosion);
        Debug.Log($"Intantiating the hit effect: {magicExplosion}");
    }

    [Command]
    void CmdDoChargePush()
    {
        pushedBody.AddExplosionForce(pushAmount * 1.5f, transform.position + transform.forward * 7, pushRadius * 1.5f);
    }

    [ClientCallback]
    void DoChargePush()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * 7, pushRadius);
        CmdSpawnHitEffect(transform.position + transform.forward * 7);
        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                pushedBody = pushedObject.GetComponent<Rigidbody>();
                CmdDoChargePush();
            }
        }
    }

    [Command]
    void CmdDoPush(GameObject ball)
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
