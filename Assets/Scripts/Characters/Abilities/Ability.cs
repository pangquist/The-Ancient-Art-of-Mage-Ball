using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : NetworkBehaviour
{
    // This is a base class for all of our ability scripts. The scripts inherits the two main functions of every ability, a method that activates the ability
    // and a method that retrieves the abilities icon. The icon is used in our UI system.
    // Author: Valter Lindecrantz

    public abstract void UseAbility(int abilityIndex);

    public abstract Sprite ReturnIcon();
}
