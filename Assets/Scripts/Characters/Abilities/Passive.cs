using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Passive : NetworkBehaviour
{
    public abstract Sprite ReturnIcon();
}
