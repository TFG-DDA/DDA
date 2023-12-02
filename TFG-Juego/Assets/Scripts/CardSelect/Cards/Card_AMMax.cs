using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/AMMO Max")]
public class Card_AMMax : Card
{
    public override void Apply()
    {
        base.Apply();
        GameManager.instance.maxAmmoWeapons();
        DoubleWeapon db = PlayerInstance.instance.GetComponent<DoubleWeapon>();
        if (db != null)
            db.Swap();
    }
}
