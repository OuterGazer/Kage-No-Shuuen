using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags] 
public enum CharacterState
{
    StandIdle = 0,
    RunningForward = 1 << 0,
    RunningBackwards = 1 << 1,
    RunningStrafeLeft = 1 << 2,
    RunningStrafeRight = 1 << 3,
    CrouchIdle = 1 << 4,
    CrouchForward = 1 << 5,
    CrouchBackwards = 1 << 6,
    CrouchStrafeLeft = 1 << 7,
    CrouchStrafeRight = 1 << 8,
    OnAir = 1 << 9,
}
