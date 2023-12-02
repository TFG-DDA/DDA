using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimation : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    GameObject eye;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    public void open_eye()
    {
        //eye.GetComponent<Animator>().enabled = true;
        //eye.GetComponent<Image>().enabled = true;
        eye.GetComponent<Animator>().SetTrigger("Open");
    }

    public void close_eye()
    {
        eye.GetComponent<Animator>().Rebind();
        eye.GetComponent<Animator>().enabled = false;
        eye.GetComponent<Image>().enabled = false;
    }
}
