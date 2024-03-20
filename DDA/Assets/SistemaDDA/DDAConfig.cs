﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public struct DDAVariableData
{
    [HideInInspector]
    public string eventName;
    [HideInInspector]
    public Tuple<float> easyRange;
    [HideInInspector]
    public Tuple<float> midRange;
    [HideInInspector]
    public Tuple<float> hardRange;
}

[Serializable]
public struct DDAData
{
    [HideInInspector]
    public List<DDAVariableData> variables;
    [HideInInspector]
    public string triggerEvent;
    [HideInInspector]
    public bool EnemiesModifierType;
    [HideInInspector]
    public bool PlayerModifierType;
    [HideInInspector]
    public bool EnviromentModifierType;
}

public class DDAConfig : MonoBehaviour
{
    [HideInInspector]
    public DDAData data;
    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

    }
}

[CustomEditor(typeof(DDAConfig))]
public class DDAConfigEditor : Editor
{
    SerializedProperty ddaPers;
    bool show;

    void OnEnable()
    {
        ddaPers = serializedObject.FindProperty("data");
    }

    void OnValidate()
    {

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(ddaPers);

        DDAConfig ddaPersistence = (DDAConfig)target;
        DDAData data = ddaPersistence.data;
        data.triggerEvent = EditorGUILayout.TextField("Trigger Event", data.triggerEvent);

        // Agrupa las variables en un foldout
        show = EditorGUILayout.Foldout(show, "Select Modifiers");
        if (show)
        {
            data.EnemiesModifierType = EditorGUILayout.Toggle("Enemies", data.EnemiesModifierType);
            data.PlayerModifierType = EditorGUILayout.Toggle("Player", data.PlayerModifierType);
            data.EnviromentModifierType = EditorGUILayout.Toggle("Enviroment", data.EnviromentModifierType);
        }

        ddaPersistence.data = data;


        //Metodo de checkeo de cambios en el editor
        EditorGUI.BeginChangeCheck();

        //Si ha habido cambios utilizamos setDirty para que unity no cambie los valores de editor y se mantengan para ejecucion
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);

        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();
    }
}
