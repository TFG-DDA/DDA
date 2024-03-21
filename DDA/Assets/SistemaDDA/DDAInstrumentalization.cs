using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DDAInstVariables
{
    public string variableName;
    [Tooltip("Modifier that must be set to implement this variable")]
    public DifficultyModifierTypes modifierType;

    [Header("Values of the variable")]
    [Tooltip("Max value until it changes it's difficulty to Mid")]
    public float easyValue;
    [Tooltip("Max value until it changes it's difficulty to Hard")]
    public float midValue;
    [Tooltip("Max value until it changes it's difficulty to Hard")]
    public float hardValue;
}


public class DDAInstrumentalization : MonoBehaviour
{
    public DDAInstVariables[] instVariables;
}
