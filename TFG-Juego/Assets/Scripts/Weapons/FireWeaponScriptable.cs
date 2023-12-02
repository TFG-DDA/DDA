using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fire Weapon", menuName = "Weapons/Fire Weapon")]
public class FireWeaponScriptable : WeaponScriptable
{
    // Para armas que tengan varias balas por disparo, el tiempo entre la instanciación de cada bala
    public float timeBetweenBullets;

    // Dispersión
    public float shotAngle;
    public GameObject bulletType;

    // Knockback
    public float knockback;

    // Sonido
    public GUNSOUND sound;

    // Posicion de disparo
    public Vector3 spawnPos;

    // VFX disparo
    public ParticleSystem vfxShoot;

}
