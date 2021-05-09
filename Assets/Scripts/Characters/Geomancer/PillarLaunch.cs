using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarLaunch : NetworkBehaviour
{
    [SerializeField] GameObject pillarPrefab;

    [SerializeField] Camera mainCamera;

    [SerializeField] float range;

    [SerializeField] LayerMask[] hitableLayers;

    Transform pillarTop;

    RaycastHit hit;

    GameObject projectileInstance;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    void DoPillarLaunch()
    {
        pillarTop = pillarPrefab.transform.GetChild(0);

        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit);
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
        
        CmdDoPillar(hit.point);
    }

    [Command]
    void CmdDoPillar(Vector3 hitLocation)
    {
        //Vector3 startPositionOffset = new Vector3(0, 10, 0);
        projectileInstance = Instantiate(pillarPrefab, hitLocation - pillarTop.position ,  Quaternion.identity);
        NetworkServer.Spawn(projectileInstance, connectionToClient);
    }
}
