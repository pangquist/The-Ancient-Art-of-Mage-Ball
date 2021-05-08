using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBoulder : NetworkBehaviour
{
    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    Transform boulderStartPoint;

    [SerializeField]
    PlayerMovement playerMovement;

    [SerializeField]
    LayerMask hitableLayer;

    [SerializeField]
    GameObject boulderPrefab;

    [SerializeField]
    GameObject hitableObject;

    [SerializeField]
    int maxDistance;
    
    RaycastHit hit;
    GameObject projectileInstance;
    public float throwForceForward;
    public float throwForceUpward;

   
    public float pushAmount;
    public float pushRadius;

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

    [Client]
    void DoBoulderThrow()
    {
        Vector3 directionOfBoulder;
        if(hit.collider != null)
        {
            directionOfBoulder = hit.point - boulderStartPoint.position;            
        }
        else
        {
            directionOfBoulder =  mainCamera.transform.forward;
        }
        projectileInstance = Instantiate(boulderPrefab, boulderStartPoint.position, Quaternion.identity);
        NetworkServer.Spawn(projectileInstance, connectionToClient);       

        projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
        projectileInstance.GetComponent<Rigidbody>().AddForce(mainCamera.transform.up * throwForceUpward , ForceMode.Force);

    }

    #endregion

    #region Server

    //[Command]
    //void CmdDoPush(GameObject ball)
    //{
    //    RpcMoveBall(ball);
    //}

    //[ClientRpc]
    //void RpcMoveBall(GameObject ball)
    //{
    //    Debug.Log("Server is moving the ball for the clients!");
    //    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //    ball.GetComponent<Rigidbody>().AddExplosionForce(pushAmount, projectileInstance.transform.position, pushRadius);
    //}
    #endregion
}
