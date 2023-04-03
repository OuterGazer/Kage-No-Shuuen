using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] AudioClip testSFX;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider.name.Contains("Sound"))
        { slider.value = PlayerPrefsConstants.GetSoundLevel(); }
        else if(slider.name.Contains("SFX"))
        { slider.value = PlayerPrefsConstants.GetSFXLevel(); }
    }

    public void OnSoundChange()
    {
        PlayerPrefsConstants.SetSoundLevel(slider.value);
        SoundManager.Instance.SetVolumeLevel();
    }

    public void OnSFXChange()
    {
        PlayerPrefsConstants.SetSFXLevel(slider.value);
        SoundManager.Instance.SetSFXLevel();
        AudioSource.PlayClipAtPoint(testSFX, Camera.main.transform.position);
    }
}
