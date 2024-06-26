using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager.UI;
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

    // Booleanos que marcan si se modifican las variables que modifican la dificultad que afectan a:
    // Default
    public bool defaultModifier;

    // Lista de dificultades
    public List<string> difficultiesConfig;

    // Dificultad inicial
    [HideInInspector]
    public uint defaultDifficultyLevel;
    public string startDiff;
}

// Variables que modifican la dificultad
[Serializable]
public struct DDAVariableModificables
{
    public int example;
    // Rellenar con variables especificas al juego
}

public class DDAConfig : MonoBehaviour
{
    // Array con los valores de cada variable que modifica la dificultad para las distintas dificultades
    // Asumira un valor automatico segun el numero de dificultades
    public DDAVariableModificables[] variablesModify;
    [HideInInspector]
    public DDAVariableModificables actVariables;

    // Estrucutura con la configuracion del DDA
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
    // Lista de booleanos para controlar si están desplegada o no cada entrada de event variable
    private List<bool> variablesFoldouts = new();
    // Indice para eliminar entradas de event variables
    private int deleteIndex;

    // En editor normal, se indica que se debe abrir la ventana par configurar el DDA
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Open Window/DDA Config to configurate");
    }

    // Editor para la ventana
    public void Editor()
    {
        serializedObject.Update();
        // Estructura principal que contiene las distintas variables de configuración
        SerializedProperty data = serializedObject.FindProperty("data");

        // Array de dificultades
        SerializedProperty diffConfig = data.FindPropertyRelative("difficultiesConfig");
        EditorGUILayout.PropertyField(diffConfig);

        // Dificultad por defecto (igual habria que mirar que sea un dropdown segun las entradas de difficultiesConfig)
        SerializedProperty startDiff = data.FindPropertyRelative("startDiff");
        EditorGUILayout.PropertyField(startDiff);

        // Variables que cambian según la dificultad
        EditorGUILayout.Space();
        SerializedProperty variablesModify = serializedObject.FindProperty("variablesModify");
        // Igualar el tamaño del array de valores de variables al de el array de dificultades
        if (variablesModify.arraySize != diffConfig.arraySize)
        {
            while (variablesModify.arraySize < diffConfig.arraySize)
                variablesModify.InsertArrayElementAtIndex(variablesModify.arraySize);
            while (variablesModify.arraySize > diffConfig.arraySize)
                variablesModify.DeleteArrayElementAtIndex(variablesModify.arraySize - 1);
        }
        EditorGUILayout.LabelField("Modifiable variables", EditorStyles.boldLabel);
        //Entradas para los valores de las variables en cada dificultad
        for (int i = 0; i < variablesModify.arraySize; i++)
            EditorGUILayout.PropertyField(variablesModify.GetArrayElementAtIndex(i), new GUIContent(diffConfig.GetArrayElementAtIndex(i).stringValue));
        
        // Variables de eventos que determinan la dificultad
        EditorGUILayout.Space();
        // Cogemos el array de variables
        SerializedProperty eventVariables = data.FindPropertyRelative("eventVariables");
        EditorGUILayout.LabelField("Event variables", EditorStyles.boldLabel);
        SerializedProperty limits, name;
        // Igualar el tamaño del array del array de variables al de foldouts
        if (eventVariables.arraySize != variablesFoldouts.Count)
        {
            while (variablesFoldouts.Count < eventVariables.arraySize)
                variablesFoldouts.Add(false);
            while (variablesFoldouts.Count > eventVariables.arraySize)
                variablesFoldouts.RemoveAt(variablesFoldouts.Count - 1);
        }
        // Para cada entrada del array de variables, un foldout
        for (int i = 0; i < eventVariables.arraySize; i++)
        {
            // Nombre del evento
            name = eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("eventName");
            // Comprobacion de si el foldout está abierto, para mostrar o no el resto de la infos
            variablesFoldouts[i] = EditorGUILayout.Foldout(variablesFoldouts[i], name.stringValue, true);
            if (variablesFoldouts[i])
            {
                // Indentamos para que quede legible
                EditorGUI.indentLevel++;
                // Campo para el nombre del evento
                EditorGUILayout.PropertyField(name);
                // Campo para el peso de la variable en el calculo
                EditorGUILayout.PropertyField(eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("weight"));
                // Limites de la variable para cambiar a la siguiente dificultad
                limits = eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("limits");
                // Igualamos el tamaño del array de limites al del de dificultades - 1 (el limite en la mas dificil es el maximo)
                if (limits.arraySize != diffConfig.arraySize)
                {
                    while (limits.arraySize < diffConfig.arraySize - 1)
                        limits.InsertArrayElementAtIndex(limits.arraySize);
                    while (limits.arraySize > diffConfig.arraySize)
                        limits.DeleteArrayElementAtIndex(limits.arraySize - 1);
                }
                EditorGUILayout.LabelField("Limits");
                EditorGUI.indentLevel++;
                // Campo para el valor de cada limite
                for (int j = 0; j < limits.arraySize; j++)
                    EditorGUILayout.PropertyField(limits.GetArrayElementAtIndex(j), new GUIContent(diffConfig.GetArrayElementAtIndex(j).stringValue));

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Minimum & maximum values");
                EditorGUI.indentLevel++;
                // Campos para los valores minimo y maximo de la variable
                EditorGUILayout.PropertyField(eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("minimum"));
                EditorGUILayout.PropertyField(eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("maximum"));
                EditorGUI.indentLevel -= 2;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        // Lo anterior se carga los botones mas y menos del array para gestionar las entradas, asi que toca hacer botones personalizados
        // Añadir (al final, el orden da igual)
        if (GUILayout.Button("Add event variable", GUILayout.Width(200)))
            eventVariables.InsertArrayElementAtIndex(eventVariables.arraySize);

        // Eliminar (tiene en cuenta el valor del intField que hay despues para poder elegir que entrada eliminar)
        if (GUILayout.Button("Remove event variable at:", GUILayout.Width(200)) && deleteIndex >= 0 && deleteIndex < eventVariables.arraySize)
            eventVariables.DeleteArrayElementAtIndex(deleteIndex);
        // El intField en cuestion
        deleteIndex = EditorGUILayout.IntField(deleteIndex, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();
        // Campo para el evento que provoca el cambio de dificultad (igual habria que mirar que sea un dropdown segun las entradas de eventVariables)
        SerializedProperty triggerEvent = data.FindPropertyRelative("triggerEvent");
        EditorGUILayout.PropertyField(triggerEvent);

        EditorGUILayout.Space();
        // Tipos de modificadores de dificultad
        EditorGUILayout.LabelField("Modifier types");
        SerializedProperty defaultModifierType = data.FindPropertyRelative("defaultModifier");
        EditorGUILayout.PropertyField(defaultModifierType);
        // Añadir aqui las entradas para el resto de modificadores especificos al juego

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