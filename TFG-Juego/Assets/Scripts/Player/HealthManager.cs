using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class HealthManager : MonoBehaviour
{
    int health;
    int maxHealth;
    bool muelto = false;
    Patroll isEnemy = null;
    [SerializeField]
    GameObject corpse;

    SpriteLibraryAsset sprite;

    private void Awake()
    {
        isEnemy = GetComponent<Patroll>();
        //if (isEnemy)
        //    health = GetComponent<EnemyAttribs>().health;
        //else
        //{
        //    health = GameManager.instance.PLAYER_LIFE;
        //    maxHealth = GameManager.instance.MAX_PLAYER_LIFE;
        //}
    }

    private void Start()
    {
        // Se comprueba si el muerto es enemigo
        if (isEnemy)
        {
            sprite = GetComponent<SpriteLibrary>().spriteLibraryAsset;
            health = GetComponent<EnemyAttribs>().health + GameManager.instance.GetPlayedLevels() / 2;
            health = (int)(health * DDA.Instance.config.enemyHealth);
        }
        else
        {
            health = GameManager.instance.PLAYER_LIFE;
            maxHealth = GameManager.instance.MAX_PLAYER_LIFE;
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (!isEnemy)
        {
            if (!GameManager.instance.isPlayerInvincible())
            {
                health -= damage;
                if (health < 0) health = 0;
                GameManager.instance.setLife(-damage);
                Camera.main.GetComponent<CameraShake>().StartShake();
                GameManager.instance.slowTime();
                GameManager.instance.lostHealth += damage;
                Tracker.Instance.AddEvent(new RecibirDanoEvent(health));
            }
        }
        else
        {
            //Tracker.Instance.AddEvent(new DanoEnemigoEvent());
            health -= damage;
            if (health < 0) health = 0;
            CustomEvent.Trigger(transform.gameObject, "Hitted");
        }
        if (health <= 0)
        {
            Die();
        }
    }

    public void AddHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
        GameManager.instance.setLife(amount);
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        GameManager.instance.setMaxLife(amount, amount);
        AddHealth(amount);
    }

    void Die()  //Este die es provisional para ver si los enemigos funcionan
    {
        // Se comprueba si el muerto es enemigo
        EnemyDrop drop = GetComponent<EnemyDrop>();
        if (drop != null && !muelto)
        {
            muelto = true;
            GameManager.instance.EnemyDie(transform);

            // Cambiar skin del cadaver
            corpse.GetComponent<SpriteLibrary>().spriteLibraryAsset = sprite;

            RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().ENEMY_DEATH, transform.position);

            // Instanciar el cadaver
            if (GameManager.instance.getNumEnemies() > 0)
            {
                drop.Drop();
                Instantiate(corpse, transform.position, Quaternion.identity);
            }
        }
        // Jugador
        else if (drop == null)
        {
            PlayerInstance.instance.GetFireWeapon().EndAttack();
            RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().PLAYER_DEATH_FALL);
            RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().PLAYER_DEATH_GRUNT);

            // Instanciar el cadaver
            Instantiate(corpse, transform.position, Quaternion.identity);
        }


        if (isEnemy)
        {
            // Destruir al enemigo
            Tracker.Instance.AddEvent(new MuerteEnemigoEvent(transform.position));
            Destroy(gameObject);
        }
        else
        {
            GameManager.instance.SetTransitionTime(2.0f);
            GameManager.instance.StartTransition(TransitionTypes.TOMENU);
            Tracker.Instance.AddEvent(new MuerteJugadorEvent(transform.position));
            Tracker.Instance.Flush();
            gameObject.SetActive(false);
        }
    }
}
