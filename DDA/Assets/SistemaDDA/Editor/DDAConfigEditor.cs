using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

#if UNITY_EDITOR
[CustomEditor(typeof(DDAConfig))]
public class DDAConfigEditor : Editor
{
    // Lista de booleanos para controlar si están desplegada o no cada entrada de event variable
    private List<bool> variablesFoldouts = new();
    // Indice para eliminar entradas de event variables
    private int deleteIndex;
    private int startDiffIndex;
    private List<string> startDiffOptions = new();
    private GUIContent diffcultiesLabel = new GUIContent("Difficulties");
    float buttonWidth;
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

        List<string> diff = new List<string>();
        for (int x = 0; x < diffConfig.arraySize; x++)
        {
            SerializedProperty property = diffConfig.GetArrayElementAtIndex(x); // get array element at x
            diff.Add(property.stringValue); // Edit this element's value, in this case limit the float's value to a positive value.
        }
        if (diff.Count != diff.Distinct().Count())
        {
            // Duplicates exist
            GUI.color = Color.red;
        }
        EditorGUILayout.PropertyField(diffConfig, diffcultiesLabel);
        GUI.color = Color.white;

        // Dificultad por defecto (se elige con popup)
        SerializedProperty startDiff = data.FindPropertyRelative("startDiff");
        // Elementos para el popup
        startDiffOptions.Clear();
        for (int i = 0; i < diffConfig.arraySize; i++)
        {
            startDiffOptions.Add(diffConfig.GetArrayElementAtIndex(i).stringValue);
        }
        // Popup para elegir dificultad inicial
        startDiffIndex = EditorGUILayout.Popup("Initial difficulty", startDiffIndex, startDiffOptions.ToArray());
        startDiff.stringValue = startDiffOptions[startDiffIndex];

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
        EditorGUILayout.LabelField("Variable values for each difficulty", EditorStyles.boldLabel);
        //Entradas para los valores de las variables en cada dificultad
        for (int i = 0; i < variablesModify.arraySize; i++)
            EditorGUILayout.PropertyField(variablesModify.GetArrayElementAtIndex(i), new GUIContent(diffConfig.GetArrayElementAtIndex(i).stringValue));

        // Variables de eventos que determinan la dificultad
        EditorGUILayout.Space();
        // Cogemos el array de variables
        SerializedProperty eventVariables = data.FindPropertyRelative("eventVariables");
        EditorGUILayout.LabelField("Events that determine the difficulty", EditorStyles.boldLabel);
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
                if (limits.arraySize != diffConfig.arraySize - 1)
                {
                    while (limits.arraySize < diffConfig.arraySize - 1)
                        limits.InsertArrayElementAtIndex(limits.arraySize);
                    while (limits.arraySize > diffConfig.arraySize - 1)
                        limits.DeleteArrayElementAtIndex(limits.arraySize - 1);
                }
                EditorGUILayout.LabelField("Limits");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("minimum"));
                // Campo para el valor de cada limite
                for (int j = 0; j < limits.arraySize; j++)
                {
                    EditorGUILayout.LabelField(diffConfig.GetArrayElementAtIndex(j).stringValue);
                    EditorGUILayout.PropertyField(limits.GetArrayElementAtIndex(j), GUIContent.none);
                }
                EditorGUILayout.LabelField(diffConfig.GetArrayElementAtIndex(limits.arraySize).stringValue);
                EditorGUILayout.PropertyField(eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("maximum"));
                // Campos para los valores minimo y maximo de la variable
                EditorGUI.indentLevel -= 2;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        // Lo anterior se carga los botones mas y menos del array para gestionar las entradas, asi que toca hacer botones personalizados
        // Añadir (al final, el orden da igual)
        buttonWidth = EditorWindow.GetWindow(typeof(DDAEditorWindow)).position.width / 5;
        if (GUILayout.Button("Add event", GUILayout.Width(buttonWidth)))
            eventVariables.InsertArrayElementAtIndex(eventVariables.arraySize);

        // Eliminar (tiene en cuenta el valor del intField que hay despues para poder elegir que entrada eliminar)
        if (GUILayout.Button("Remove event at:", GUILayout.Width(buttonWidth)) && deleteIndex >= 0 && deleteIndex < eventVariables.arraySize)
            eventVariables.DeleteArrayElementAtIndex(deleteIndex);
        // El intField en cuestion
        deleteIndex = EditorGUILayout.IntField(deleteIndex, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        // Campo para el evento que provoca el cambio de dificultad
        EditorGUILayout.LabelField("Event that triggers difficulty change:", EditorStyles.boldLabel);
        SerializedProperty triggerEvent = data.FindPropertyRelative("triggerEvent");
        EditorGUILayout.PropertyField(triggerEvent, GUIContent.none);

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
