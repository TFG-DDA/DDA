using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    public int amount;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>())
        {
            PlayerWeapon weapon;
            if (collision.transform.childCount > 2 && collision.transform.GetChild(2).gameObject.activeSelf)
                weapon = collision.transform.GetChild(2).GetComponent<PlayerWeapon>();
            else
                weapon = collision.transform.GetChild(1).GetComponent<PlayerWeapon>();

            if (weapon != null)
            {
                RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().PICK_AMMO);
                weapon.AddAmmo(amount);
                Destroy(gameObject);
            }
        }
    }
}
