using System;
using System.Collections.Generic;
using UnityEngine;

/*
* El diseñador añadirá los Eventos que determinan la dificultad,
* y los rangos que usará el DDA para determinar la destreza del jugador.
*
* El DDA usará y mezclará todas estas DDAVariableData para calcular una puntuación final,
* que luego categorizará al jugador dentro de los 3 rangos que tenemos.
*/
[Serializable]
public struct DDAVariableData
{
    public string eventName;

    // Valor minimo que debera tener la variable
    [Tooltip("Minimum value of the event")]
    public float minimum;
    // Array que marca los limites de valor de la variable para cambiar de dificultad.
    // Asumira un valor automatico segun el numero de dificultades
    public float[] limits;
    // Valor maximo que podra tener la variable
    [Tooltip("Maximum value of the event")]
    public float maximum;
    // El peso que tiene esta variable en el calculo de la dificultad
    [Tooltip("Weight of this variable to change the difficulty")]
    [Range(0.0f, 1.0f)]
    public float weight;

}

[Serializable]
public struct DDAData
{
    // Array con las distintas variables que marcan a la dificultad
    public DDAVariableData[] eventVariables;

    // Evento que provocara que se recalcule la dificultad
    public string triggerEvent;

    // Lista de dificultades
    public List<string> difficultiesConfig;

    // Dificultad inicial
    public uint startDiff;
}

public class DDAConfig : MonoBehaviour
{
    // Array con los valores de cada variable que modifica la dificultad para las distintas dificultades
    // Asumira un valor automatico segun el numero de dificultades
    public DDAVariables[] variablesModify;
    public DDAVariables actVariables;

    // Estrucutura con la configuracion del DDA
    public DDAData data;
}