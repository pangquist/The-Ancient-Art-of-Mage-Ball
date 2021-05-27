using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jailer : MonoBehaviour
{
    [SerializeField] PlayerMovement player;

    [SerializeField] float baseSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float speedIncrement;

    [SerializeField] float baseJump;
    [SerializeField] float maxJump;
    [SerializeField] float jumpIncrement;

    [SerializeField] float buffDuration;
    [SerializeField] float maxDuration;

    bool buffActive;

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
}
