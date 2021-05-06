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
    [SerializeField] Vector3 force;
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
        //ServerActivateBeam();
        RpcActivateBeam();
    }

    [Server]
    void ServerActivateBeam()
    {
        RpcActivateBeam();
    }

    [ClientRpc]
    void RpcActivateBeam()
    {
        amplifyBeam.SetActive(true);
        amplifyBeam.GetComponent<AmplifyBeamBehaviour>().SetLifeTime(duration);
        amplifyBeam.GetComponent<AmplifyBeamBehaviour>().SetForce(force);
    }

    [Client]
    public void MoveBall(GameObject ball, Vector3 _force)
    {
        if (!hasAuthority)
        {
            return;
        }

        CmdMoveBall(ball, _force);
    }

    [Command]
    public void CmdMoveBall(GameObject ball, Vector3 _force)
    {
        RpcMoveBall(ball, _force);
    }

    [ClientRpc]
    void RpcMoveBall(GameObject ball, Vector3 _force)
    {
        ball.GetComponent<Rigidbody>().AddForce(_force);
    }
}
