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
    [Tooltip("Minimum value of the event")]
    public float minimum;
    public float[] limits;
    [Tooltip("MAximum value of the event")]
    public float maximum;
    [Tooltip("Weight of this variable to change the difficulty")]
    [Range(0.0f, 1.0f)]
    public float weight;

}

[Serializable]
public struct DDAData
{
    public DDAVariableData[] variables;//

    public string triggerEvent;//

    public bool enemiesModifierType;//

    public bool playerModifierType;//

    public bool enviromentModifierType;//

    public List<string> difficultiesConfig;//

    [HideInInspector]
    public uint defaultDifficultyLevel;
    public string startDiff;
}

[Serializable]
public struct DDAVariableModificables
{

}

public class DDAConfig : MonoBehaviour
{

    public DDAVariableModificables[] variablesModify;
    [HideInInspector]
    public DDAVariableModificables actVariables;

    public DDAData data;

    private void Awake()
    {
        for (int i = 0; i < data.difficultiesConfig.Count; i++)
        {
            if (data.difficultiesConfig[i] == data.startDiff)
            {
                data.defaultDifficultyLevel = (uint)i;
                return;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DDAConfig))]
public class DDAConfigEditor : Editor
{
    void OnEnable()
    {

    }

    public override void OnInspectorGUI() {
        EditorGUILayout.LabelField("Open Window/DDA Config to configurate");
    }

    public void Window()
    {
        serializedObject.Update();
        SerializedProperty data = serializedObject.FindProperty("data");
        SerializedProperty diffConfig = data.FindPropertyRelative("difficultiesConfig");
        //EditorGUILayout.PropertyField(data);
        EditorGUILayout.PropertyField(diffConfig);

        SerializedProperty startDiff = data.FindPropertyRelative("startDiff");
        EditorGUILayout.PropertyField(startDiff);

        EditorGUILayout.Space(5);
        SerializedProperty variables = data.FindPropertyRelative("variables");
        EditorGUILayout.PropertyField(variables);
        SerializedProperty limits;
        for (int i = 0; i < variables.arraySize; i++)
        {
            limits = variables.GetArrayElementAtIndex(i).FindPropertyRelative("limits");
            if(limits.arraySize != diffConfig.arraySize)
            {
                while (limits.arraySize < diffConfig.arraySize)
                    limits.InsertArrayElementAtIndex(limits.arraySize);
                while(limits.arraySize > diffConfig.arraySize)
                    limits.DeleteArrayElementAtIndex(limits.arraySize - 1);
            }
        }
        SerializedProperty triggerEvent = data.FindPropertyRelative("triggerEvent");
        EditorGUILayout.PropertyField(triggerEvent);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Modifier types");
        SerializedProperty enemiesModifierType = data.FindPropertyRelative("enemiesModifierType");
        EditorGUILayout.PropertyField(enemiesModifierType);
        SerializedProperty playerModifierType = data.FindPropertyRelative("playerModifierType");
        EditorGUILayout.PropertyField(playerModifierType);
        SerializedProperty enviromentModifierType = data.FindPropertyRelative("enviromentModifierType");
        EditorGUILayout.PropertyField(enviromentModifierType);

        //Metodo de checkeo de cambios en el editor
        EditorGUI.BeginChangeCheck();

        //Si ha habido cambios utilizamos setDirty para que unity no cambie los valores de editor y se mantengan para ejecucion
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);

        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();
    }
}
#endif