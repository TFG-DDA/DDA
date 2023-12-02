using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : Weapon
{
    // Arma equipada
    [SerializeField]
    FireWeaponScriptable currentWeapon;

    // Objeto vac�o que determina la posici�n donde se instancian las balas
    Transform spawner;

    // Sistema de particulas para disparos
    ParticleSystem vfxShoot;

    [SerializeField]
    bool IsPlayer = false;

    Rigidbody2D rb;
    PlayerMovement pm;

    private FMOD.Studio.EventInstance instance;
    bool instPlaying = false;

    private void Start()
    {
        base.Start();
        spawner = transform.GetChild(0);
        vfxShoot = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
        //if(GetComponent<PlayerInstance>())
        //{
        //    Debug.Log("SOY EL MAN");
        //}
        instance = FMODUnity.RuntimeManager.CreateInstance(GameManager.instance.GetSoundResources().GUN_FLAMETHROWER);
    }

    private void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }

    public void ChangeWeapon(FireWeaponScriptable newWeapon)
    {
        currentWeapon = newWeapon;
        GetComponent<SpriteRenderer>().sprite = newWeapon.sprite;
        transform.GetChild(0).localPosition = newWeapon.spawnPos;
        transform.GetChild(1).localPosition = newWeapon.spawnPos;
    }

    public override void Attack(bool infiniteAmmo)
    {
        if (timeSinceShot > currentWeapon.cadence && !GameManager.instance.IsPaused())
        {
            if (IsPlayer)
            {
                if (!currentWeapon.infiniteAmmo)
                    GameManager.instance.AddShot();
                else
                    GameManager.instance.AddInfiniteShot();
            }

            int ammo = -1;
            if (!infiniteAmmo && !currentWeapon.infiniteAmmo)
                ammo = GameManager.instance.getAmmo(currentWeapon);
            timeSinceShot = 0;
            int shots = 0;
            while (shots < currentWeapon.ammoPerAttack && (ammo > 0 || infiniteAmmo || currentWeapon.infiniteAmmo))
            {
                //Debug.Log("Bucle FireWeapon46");
                //Debug.Log("Bucle FireWeapon46");
                //float angle = Random.Range(-currentWeapon.shotAngle, currentWeapon.shotAngle);
                //Vector3 spread = new Vector3(0, 0, angle);
                //GameObject bullet = Instantiate(currentWeapon.bulletType, spawner.position, Quaternion.Euler(spawner.rotation.eulerAngles + spread));
                ////bullet.transform.Rotate(new Vector3(0, 0, angle));
                //bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.right * currentWeapon.attackSpeed, ForceMode2D.Impulse);
                //bullet.GetComponent<Bullet>().SetDamage(currentWeapon.damage);
                //bullet.GetComponent<Bullet>().SetPlayer(transform.parent.parent.gameObject.layer == 7);
                StartCoroutine(SpawnBullet(shots * currentWeapon.timeBetweenBullets));
                shots++;

                if (IsPlayer)
                {
                    Camera.main.GetComponent<CameraShake>().StartShake();
                    if (currentWeapon.name == "Lanzallamas")
                    {
                        if (!instPlaying)
                        {
                            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                            instance.setParameterByName("FlameThrowerEnd", 0.0f);
                            instance.start();
                            instPlaying = true;
                        }
                    }
                    else
                        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().gunSound(currentWeapon.sound), transform.position);

                    // Activar VFX de disparo
                    vfxShoot.Play();
                }
                else
                {
                    RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().GUN_ENEMY_REVOLVER, transform.position);
                }


                if (!infiniteAmmo && !currentWeapon.infiniteAmmo)
                {
                    ammo--;
                    GameManager.instance.setAmmo(currentWeapon, -1, false);
                }
            }
            if (!infiniteAmmo && ammo <= 0)
            {
                DoubleWeapon db = GetComponentInParent<DoubleWeapon>();
                if (db != null && !currentWeapon.infiniteAmmo)
                {
                    EndAttack();
                    db.Infinite();
                }
            }
            if (!infiniteAmmo)
            {
                Vector2 dir;
                if (!PlayerInstance.instance.usingController())
                    dir = PlayerInstance.instance.GetComponentInChildren<CursorPos>().getMousePos() - new Vector2(transform.position.x, transform.position.y);
                else
                    dir = PlayerInstance.instance.GetComponentInChildren<gamepadControl>().getCursorPadPos() - new Vector2(transform.position.x, transform.position.y);

                GetComponentInParent<Rigidbody2D>().AddForce(-dir.normalized * currentWeapon.knockback);
            }
        }
    }

    public void EndAttack()
    {
        if (currentWeapon.name == "Lanzallamas")
        {
            instance.setParameterByName("FlameThrowerEnd", 1.0f);
            instPlaying = false;
        }
    }

    IEnumerator SpawnBullet(float delay)
    {
        yield return new WaitForSeconds(delay);
        float angle = Random.Range(-currentWeapon.shotAngle, currentWeapon.shotAngle);
        Vector3 spread = new Vector3(0, 0, angle);
        //if (currentWeapon.timeBetweenBullets > 0.0f)
        //    RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().gunSound(currentWeapon.sound), transform.position);
        GameObject bullet = Instantiate(currentWeapon.bulletType, spawner.position, Quaternion.Euler(spawner.rotation.eulerAngles + spread));
        //bullet.transform.Rotate(new Vector3(0, 0, angle));
        bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.right * currentWeapon.attackSpeed, ForceMode2D.Impulse);
        bullet.GetComponent<Bullet>().SetDamage(currentWeapon.damage);
        bullet.GetComponent<Bullet>().SetPlayer(transform.parent.parent.gameObject.layer == 7);
        bullet.GetComponent<Bullet>().SetWeapon(currentWeapon.name);
    }
    public override WeaponScriptable GetScriptable()
    {
        return currentWeapon;
    }
}
