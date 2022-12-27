using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    
    public void ForwardHookHasArrivedAtTarget()
    {
        SendMessageUpwards("HookHasArrivedAtTarget");
    }

    public void ForwardExitDodgingState()
    {
        SendMessageUpwards("ExitDodgingState");
    }
}
