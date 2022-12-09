using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum CharacterState
{
    Nothing = 0,
    Idle = 1 << 0,
    Running = 1 << 1,
    Crouching = 1 << 2,
    OnWall = 1 << 3,
    OnHook = 1 << 4,
    OnAir = 1 << 5,
}
