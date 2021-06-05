using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amplify : Ability
{
    // This script handels the functioning of Force Manipulators first ability, Amplifying Beam.
    // It inherits from the ability class.
    // It is responsible for spawning the beam for all clients, and telling the ball to move when the beam is in contact with the ball.
    // Author: Valter Lindecrantz.

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

    // Overriden method from Ability that is called from an animation from the character.
    // It calls a command on the server to activate the ability.
    [Client]
    public override void UseAbility(int abilityIndex)
    {
        if (abilityIndex != 1)
        {
            return;
        }

        CmdSpawnBeam();
    }

    // A method that tells the server to activate the beam for all clients and set the ability on cooldown thorugh the useAbility script.
    [Command]
    void CmdSpawnBeam()
    {
        RpcActivateBeam();
        useAbilities.SetOnCooldown(0, cooldown);
    }

    // This method activates the beam for all clients and tells the beam how long it will be active for aswell as how powerful it will be.
    [ClientRpc]
    void RpcActivateBeam()
    {
        soundEffect.Play();
        amplifyBeam.SetActive(true);
        amplifyBeam.GetComponent<AmplifyBeamBehaviour>().SetLifeTime(duration);
        amplifyBeam.GetComponent<AmplifyBeamBehaviour>().SetForce(force);
    }

    // Method that is called from the beam object that sends a command to the server telling the server to apply force to the ball.
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
