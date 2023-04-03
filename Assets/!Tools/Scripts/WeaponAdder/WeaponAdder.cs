using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAdder : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    [SerializeField] AudioClip equipSound;

    public UnityEvent<string> onWeaponAdded;

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
                    onWeaponAdded.Invoke(item.name);
                }
            }

            AudioSource.PlayClipAtPoint(equipSound, other.transform.position);

            Destroy(weapon.gameObject);
            Destroy(gameObject);
        }
    }
}
