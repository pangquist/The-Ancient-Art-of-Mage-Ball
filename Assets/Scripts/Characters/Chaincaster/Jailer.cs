using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jailer : Passive
{
    private float baseSpeed;
    private float baseJump;
    private bool buffActive;

    [Header("Dependencies")]
    [SerializeField] PlayerMovement player;

    [Header("Settings")]
    [SerializeField] Sprite passiveIcon;

    [Header("Values")]
    [SerializeField] float maxSpeed;
    [SerializeField] float speedIncrement;
    [SerializeField] float maxJump;
    [SerializeField] float jumpIncrement;
    [SerializeField] float buffDuration;
    [SerializeField] float maxDuration;

    private void Start()
    {
        player = gameObject.GetComponent<PlayerMovement>();
        baseSpeed = player.Speed;
        baseJump = player.JumpPower;
        buffActive = false;
    }

    private void Update()
    {
        if (buffActive)
        {
            buffDuration -= Time.deltaTime;
        }

        if(buffDuration <= 0)
        {
            RemoveBonus();
        }
    }

    public void TriggerBuff()
    {
        buffActive = true;

        buffDuration = maxDuration;

        if(player.JumpPower < maxJump)
        {
            player.JumpPower += jumpIncrement;
        }
        if (player.Speed < maxSpeed)
        {
            player.Speed += speedIncrement;
        }
    }

    void RemoveBonus()
    {
        buffActive = false;
        buffDuration = maxDuration;
        player.Speed = baseSpeed;
        player.JumpPower = baseJump;
    }

    public override Sprite ReturnIcon()
    {
        return passiveIcon;
    }
}
