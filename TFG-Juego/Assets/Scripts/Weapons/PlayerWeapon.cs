using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    bool infiniteAmmo = false;

    // Arma actual
    [SerializeField]
    FireWeapon fire;
    [SerializeField]
    MeleeWeapon melee;

    // Mirilla
    Transform sightMouse;
    Transform sightGamepad;
    int index = 0;

    bool shooting = false;
    //FMODUnity.StudioEventEmitter emitter;

    // Start is called before the first frame update
    void Start()
    {
        sightMouse = transform.parent.GetChild(0);
        sightGamepad = transform.parent.GetChild(6);
        //ChangeWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.IsPaused())
        {
            Vector2 lookDir;
            float angle;
            if (sightMouse.gameObject.activeSelf)
            {
                lookDir = sightMouse.transform.position - transform.position;
                angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            }
            else
                angle = sightGamepad.transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            float zDegree = math.abs(transform.rotation.eulerAngles.z);
            if (zDegree >= 90.0f && zDegree <= 270.0f)
                transform.localScale = new Vector3(1, -1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }

        if (Input.GetAxis("Fire1") == 1)
        {
            shooting = true;
            if (fire.gameObject.activeSelf)
                fire.Attack(infiniteAmmo);
            else if (melee.gameObject.activeSelf)
                melee.Attack(infiniteAmmo);
        }
        else if (shooting && Input.GetAxis("Fire1") == 0)
        {
            fire.EndAttack();
            shooting = false;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
        {
            index++;// = UnityEngine.Random.Range(0, GameManager.instance.weapons.Length);
            if (index >= GameManager.instance.weapons.Length)
                index = 0;
            ChangeWeapon(index);
            GameManager.instance.RefillWeapon(index);
        }
#endif
    }

    // Cambio de arma (de momento está hecho con un índice en el array de armas del gamemanager igual se puede cambiar)
    public void ChangeWeapon(int index)
    {
        fire.EndAttack();
        if (index > GameManager.instance.weapons.Length || index < 0)
        {
            Debug.Log("La lista no tiene tantas armas crack. Indíce: " + index + " Indíce máximo: " + (GameManager.instance.weapons.Length - 1));
            return;
        }
        WeaponScriptable newWeapon = GameManager.instance.weapons[index];
        Type t = newWeapon.GetType();
        if (t == typeof(FireWeaponScriptable))
        {
            fire.ChangeWeapon((FireWeaponScriptable)newWeapon);
            fire.gameObject.SetActive(true);
            melee.gameObject.SetActive(false);
        }
        else if (t == typeof(MeleeWeaponScriptable))
        {
            melee.ChangeWeapon((MeleeWeaponScriptable)newWeapon);
            fire.gameObject.SetActive(false);
            melee.gameObject.SetActive(true);
        }
        GameManager.instance.setGun(index);
    }

    public void AddAmmo(int amount)
    {
        if (fire.isActiveAndEnabled)
        {
            DoubleWeapon db = GetComponentInParent<DoubleWeapon>();
            bool hasToSwap = false;
            if (db != null)
            {
                hasToSwap = fire.GetScriptable().infiniteAmmo;
                if (hasToSwap)
                {
                    GameObject other = db.GetFirst();
                    FireWeapon fw = other.GetComponentInChildren<FireWeapon>();
                    GameManager.instance.setAmmo(fw.GetScriptable(), amount * fw.GetScriptable().ammoMultiplier, true);
                    db.Swap();
                    return;
                }
            }

            GameManager.instance.setAmmo(fire.GetScriptable(), amount * fire.GetScriptable().ammoMultiplier, true);
        }
    }

    public void DoubleWeaponChange()
    {
        if (fire.gameObject.activeSelf)
            GameManager.instance.setGun(fire.GetComponent<FireWeapon>().GetScriptable());
        else if (melee.gameObject.activeSelf)
            GameManager.instance.setGun(melee.GetComponent<MeleeWeapon>().GetScriptable());
    }

    public WeaponScriptable GetWeapon()
    {
        if (fire.gameObject.activeSelf)
            return fire.GetComponent<FireWeapon>().GetScriptable();
        else if (melee.gameObject.activeSelf)
            return melee.GetComponent<MeleeWeapon>().GetScriptable();
        else return null;
    }
}
