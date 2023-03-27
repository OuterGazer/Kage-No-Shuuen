using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Slider lifebar;
    [SerializeField] TextMeshProUGUI[] weaponTexts;

    [SerializeField] Color unselectedColor = Color.white;
    [SerializeField] Color selectedColor = Color.white;

    public void SetMaxLife(float value)
    {
        lifebar.maxValue = value;
        lifebar.value = value;
    }

    public void UpdateLifebar(float value)
    {
        lifebar.value = value;
    }

    public void HideLifebar()
    {
        lifebar.gameObject.SetActive(false);
    }

    public void ShowLifebar()
    {
        lifebar.gameObject.SetActive(true);
    }

    public void AddWeaponToDisplayList(string weapon)
    {
        for(int i = 0; i < weaponTexts.Length; i++)
        {
            if (weaponTexts[i].text.Contains(weapon)) 
            { 
                weaponTexts[i].gameObject.SetActive(true);
                break; 
            }
        }
    }

    public void UpdateWeaponDisplay(int weaponIndex)
    {
        foreach(TextMeshProUGUI item in weaponTexts)
        {
            item.color = unselectedColor;
        }

        weaponTexts[weaponIndex].color = selectedColor;
    }
}
