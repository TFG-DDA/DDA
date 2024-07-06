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
public enum DifficultyModifierTypes { PLAYER = 1 << 0, ENEMIES = 1 << 1, ENVIROMENT = 1 << 2, ALL = ~0 }

// Ejemplo de implementacion para el cambio de variables segun la dificultad.
// Lo unico obligatorio es sobrecargar el metodo UpdateDifficulty de la clase DDA y cambiar el valor de la variable que se quiera de config.actVariables,
// que sera a la que se acceda en el codigo del juego por el valor de config.variablesModify[currentPlayerDifficult] correspondiente.
public class DDAOverrideExample : DDA
{
    [SerializeField]
    DifficultyModifierTypes modifier;

    public override void UpdateDifficulty()
    {
        base.UpdateDifficulty();

        if (modifier.HasFlag(DifficultyModifierTypes.ALL))
        {
            UpdateAll();
        }
        else
        {
            if (modifier.HasFlag(DifficultyModifierTypes.PLAYER))
                UpdatePlayerDifficulty();
            if (modifier.HasFlag(DifficultyModifierTypes.ENEMIES))
                UpdateEnemiesDifficulty();
            if (modifier.HasFlag(DifficultyModifierTypes.ENVIROMENT))
                UpdateEnvironmentDifficulty();
        }

    }

    void UpdateAll()
    {
        config.actVariables = config.variablesModify[currentPlayerDifficult];
    }

    void UpdatePlayerDifficulty()
    {
        // Actualizar solo variables que afectan al jugador
    }

    void UpdateEnemiesDifficulty()
    {
        // Actualizar solo variables que afectan a los enemigos
    }

    void UpdateEnvironmentDifficulty()
    {
        // Actualizar solo variables que afectan al entorno
    }
}