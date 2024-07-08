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

    // Modificadores ratios drop armas (es un valor que se suma a la probabilidad base de cada nivel, entre los tres tienen que sumar 0 para que las problabilidades sumen 100)
    public int pistolProb;
    public int akProb;
    public int shotgunProb;
}
