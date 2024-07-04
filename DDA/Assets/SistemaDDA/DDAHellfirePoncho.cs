using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* DifficultyModifierTypes define las diferentes formas de modificar la dificultad en el DDA,
* se pueden utilizar varias a la vez
* 
*/
[Flags]
public enum DifficultyModifierTypes { DEFAULT = 1 << 0, ENEMIES = 1 << 1, ENVIROMENT = 1 << 2, ALL = ~0 }

public class DDAHellfirePoncho : DDA
{
    [SerializeField]
    DifficultyModifierTypes modifier;
    public override void UpdateDifficulty()
    {
        base.UpdateDifficulty();        
    }
}