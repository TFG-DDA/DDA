using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    // Munición restante
    //int ammo = int.MaxValue;
    bool infiniteAmmo = true;

    // Arma actual
    [SerializeField]
    FireWeapon fire;
    //FMODUnity.StudioEventEmitter emitter;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerInstance.instance.transform;
        int rand = UnityEngine.Random.Range(0, 100);

        EnemyGun[] weapons = GameManager.instance.enemyWeapons;
        int c = 0, i = 0;

        while (c <= rand)
        {
            //Debug.Log("Bucle EnemyWeapon29");
            c += weapons[i].probability;
            i++;
        }

        fire.ChangeWeapon((FireWeaponScriptable)weapons[i-1].weapon);
    }

    public void RotateGun()
    {
        Vector2 lookDir = player.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        float zDegree = Mathf.Abs(transform.rotation.eulerAngles.z);
        if (zDegree >= 90.0f && zDegree <= 270.0f)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    public void Shoot()
    {
        if (fire.gameObject.activeSelf && player.gameObject.activeSelf)
            fire.Attack(infiniteAmmo);
    }
}
