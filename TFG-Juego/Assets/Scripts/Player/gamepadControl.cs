using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamepadControl : MonoBehaviour
{
    private float rotationX;
    private float rotationY;
    private Transform cursorPad;

    private void Start()
    {
        cursorPad = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        float newrotationX = Input.GetAxis("HorizontalRightJoystick");
        float newrotationY = Input.GetAxis("VerticalRightJoystick");
        if(newrotationX != 0 || newrotationY != 0)
        {
            rotationX = newrotationX;
            rotationY = newrotationY;
            transform.rotation = Quaternion.AngleAxis((Mathf.Atan2(rotationY, rotationX) * Mathf.Rad2Deg - 90), Vector3.forward);
        }
    }

    public Vector2 getRotation()
    {
        return new Vector2 (rotationX, rotationY);
    }

    public Vector2 getCursorPadPos() { return cursorPad.position; }
}
