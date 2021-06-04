using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalControl : Passive
{
    [Header("Script Dependecies")]
    [SerializeField] PlayerMovement movement;

    [Header("Settings")]
    [SerializeField] Sprite passiveIcon;

    private void OnEnable()
    {
        movement.Speed = 16;
        movement.JumpPower = 10;
    }

    public override Sprite ReturnIcon()
    {
        return passiveIcon;
    }
}
