using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBoulder : NetworkBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] Transform boulderStartPoint;

    [SerializeField] LayerMask [] hitableLayers;    

    [SerializeField] GameObject boulderPrefab;

    [SerializeField] GameObject hitableObject;
    
    RaycastHit hit;

    GameObject projectileInstance;

    public float throwForceForward;
    public float throwForceUpward;  

    #region Client
    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    void DoBoulderThrow()
    {
        foreach (LayerMask hitableLayer in hitableLayers)
        {
            Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, hitableLayer);
        }
        Vector3 directionOfBoulder;
        if(hit.collider != null)
        {
            directionOfBoulder = hit.point - boulderStartPoint.position;            
        }
        else
        {
            directionOfBoulder =  mainCamera.transform.forward;
        }
        Vector3 forceDirection = mainCamera.transform.forward;

       
        CmdBoulderThrow(directionOfBoulder, boulderStartPoint.position,forceDirection); //calls command to spawn and add force to the instantiated object
        
    }

    #endregion

    #region Server
    [Command]
    void CmdBoulderThrow(Vector3 directionOfBoulder, Vector3 startPos, Vector3 direction)
    {
        //if (NetworkServer.active)
        //{
        //    projectileInstance = Instantiate(boulderPrefab, startPos, Quaternion.identity);

        //    NetworkServer.Spawn(projectileInstance, connectionToClient);
        //projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
        //projectileInstance.GetComponent<Rigidbody>().AddForce(direction * throwForceUpward, ForceMode.Force);
        ClientRpcBoulderThrow(directionOfBoulder, direction);
    }

    [Server]
    void ServerBoulderThrow(Vector3 directionOfBoulder, Vector3 direction)
    {
        ClientRpcBoulderThrow(directionOfBoulder, direction);
    }

    [ClientRpc]
    void ClientRpcBoulderThrow(Vector3 directionOfBoulder, Vector3 direction)
    {
        projectileInstance = Instantiate(boulderPrefab, boulderStartPoint.position, Quaternion.identity);

        NetworkServer.Spawn(projectileInstance);
        projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
        projectileInstance.GetComponent<Rigidbody>().AddForce(direction * throwForceUpward, ForceMode.Force);
    }

    #endregion
}
