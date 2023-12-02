using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/HP Max")]
public class Card_HPMax : Card
{
    [SerializeField]
    int addedHP;
    public override void Apply()
    {
        base.Apply();
        PlayerInstance.instance.GetComponent<HealthManager>().AddMaxHealth(addedHP);
    }
}
