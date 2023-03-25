using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAdder : MonoBehaviour
{
    [SerializeField] Weapon weapon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string weaponName = weapon.name;

            Weapon[] weapons = other.GetComponentsInChildren<Weapon>(true);

            foreach (Weapon item in weapons)
            {
                if (item.name.Equals(weaponName))
                {
                    other.GetComponent<WeaponController>().AddWeapon(item);
                    item.gameObject.SetActive(false);
                }
            }

            Destroy(weapon.gameObject);
            Destroy(gameObject);
        }
    }
}
