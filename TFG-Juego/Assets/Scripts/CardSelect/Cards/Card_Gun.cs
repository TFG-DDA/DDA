using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/Gun")]

public class Card_Gun : Card
{
    [SerializeField]
    int weaponIndex;

    public override void Apply()
    {
        base.Apply();
        GameManager.instance.RefillWeapon(weaponIndex);
        PlayerInstance.instance.ChangeWeapon(weaponIndex);
        PlayerInstance.instance.GetComponent<DoubleWeapon>().Swap();
    }
}
