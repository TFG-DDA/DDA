using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponScriptable : ScriptableObject
{
    // Clase base para cada tipo de arma
    public float cadence;
    public int ammoPerAttack;

    // Para armas de fuego, velocidad de las balas, para armas a melee, velocidad de movimiento de los ataques
    public float attackSpeed;

    public int damage;

    public Sprite sprite;
    public Texture2D uiTexture;

    public float fmodParamValue;

    public bool infiniteAmmo;
    public int initialAmmo;
    public int maxAmmo;
    public int ammoMultiplier;

}
