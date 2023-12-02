using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField]
    float speedfactor = 1;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed *= speedfactor;
    }

}
