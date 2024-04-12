using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    // Máximo de objetos que puede soltar
    int maxDrop;

    // Probabilidad de soltar un objeto o no
    [Range(0f, 1f)]
    float dropProb;
    
    // Probabilidad de que el objeto soltado sea munición (si no es vida)
    [Range(0f, 1f)]
    float ammoProb;

    [SerializeField]
    GameObject ammoDrop;
    [SerializeField]
    GameObject healthDrop;

    float maxDropForce;

    void Awake()
    {
        EnemyAttribs eAt = GetComponent<EnemyAttribs>();
        maxDrop = eAt.maxDrop;
        dropProb = eAt.dropProb * DDA.Instance.config.actVariables.enemyDrops;
        ammoProb = eAt.ammoProb;
        maxDropForce = eAt.maxDropForce;
    }

    public void Drop()
    {
        for(int i = 0; i < maxDrop; i++)
        {
            float prob = Random.Range(0f, 1f);
            if(prob <= dropProb)
            {
                GameObject drop;
                prob = Random.Range(0f, 1f);
                if (prob <= ammoProb)
                    drop = Instantiate(ammoDrop, transform.position, transform.rotation);
                else
                    drop = Instantiate(healthDrop, transform.position, transform.rotation);
                Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
                Vector2 force = new Vector2(Random.Range(-maxDropForce, maxDropForce), Random.Range(-maxDropForce, maxDropForce));
                rb.AddForce(force);
            }
        }
    }
}
