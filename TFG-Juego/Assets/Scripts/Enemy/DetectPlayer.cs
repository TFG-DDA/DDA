using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    int playerLayer = 7;
    float timerSeen = 0, timerShoot = 0;
    bool notSeen = false;
    bool atacking = false;

    float timeKeepSearching;
    float cadenceShoot;

    private void Awake()
    {
        float s = GameManager.instance.GetPlayedLevels() - 2;
        float diff = 100;
        if (s >= 0)
            diff += Mathf.Pow(2.0f, s);

        EnemyAttribs eAt = transform.parent.GetComponent<EnemyAttribs>();
        timeKeepSearching = eAt.timeKeepSearch * (diff / 100);
        FireWeapon fW = transform.parent.GetChild(1).GetChild(0).GetComponent<FireWeapon>();
        cadenceShoot = Mathf.Max(eAt.cadence / (diff / 100), 0.5f) * fW.GetScriptable().cadence * DDA.Instance.config.enemyCadence;
    }

    private void FixedUpdate()
    {
        if (notSeen)
        {
            if (timerSeen < 0f)      //Timer perseguir sin ver
            {
                timerSeen = timeKeepSearching;
                notSeen = false;
                atacking = false;
                GameManager.instance.DynamicGameplayMusic(-1);
                CustomEvent.Trigger(transform.parent.gameObject, "Undetected");
            }
            else
                timerSeen -= Time.deltaTime;
        }
        if (timerShoot < 0f)                                //Timer disparo
        {
            timerShoot = cadenceShoot;
            CustomEvent.Trigger(transform.parent.gameObject, "Shoot");
        }
        else
            timerShoot -= Time.deltaTime;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!atacking && collision.gameObject.layer == playerLayer)          //Player entra en rango
        {
            GameManager.instance.DynamicGameplayMusic(1);
            CustomEvent.Trigger(transform.parent.gameObject, "Detected");
            timerShoot = cadenceShoot;
            notSeen = false;
            atacking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer)          //Player sale de rango
        {
            NotInVision();
        }
    }

    public void NotInVision()
    {
        timerSeen = timeKeepSearching;
        notSeen = true;
    }

    private void OnDestroy()
    {
        if (atacking)
        {
            GameManager.instance.DynamicGameplayMusic(-1);
        }
    }
}
