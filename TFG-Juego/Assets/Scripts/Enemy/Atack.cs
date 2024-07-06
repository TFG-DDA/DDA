using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Atack : MonoBehaviour
{
    EnemyWeapon weapon;
    GameObject player;
    NavMeshAgent navMesh;

    float followVel;
    float objectiveDistance;

    private void Awake()
    {
        float s = GameManager.instance.GetPlayedLevels() - 2;
        float diff = 100;
        if (s >= 0)
            diff += Mathf.Pow(2.0f, s);

        EnemyAttribs eAt = GetComponent<EnemyAttribs>();
        followVel = (Mathf.Min(eAt.fVel * (diff / 100), eAt.fVel*2f) + Random.Range(-eAt.fVelVar, eAt.fVelVar)) * DDA.instance.config.actVariables.enemySpeed;
        objectiveDistance = eAt.fRange + Random.Range(-eAt.fRangeVar, eAt.fRangeVar);

        int jugaos = GameManager.instance.GetPlayedLevels();
        eAt.health = 10 + (jugaos/2);

        weapon = GetComponentInChildren<EnemyWeapon>();
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.updateRotation = false;
        navMesh.updateUpAxis = false;
    }

    public Vector3 GetObj()
    {
        if(player == null)
        {
            player = PlayerInstance.instance.gameObject;
        }
        return new Vector3(player.transform.position.x, player.transform.position.y, 0);
    }

    public bool ReachedObj()
    {
        return Vector3.Distance(transform.position, player.transform.position) < objectiveDistance;
    }

    public bool ToClose()
    {
        return Vector3.Distance(transform.position, player.transform.position) < objectiveDistance * 0.9f;
    }

    public Vector3 WalkAway()
    {
        Vector2 newPos = (transform.position - player.transform.position).normalized * objectiveDistance;
        newPos += new Vector2(transform.position.x, transform.position.y);

        NavMeshHit hit;
        float angle = 0;

        while(!NavMesh.SamplePosition(newPos, out hit, 0.01f, NavMesh.AllAreas))
        {
            //Debug.Log("Bucle Atack66");
            newPos = Quaternion.AngleAxis(angle, Vector3.forward) * newPos;
            angle += 0.5f;
        }

        return new Vector3(newPos.x, newPos.y, 0);
    }

    public EnemyWeapon GetEnemyWeapon() { return weapon; }
    private void OnEnable()
    {
        navMesh.speed = followVel / 10;
    }
}
