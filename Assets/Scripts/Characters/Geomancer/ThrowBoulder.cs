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
    void Update()
    {
        foreach(LayerMask hitableLayer in hitableLayers)
        {
            Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, hitableLayer);
        }
        
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
        Vector3 forceDirection = mainCamera.transform.forward;
        projectileInstance = Instantiate(boulderPrefab, boulderStartPoint.position, Quaternion.identity);

        CmdBoulderThrow(directionOfBoulder, forceDirection, projectileInstance); //calls command to spawn and add force to the instantiated object
    }

    #endregion

    #region Server
    [Command]
    void CmdBoulderThrow(Vector3 directionOfBoulder, Vector3 direction, GameObject projectileServerSpawn)
    {
        NetworkServer.Spawn(projectileServerSpawn);
        projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
        projectileInstance.GetComponent<Rigidbody>().AddForce(direction * throwForceUpward, ForceMode.Force);
    }

    #endregion
}
