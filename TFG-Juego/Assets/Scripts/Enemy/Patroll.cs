using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Patroll : MonoBehaviour
{
    NavMeshAgent navMesh;

    Vector2 initPos;
    Vector2 lastPos;
    Vector2 newObjective;
    float angle;

    float vel;
    float range;

    public float getVel() { return vel; }
    // Start is called before the first frame update
    void Awake()
    {        
        EnemyAttribs eAt = GetComponent<EnemyAttribs>();
        vel = eAt.pVel + Random.Range(-eAt.pVelVar, eAt.pVelVar);
        range = eAt.pRange + Random.Range(-eAt.pRangeVar, eAt.pRangeVar);

        navMesh = GetComponent<NavMeshAgent>();
        navMesh.updateRotation = false;
        navMesh.speed = vel / 10;
        navMesh.updateUpAxis = false;

        initPos = lastPos = newObjective = transform.position;
        angle = Random.Range(0f, 360f);
        FindNewDir();
    }

    void FixedUpdate()
    {
        Debug.DrawLine(transform.position, newObjective, Color.red);
    }

    void OnDrawGizmos()     //Debug de range 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastPos, range);
    }

    public void FindNewDir()     //Calcula una nueva posicion para ir y la direccion para llegar
    {
        navMesh.speed = vel / 10;
        Vector2 pos = transform.position;
        if((pos - lastPos).magnitude > range)
        {
            newObjective = lastPos;
            return;
        }

        Vector2 dist = new Vector2(0, Random.Range(range / 10, range / 4));
        Quaternion aux;
        Vector2 a;
        int i = 0;

        do
        {
            angle += Random.Range(-20, 21);
            if (angle > 360) angle -= 360f;
            if (angle < -360) angle += 360f;
            aux = Quaternion.AngleAxis(angle, Vector3.forward);
            a = aux * dist;
            i++;
        }
        while (i<200 && ((newObjective + a - lastPos).magnitude > range || !CanReachArea(newObjective + a)));

        if (i >= 200)
            newObjective = initPos;
        else
        {
            a = aux * dist;
            newObjective += a;
        }
    }

    private bool CanReachArea(Vector2 target)
    {
        NavMeshHit hit;
        
        return (NavMesh.SamplePosition(target, out hit, 0.015f, NavMesh.AllAreas));
    }

    public bool ReachedObj()
    {
        bool end = false;
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), newObjective) < 0.07f)
        {
            newObjective = new Vector2(transform.position.x, transform.position.y);
            end = true;
        }

        return end;
    }

    public Vector2 getDir()
    {
        return newObjective;
    }

    public Vector2 getObjetiveDir()
    {
        Vector2 dir = newObjective - (Vector2)transform.position;
        return dir.normalized;
    }

    public void setObjetiveDir(Vector2 nDir)
    {
        newObjective = nDir;
    }
}
