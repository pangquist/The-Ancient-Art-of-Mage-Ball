using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Passive : NetworkBehaviour
{
    // This is a base class for all of our passive scripts. The scripts inherits the main functions of every passive, a method
    // that retrieves the abilities icon. The icon is used in our UI system.
    // Author: Valter Lindecrantz

    public abstract Sprite ReturnIcon();
}
