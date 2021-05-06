using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amplify : NetworkBehaviour
{
    // New amplify!
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject amplifyBeam;
    [SerializeField] float range;
    [SerializeField] float duration;
    float currentDuration;
    
    RaycastHit hit;
    GameObject ball;

    [Client]
    public void DoAmplifySpell()
    {
        if (!hasAuthority)
        {
            return;
        }
        
        CmdSpawnBeam();
    }

    [Command]
    void CmdSpawnBeam()
    {
        amplifyBeam.SetActive(true);
    }
}
