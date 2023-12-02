using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected float timeSinceShot;

    protected StudioEventEmitter emitter;

    protected void Start()
    {
        timeSinceShot = Time.time;
        emitter = GetComponent<StudioEventEmitter>();
    }

    protected void Update()
    {
        timeSinceShot += Time.deltaTime;
    }

    public abstract void Attack(bool infiniteAmmo);

    public abstract WeaponScriptable GetScriptable();
}
