using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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

    [Header("Ranges of the event")]
    [Tooltip("Max value until it changes it's difficulty to Mid")]
    public float easyMax;
    [Tooltip("Max value until it changes it's difficulty to Hard")]
    public float midMax;
    [Tooltip("True if the event should not be restarted when the difficulty updates")]
    public bool persistent;

    [Tooltip("Weight of this variable to change the difficulty")]
    [Range(0.0f,1.0f)]
    public float weight;
}

[Serializable]
public struct DDAData
{
    public DDAVariableData[] variables;

    public string triggerEvent;

    public bool EnemiesModifierType;

    public bool PlayerModifierType;

    public bool EnviromentModifierType;

    public List<PlayerDifficulty> difficultiesConfig;
    
    //public int defaultDifficultyLevel;
    public PlayerDifficulty startDifficulty;
}

public class DDAConfig : MonoBehaviour
{
    [HideInInspector]
    public float enemyDamage;
    [HideInInspector]
    public float enemyHealth;
    [HideInInspector]
    public float enemySpeed;
    [HideInInspector]
    public float enemyCadence;
    [HideInInspector]
    public float enemyDrops;

    public DDAData data;
    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

    }
}

//[CustomEditor(typeof(DDAConfig))]
//public class DDAConfigEditor : Editor
//{
//    SerializedProperty ddaPers;
//    bool show;

//    void OnEnable()
//    {
//        ddaPers = serializedObject.FindProperty("data");
//    }

//    void OnValidate()
//    {

//    }
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        EditorGUILayout.PropertyField(ddaPers);

//        DDAConfig ddaPersistence = (DDAConfig)target;
//        DDAData data = ddaPersistence.data;
//        data.triggerEvent = EditorGUILayout.TextField("Trigger Event", data.triggerEvent);

//        // Agrupa las variables en un foldout
//        show = EditorGUILayout.Foldout(show, "Select Modifiers");
//        if (show)
//        {
//            data.EnemiesModifierType = EditorGUILayout.Toggle("Enemies", data.EnemiesModifierType);
//            data.PlayerModifierType = EditorGUILayout.Toggle("Player", data.PlayerModifierType);
//            data.EnviromentModifierType = EditorGUILayout.Toggle("Enviroment", data.EnviromentModifierType);
//        }

//        ddaPersistence.data = data;


//        //Metodo de checkeo de cambios en el editor
//        EditorGUI.BeginChangeCheck();

//        //Si ha habido cambios utilizamos setDirty para que unity no cambie los valores de editor y se mantengan para ejecucion
//        if (EditorGUI.EndChangeCheck())
//            EditorUtility.SetDirty(target);

//        // Guarda los cambios realizados en el editor
//        serializedObject.ApplyModifiedProperties();
//    }
//}
