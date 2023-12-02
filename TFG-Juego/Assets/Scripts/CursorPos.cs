using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorPos : MonoBehaviour
{
    void Update()
    {
        //Posicion del raton a posicion de escena + Viewport posicion
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 camPos = Camera.main.WorldToViewportPoint(mouse);

        //Comprobacion de si está dentro de la pantalla para mover el puntero
        if (camPos.x > 0.0f && camPos.x < 1.0f && camPos.y > 0.0f && camPos.y < 1.0f)
            transform.position = new Vector3(mouse.x, mouse.y, 0);

        //Colocacion del hijo1 = Objetivo de la cam
        Vector3 parentPos = transform.parent.position;
        Vector3 newPos = new Vector3
        {
            x = (parentPos.x + (parentPos.x + transform.position.x) / 2) / 2,
            y = (parentPos.y + (parentPos.y + transform.position.y) / 2) / 2
        };
        transform.GetChild(0).position = newPos;
        //transform.GetChild(1).LookAt(newPos);
    }

    public Vector2 getMousePos() { return  new Vector2(transform.position.x, transform.position.y); }

}
