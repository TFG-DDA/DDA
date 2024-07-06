using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Variables que modifican la dificultad
[Serializable]
public struct DDAVariables
{
    // Escribir aqui las variables que se utilizaran en el juego para modificar la dificultad
    public float enemyDamage;
    public float enemyHealth;
    public float enemySpeed;
    public float enemyCadence;
    public float enemyDrops;

    // Probabilidades de Cartas
    public float Q1Prob;
    public float Q2Prob;

    // Mapas
    public bool hardMap;
}
