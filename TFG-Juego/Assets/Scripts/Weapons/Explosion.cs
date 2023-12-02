using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    float explosionTime;

    float explosionScale;

    [SerializeField]
    float moreDamageDistance;

    [SerializeField]
    int moreDamageMultiplier;

    int damage;

    CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        Camera.main.GetComponent<CameraShake>().StartShake();
    }

    // Update is called once per frame
    void Update()
    {
        explosionTime -= Time.deltaTime;
        //circleCollider.radius += Time.deltaTime * explosionTime;
        float scale = transform.localScale.x + Time.deltaTime * explosionTime;
        transform.localScale = Vector3.one * scale;
        if (explosionTime <= 0)
            Destroy(gameObject);
    }

    public void setDamage(int d) {
        damage = d; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 13)
        {
            HealthManager health;
            health = collision.gameObject.GetComponentInParent<HealthManager>();

            DemonBasicAnimation enemyAanim = collision.gameObject.GetComponentInParent<DemonBasicAnimation>();
            RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().IMPACT_ENEMY, transform.position);
            enemyAanim.anim_hit();

            if (Vector3.Distance(transform.position, collision.transform.position) >= moreDamageDistance)
                health.ReceiveDamage(damage);
            else health.ReceiveDamage(damage * moreDamageMultiplier);
        }
    }
}
