using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    protected float timeToDestroy;

    [SerializeField]
    protected int damage;

    [SerializeField]
    GameObject VFX_Explosion;

    protected bool playerBullet = false;

    string weapon;

    // Update is called once per frame
    protected virtual void Update()
    {
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0.0f )
            Destroy(gameObject);
    }
    
    public void SetDamage(int d)
    {
        damage = d;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Layer 9 = Enemigo, Layer 7 = Player, Layer 0 = Pared
        if ((playerBullet && collision.gameObject.layer==9) || (!playerBullet && collision.gameObject.layer == 7) || (collision.gameObject.layer == 0))
        {
            HealthManager health;
            if(playerBullet)
                health = collision.gameObject.GetComponent<HealthManager>();
            else
                health = collision.gameObject.GetComponentInParent<HealthManager>();

            // Comprobar si es enemigo o player
            DemonBasicAnimation enemyAanim = collision.gameObject.GetComponent<DemonBasicAnimation>();
            PlayerAnimation playerAnim = collision.gameObject.GetComponent<PlayerAnimation>();
            if (enemyAanim)
            {
                RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().IMPACT_ENEMY, transform.position);
                enemyAanim.anim_hit();
                GameManager.instance.AddHit(weapon);
            }
            else if (playerAnim)
            {
                if(!GameManager.instance.isPlayerInvincible()) {
                    RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().IMPACT_PLAYER);
                    playerAnim.anim_hit();
                }
            }
            else
                RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().IMPACT_ROCK, transform.position);


            if (health != null)
                health.ReceiveDamage(damage);

            // Instanciar el VFX de la explosion
            Instantiate(VFX_Explosion, transform.GetChild(0).position, Quaternion.identity);

            // Destruir la bala
            Destroy(gameObject);
        }
    }

    public void SetPlayer(bool pl) { playerBullet = pl; }

    public void SetWeapon(string s)
    {
        weapon = s;
    }
}
