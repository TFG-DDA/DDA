using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    // Arma equipada
    [SerializeField]
    MeleeWeaponScriptable currentWeapon;

    // Objeto que contiene el collider que golpea
    GameObject meleeCollider;

    [SerializeField]
    bool infiniteAmmo = false;

    public void ChangeWeapon(MeleeWeaponScriptable newWeapon)
    {
        currentWeapon = newWeapon;
    }

    public override void Attack(bool infiniteAmmo)
    {
        if (timeSinceShot > currentWeapon.cadence)
        {
            emitter.Play();
        }
    }

    public override WeaponScriptable GetScriptable()
    {
        return currentWeapon;
    }
}
