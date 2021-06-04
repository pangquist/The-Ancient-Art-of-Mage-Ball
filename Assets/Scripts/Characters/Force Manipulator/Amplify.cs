using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amplify : Ability
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

    [Header("Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject amplifyBeam;
    [SerializeField] AudioSource soundEffect;
    [SerializeField] Sprite abilityIcon;

    [Header("Values")]
    [SerializeField] float cooldown;
    [SerializeField] float duration;
    [SerializeField] float force;

    float currentDuration;

    RaycastHit hit;
    GameObject ball;
    
    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    public override void UseAbility(int abilityIndex)
    {
        if (abilityIndex != 1)
        {
            return;
        }

        CmdSpawnBeam();
    }

    [Command]
    void CmdSpawnBeam()
    {
        RpcActivateBeam();
        useAbilities.SetOnCooldown(0, cooldown);
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

    public override Sprite ReturnIcon()
    {
        return abilityIcon;
    }
}
