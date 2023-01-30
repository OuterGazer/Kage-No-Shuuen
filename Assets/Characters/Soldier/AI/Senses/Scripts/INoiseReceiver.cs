using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoiseReceiver
{
    void NotifyNoise(NoiseEmitter noiseEmitter); 
}
