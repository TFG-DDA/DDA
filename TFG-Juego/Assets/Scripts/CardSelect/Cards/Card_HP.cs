using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/HP")]
public class Card_HP : Card
{
    [SerializeField]
    int addedHP;
    public override void Apply()
    {
        base.Apply();
        PlayerInstance.instance.GetComponent<HealthManager>().AddHealth(addedHP);
    }
}
