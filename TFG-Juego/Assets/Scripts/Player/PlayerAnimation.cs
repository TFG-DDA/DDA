using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Para poner aqui las referencias de los nombres de los parametros 
// que usa el animator
public class Anim_Param_Define
{
    public string SPEED = "speed";
    public string RUNNIG = "running";
    public string DEATH = "dead";
    public string HIT = "hit";
    public string WALKING = "walking";
    public string ATTACK = "attack";
}

public class PlayerAnimation : MonoBehaviour
{
    Animator anim;
    SpriteRenderer spriteRenderer;
    PlayerMovement move;

    [SerializeField]
    Transform cursor;
    [SerializeField]
    Transform pad;
    int turn_factor = 1;

    Anim_Param_Define param;

    // Start is called before the first frame update
    void Start()
    {
        param = new Anim_Param_Define();
        anim = GetComponent<Animator>();
        move = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movimiento
        anim_move();

    }

    void anim_move()
    {
        // Orientacion del sprite
        if (GameManager.instance.getConnected())
        {
            if (pad.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
                turn_factor = 1;
            }
            else
            {
                spriteRenderer.flipX = false;
                turn_factor = -1;
            }
        }
        else
        {
            if (cursor.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
                turn_factor = 1;
            }
            else
            {
                spriteRenderer.flipX = false;
                turn_factor = -1;
            }
        }

        //if (cursor.position.x < transform.position.x)
        //{
        //    spriteRenderer.flipX = true;
        //    turn_factor = 1;
        //}
        //else
        //{
        //    spriteRenderer.flipX = false;
        //    turn_factor = -1;
        //}


        // Moverse
        Vector2 dir = move.getDir();
        //Debug.Log(dir);
        anim.SetFloat(param.RUNNIG, dir.magnitude); // Entrar a correr
        anim.SetFloat(param.SPEED, dir.x * turn_factor); // Decidir la direccion en la que corres (atras o delante)
    }

    public void anim_hit()
    {
        anim.SetTrigger(param.HIT);
    }

    public void anim_death()
    {
        anim.SetTrigger(param.DEATH);
    }

    public void footstepR()
    {
        //RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().PLAYER_RIGHT);
    }
    public void footstepL()
    {
        //RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().PLAYER_LEFT);
    }
}