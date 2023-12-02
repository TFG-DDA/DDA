using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonBasicAnimation : MonoBehaviour
{
    Animator anim;
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Transform weaponTransform;
    Patroll move;

    int turn_factor = 1;

    Anim_Param_Define param;

    // Esto probablemente lo pasaras desde algun script o desde la maquina de estados, si lo ves necesario pues cambialo
    Transform player;

    Rigidbody2D rb;

    NavMeshAgent navMeshAgent;

    bool ATTACKING = false;

    // Start is called before the first frame update
    void Start()
    {
        param = new Anim_Param_Define();
        anim = GetComponent<Animator>();
        move = GetComponent<Patroll>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = PlayerInstance.instance.transform; //GameManager.instance.getPlayer().transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG //
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    anim_hit();
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    anim_death();
        //}
        // END DEBUG //


        // Diferenciacion ataque y reposo
        // Aqui tendremos que tener alguna manera de acceder al estado del enemigo (ahora pues no se si la hay xd)
        if(ATTACKING)
        {
            // Modificamos el parametro para el animator
            if (!anim.GetBool(param.ATTACK))
            {
                anim.SetBool(param.ATTACK, true);
                //weaponTransform.localScale = new Vector3(1, weaponTransform.localScale.y, weaponTransform.localScale.z);
            }
            anim_run();
        }
        else
        {
            // Modificamos el parametro para el animator
            if (anim.GetBool(param.ATTACK))
            {
                anim.SetBool(param.ATTACK, false);
                //weaponTransform.localScale = new Vector3(1, weaponTransform.localScale.y, weaponTransform.localScale.z);
            }
            anim_walk();
        }
    }

    void anim_walk()
    {
        // Orientacion del sprite
        //Vector2 vel = rb.velocity.normalized;
        Vector2 vel = navMeshAgent.velocity.normalized;
        Vector2 dir = move.getObjetiveDir();

        if (dir.x < 0)
        {
            //transform.GetChild(1).localScale = new Vector3(1, 1, 1);
            spriteRenderer.flipX = true;
            //weaponTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            //transform.GetChild(1).localScale = new Vector3(1, 1, 1);
            spriteRenderer.flipX = false;
            //weaponTransform.localScale = new Vector3(1, 1, 1);
        }

        anim.SetFloat(param.WALKING, vel.magnitude);
    }

    // Estos no hay que llamarlos desde fuera (en principio)
    void anim_run()
    {
        if(player)
        {            
            // Orientacion del sprite en funcion al jugador
            if (player.position.x < transform.position.x)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;

            // Moverse
            //Vector2 vel = rb.velocity.normalized;
            Vector2 vel = navMeshAgent.velocity.normalized;
            // Mueve derecha / jugador derecha // Mueve izquierda / jugador izquierda
            if ((vel.x > 0 && player.position.x < transform.position.x) || (vel.x < 0 && player.position.x > transform.position.x))
                turn_factor = -1; // back
                                  // Mueve izquierda / jugador derecha // Mueve derecha / jugador izquierda
            else if ((vel.x < 0 && player.position.x < transform.position.x) || (vel.x > 0 && player.position.x > transform.position.x))
                turn_factor = 1; // front

            anim.SetFloat(param.RUNNIG, vel.magnitude); // Entrar a correr
            anim.SetFloat(param.SPEED, turn_factor); // Decidir la direccion en la que corres (atras o delante)
        }
    }


    // Estos hay que llamarlos desde fuera
    public void anim_hit()
    {
        anim.SetTrigger(param.HIT);
    }

    public void anim_death()
    {
        anim.SetTrigger(param.DEATH);
    }

    // Para indicar al animator si el enemigo esta atacando o en reposo
    public void setEnemyState(bool a)
    {
        //if(a) transform.GetChild(1).localScale = new Vector3(1, 1, 1);
        ATTACKING = a;
    }
}
