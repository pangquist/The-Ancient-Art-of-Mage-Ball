using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBoulder : NetworkBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform boulderStartPoint;
    [SerializeField] LayerMask [] hitableLayers;    
    [SerializeField] GameObject boulderPrefab;
    [SerializeField] GameObject hitableObject;
    [SerializeField] UseAbilities useAbilities;

    RaycastHit hit;

    public float throwForceForward;
    public float throwForceUpward;  

    #region Client
    public override void OnStartClient()
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

        //if (NetworkServer.active)
        //{
        CmdBoulderThrow(directionOfBoulder, boulderStartPoint.position, forceDirection, throwForceForward); //calls command to spawn and add force to the instantiated object
        useAbilities.SetOnCooldownAbility1();
    }

    #endregion

    #region Server
    [Command]
    void CmdBoulderThrow(Vector3 directionOfBoulder, Vector3 startPos, Vector3 direction, float throwForceForward)
    {
        //if (NetworkServer.active)
        //{
        //    projectileInstance = Instantiate(boulderPrefab, startPos, Quaternion.identity);

        //    NetworkServer.Spawn(projectileInstance, connectionToClient);
        //projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
        //projectileInstance.GetComponent<Rigidbody>().AddForce(direction * throwForceUpward, ForceMode.Force);
        GameObject projectileInstance = Instantiate(boulderPrefab, startPos, Quaternion.identity);

        NetworkServer.Spawn(projectileInstance);
        projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
        projectileInstance.GetComponent<Rigidbody>().AddForce(Vector3.up * throwForceUpward, ForceMode.Force);

        //ClientRpcBoulderThrow(directionOfBoulder, startPos, direction);
    }

    //[Server]
    //void ServerBoulderThrow(Vector3 directionOfBoulder, Vector3 direction)
    //{
    //    ClientRpcBoulderThrow(directionOfBoulder, direction);
    //}

    //[ClientRpc]
    //void ClientRpcBoulderThrow(Vector3 directionOfBoulder, Vector3 boulderStartPos, Vector3 direction)
    //{
    //    GameObject projectileInstance = Instantiate(boulderPrefab, boulderStartPos, Quaternion.identity);

    //    NetworkServer.Spawn(projectileInstance);
    //    projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
    //    projectileInstance.GetComponent<Rigidbody>().AddForce(direction * throwForceUpward, ForceMode.Force);
    //}
    //void BoulderSpawn(Vector3 directionOfBoulder, Vector3 boulderStartPos, Vector3 direction)
    //{
    //    GameObject projectileInstance = Instantiate(boulderPrefab, boulderStartPos, Quaternion.identity);

    //    NetworkServer.Spawn(projectileInstance);
    //    projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
    //    projectileInstance.GetComponent<Rigidbody>().AddForce(direction * throwForceUpward, ForceMode.Force);
    //}

    #endregion
}
