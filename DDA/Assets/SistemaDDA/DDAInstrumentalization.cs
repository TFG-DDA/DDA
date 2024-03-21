using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO: INSTRUMENTALIZACIÓN DEL CÓDIGO
 * 
 * Para instrumentalizar el código se deberán cambiar las variables que quiera modificar el jugador,
 * sustituyéndolas por la información que dará el DDA, ejemplo:
 * 
 * (Antes)
 *      enemyHealth = salasPasadas * 10;
 * 
 * (Despúes)
 *      if(DDA.Instance.modifierType.HasFlag(DifficultyModifierTypes.ENEMIES))
 *          enemyHealth = DDA.Instance.variableModificableDDA;
 *      else
 *          enemyHealth = salasPasadas * 10;
 *          
 * !!! ESTO ES UNA IDEA !!!
 *          
 */

[Serializable]
public struct DDAInstVariables
{
    public string variableName;
    [Tooltip("Modifier that must be set to implement this variable")]
    public DifficultyModifierTypes modifierType;

    [Header("Values of the variable")]
    [Tooltip("Value of the variable when in the Easy range")]
    public float easyValue;
    [Tooltip("Value of the variable when in the Mid range")]
    public float midValue;
    [Tooltip("Value of the variable when in the Hard range")]
    public float hardValue;
}


public class DDAInstrumentalization : MonoBehaviour
{
    public DDAInstVariables[] instVariables;

    private void Start()
    {
        // TODO: Ejemplo de instrumentalización
        float health = 0;
        if (DDA.Instance.instVariables.ContainsKey("EnemyHealth"))
            health = DDA.Instance.instVariables["EnemyHealth"];
        else
            health = 10;
    }
}
