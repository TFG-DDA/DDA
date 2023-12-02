using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttribs : MonoBehaviour
{
    public int health;

    [Header("Patroll")]
    public float pVel;
    public float pVelVar;
    public float pRange;
    public float pRangeVar;

    [Header("Atack")]
    public float fVel;
    public float fVelVar;
    public float fRange;
    public float fRangeVar;

    [Header("Drop")]
    public int maxDrop;

    // Probabilidad de soltar un objeto o no
    [Range(0f, 1f)]
    public float dropProb;

    // Probabilidad de que el objeto soltado sea munición (si no es vida)
    [Range(0f, 1f)]
    public float ammoProb;

    public float maxDropForce;

    [Header("Detection")]
    public float timeKeepSearch;
    public float cadence;
}
