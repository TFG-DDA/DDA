using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    float duration;
    float timeLeft; 

    [SerializeField]
    float amount;

    public void StartShake()
    {
        timeLeft = duration;
    }

    private void Update()
    {
        if (timeLeft > 0.0f && !GameManager.instance.IsPaused())
        {
            transform.position += Random.insideUnitSphere * amount;

            timeLeft -= Time.deltaTime;
        }
    }
}
