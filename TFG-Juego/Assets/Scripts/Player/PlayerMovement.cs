using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Fuerza
    [SerializeField]
    float acceleration;

    [SerializeField]
    float dashSpeed = 100;
    [SerializeField]
    float maxDashTime = 0.1f;
    float dashTime = 0f;

    [SerializeField]
    float invincibleTime;

    [SerializeField]
    ParticleSystem dashParticles;


    //Valor l�mite para rb.velocity
    [SerializeField]
    [Tooltip("No hace nada si es mayor a acceleration/minDrag")]
    float speed;


    //Drag para cuando no hay input, minDrag para cuando te mueves
    [SerializeField]
    float drag, minDrag;

    Rigidbody2D rb;
    Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //Input
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();
        if (dashTime >= maxDashTime && Input.GetButtonDown("Dash"))
        {
            if(rb.velocity.magnitude > 0.1f)
                rb.AddForce(direction * dashSpeed);
            else
            {
                if (!PlayerInstance.instance.usingController())
                    direction = PlayerInstance.instance.GetComponentInChildren<CursorPos>().getMousePos() - new Vector2(transform.position.x, transform.position.y);
                else
                    direction = PlayerInstance.instance.GetComponentInChildren<gamepadControl>().getCursorPadPos() - new Vector2(transform.position.x, transform.position.y); 

                direction.Normalize();
                rb.AddForce(direction * dashSpeed * 2);
            }

            Debug.Log("DASH: " + direction);
            Tracker.Instance.AddEvent(new DashEvent());

            dashParticles.Play();
            dashTime = 0;
            GameManager.instance.setIgnoreBullets(invincibleTime);

            RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().DASH);
        }
        dashTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        //Deteccion de levantar tecla para cambiar drag
        if (Mathf.Abs(direction.magnitude) > 0.95f)
            rb.drag = minDrag;
        else
            rb.drag = drag;

        //Aplicacion de fuerza para el movimiento
        if (Mathf.Abs(rb.velocity.magnitude) < speed)
            rb.AddForce(direction * acceleration);
    }

    public Vector2 getDir() { return direction; }

    private void OnDisable()
    {
        direction = Vector2.zero;
    }
}

