using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MovementVFX : MonoBehaviour
{

    Vector2 nextPosition;
    Vector2 firstPosition;
    Vector2 dir;
    float range = 10f;
    float speed = 3f;
    RectTransform rectTransform;

    float time = 1f;
    void Start()
    {   
        rectTransform = GetComponent<RectTransform>();
        //firstPosition = transform.position;
        firstPosition = rectTransform.anchoredPosition;
        
        calculateNextPosition();
    }

    private void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {

            calculateNextPosition();
            time = Random.Range(2f, 5f);
        }

        rectTransform.Translate(dir * Time.deltaTime * speed);
        Vector3 rot = new Vector3(dir.x * Time.deltaTime * speed, dir.y * Time.deltaTime * speed, 0f);
        rectTransform.Rotate(rot);
    }

    private void calculateNextPosition()
    {

        nextPosition = new Vector2(Random.Range(firstPosition.x + range, firstPosition.x - range), Random.Range(firstPosition.y + range, firstPosition.y - range));
        dir = nextPosition - (Vector2)rectTransform.anchoredPosition;
        dir.Normalize();
    }

}
