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

    public Vector2 easyRange;

    public Vector2 midRange;

    public Vector2 hardRange;
}

[Serializable]
public struct DDAData
{
    public DDAVariableData[] variables;

    public string triggerEvent;

    public bool EnemiesModifierType;

    public bool PlayerModifierType;

    public bool EnviromentModifierType;

    public PlayerDifficulty defaultDifficulty;
}

public class DDAConfig : MonoBehaviour
{
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
