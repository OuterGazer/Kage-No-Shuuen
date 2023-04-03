using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsConstants
{
    public const string SOUND_LEVEL = "Sound Level";
    public const string SFX_LEVEL = "SFX Level";

    public static void SetSoundLevel(float soundLevel)
    {
        PlayerPrefs.SetFloat(SOUND_LEVEL, soundLevel);
    }

    public static void SetSFXLevel(float soundLevel)
    {
        PlayerPrefs.SetFloat(SFX_LEVEL, soundLevel);
    }

    public static float GetSoundLevel()
    {
        return PlayerPrefs.GetFloat(SOUND_LEVEL, 0.5f);
    }

    public static float GetSFXLevel()
    {
        return PlayerPrefs.GetFloat(SFX_LEVEL, 0.3f);
    }
}
