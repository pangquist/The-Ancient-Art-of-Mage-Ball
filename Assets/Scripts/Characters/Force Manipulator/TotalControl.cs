using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalControl : Passive
{
    [Header("Script Dependecies")]
    [SerializeField] PlayerMovement movement;

    private void OnEnable()
    {
        movement.Speed = 16;
        movement.JumpPower = 10;
    }
}
