using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : Bullet
{
    [SerializeField]
    float timeToExplode;

    bool exploding = false;

    [SerializeField]
    GameObject explosionObject;

    [SerializeField]
    int explosionDamage;

    // Update is called once per frame
    protected override void Update()
    {
        if (exploding)
        {
            timeToExplode -= Time.deltaTime;
            if (timeToExplode <= 0)
                Explode();
        }
        else
        {
            timeToDestroy -= Time.deltaTime;
            if (timeToDestroy <= 0)
                Explode();
        }
    }

    void Explode()
    {
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().GRENADE_EXPLOSION, transform.position);
        GameObject e = Instantiate(explosionObject, transform.position, transform.rotation);
        e.GetComponent<Explosion>().setDamage(explosionDamage);
        Destroy(gameObject);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerBullet && collision.gameObject.layer == 9 && !exploding)
        {
            HealthManager health;
            health = collision.gameObject.GetComponent<HealthManager>();

            DemonBasicAnimation enemyAanim = collision.gameObject.GetComponent<DemonBasicAnimation>();
            RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().IMPACT_ENEMY, transform.position);
            enemyAanim.anim_hit();

            health.ReceiveDamage(damage);
        }
        exploding = true;
    }
}
