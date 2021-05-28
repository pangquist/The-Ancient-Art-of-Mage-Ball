using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amplify : NetworkBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

    [Header("Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject amplifyBeam;
    [SerializeField] AudioSource soundEffect;

    [Header("Values")]
    [SerializeField] float duration;
    [SerializeField] float force;

    float currentDuration;

    RaycastHit hit;
    GameObject ball;

    //[Client]
    //public void DoAmplifySpell()
    //{
    //    if (!hasAuthority)
    //    {
    //        return;
    //    }

    //    CmdSpawnBeam();

    //}

    [Command]
    void CmdSpawnBeam()
    {
        RpcActivateBeam();
        useAbilities.SetOnCooldown(0);
    }

    [ClientRpc]
    void RpcActivateBeam()
    {
        soundEffect.Play();
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
