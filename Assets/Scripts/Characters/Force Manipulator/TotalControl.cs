using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalControl : Passive
{
    // This script handels the functioning of Force Manipulators passive, Total control.
    // It inherits from the passive class.
    // When the sctipt is enabled it will change the values of the referenced movement script to give Force Manipulator the effect of his passive.
    // Author: Valter Lindecrantz.

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
