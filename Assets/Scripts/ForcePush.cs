using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePush : NetworkBehaviour
{
    [SerializeField]
    Camera mainCamera;
   
    [SerializeField]
    PlayerMovement playerMovement;

    [SerializeField]
    LayerMask hitableLayer;

    [SerializeField]
    GameObject hitableObject;
    RaycastHit hit;

    public float pushAmount;
    public float pushRadius;
    public float forceJumpHeight;
    Rigidbody pushedBody;
    public GameObject hitEffect;

    #region Client

    public override void OnStartAuthority()
    {
        enabled = true;
    }
    
    [Client]
    void Update()
    {
        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, hitableLayer);
    }
    
    void DoPush()
    {
        Collider[] colliders = Physics.OverlapSphere(hit.point, pushRadius);
        CmdSpawnHitEffect(hit.point);

        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                pushedBody = pushedObject.GetComponent<Rigidbody>();
                CmdDoPush(pushedObject.gameObject);
            }
        }
    }
    
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
    }
    [Command]
    void CmdDoChargePush()
    {
        pushedBody.AddExplosionForce(pushAmount * 1.5f, transform.position + transform.forward * 7, pushRadius * 1.5f);
    }

    [Command]
    void CmdDoPush(GameObject _pushedBody)
    {
        Debug.Log("Client is pushing!");
        RpcMoveBall(_pushedBody);
    }

    [ClientRpc]
    void RpcMoveBall(GameObject ball)
    {
        Debug.Log("Server is moving the ball for the clients!");
        ball.GetComponent<Rigidbody>().AddExplosionForce(pushAmount, hit.point, pushRadius);
    }
    #endregion
}
