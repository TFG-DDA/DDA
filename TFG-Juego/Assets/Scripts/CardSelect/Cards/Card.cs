using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Card : ScriptableObject
{
    public int id;
    public Sprite backImage;
    public Sprite frontImage;
    public string text;

    public virtual void Apply() { }
}
