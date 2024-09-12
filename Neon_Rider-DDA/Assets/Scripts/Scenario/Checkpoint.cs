using System;
using System.Linq;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int deadVal = 0;
    public GameObject[] pos;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            // Transforma ésta en la posición de respawn del jugador
            // y se encarga de evitar que revivan los enemigos anteriores
            GameManager.instance.checkpoint = transform.position;
            if (deadVal > GameManager.instance.deadVal)
            {
                if(deadVal > 0)
                {
                    Debug.Log("Actualizacion");
                    Tracker.Instance.AddEvent(new LostHealthEvent(GameManager.instance.deads));
                    GameManager.instance.deads = 0;
                    Tracker.Instance.AddEvent(new FinNivelEvent(deadVal, "Level: " + GameManager.instance.actualScene + ", Room: " + deadVal));
                }
                GameManager.instance.deadVal = deadVal;
                Tuple<Vector2, SpawnEnemy.Type>[] posit = new Tuple<Vector2, SpawnEnemy.Type>[pos.Length];
                for (int i = 0; i < pos.Length; i++)
                {
                    posit[i] = Tuple.Create((Vector2)pos[i].transform.position, pos[i].GetComponent<SpawnEnemy>().enemyToSpawn);
                }
                GameManager.instance.posSpawn = posit;
                GameManager.instance.SpawnEnemies(posit, collision.transform, deadVal);
            }
        }
    }
}