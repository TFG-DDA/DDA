using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public int amount;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().PICK_HP);
            HealthManager health = collision.gameObject.GetComponent<HealthManager>();
            health.AddHealth(amount);
            Destroy(gameObject);
        }
    }
}
