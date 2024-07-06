using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

#if UNITY_EDITOR
[System.Serializable]
[CustomEditor(typeof(DDAConfig))]
public class DDAConfigEditor : Editor
{
    // Lista de booleanos para controlar si están desplegada o no cada entrada de event variable
    private List<bool> variablesFoldouts = new();
    // Indice para eliminar entradas de event variables
    private int startDiffIndex;
    // Listas para asegurarnos de que cada dificultad solo esta una vez e identificar las repetidas
    private HashSet<string> uniqueDificulties = new();
    private HashSet<string> duplicateDifficulties = new();
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
        EditorGUILayout.LabelField("Difficulties", EditorStyles.boldLabel);
        // Array de dificultades
        SerializedProperty diffConfig = data.FindPropertyRelative("difficultiesConfig");
        EditorGUIUtility.labelWidth = 50;
        buttonWidth = EditorWindow.GetWindow(typeof(DDAEditorWindow)).position.width / 5;
        uniqueDificulties.Clear();
        duplicateDifficulties.Clear();
        string diff;
        for (int i = 0; i < diffConfig.arraySize; i++)
        {
            // Comprobamos que no este repetida
            diff = diffConfig.GetArrayElementAtIndex(i).stringValue;
            if (!uniqueDificulties.Contains(diff))
                uniqueDificulties.Add(diff);
            else
            {
                // Si lo esta la marcamos en rojo
                GUI.color = Color.red;
                duplicateDifficulties.Add(diff);
            }
            EditorGUILayout.BeginHorizontal();
            // Entrada del nombre de la dificultad
            EditorGUILayout.PropertyField(diffConfig.GetArrayElementAtIndex(i), new GUIContent(i.ToString()));
            // Boton de eliminar
            if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
                diffConfig.DeleteArrayElementAtIndex(i);
            // Boton de subir
            if (i > 0 && GUILayout.Button("^", GUILayout.Width(buttonWidth / 4)))
            {
                diffConfig.GetArrayElementAtIndex(i).stringValue = diffConfig.GetArrayElementAtIndex(i - 1).stringValue;
                diffConfig.GetArrayElementAtIndex(i - 1).stringValue = diff;
            }
            // Boton de bajar
            if (i < diffConfig.arraySize - 1 && GUILayout.Button("v", GUILayout.Width(buttonWidth / 4)))
            {
                diffConfig.GetArrayElementAtIndex(i).stringValue = diffConfig.GetArrayElementAtIndex(i + 1).stringValue;
                diffConfig.GetArrayElementAtIndex(i + 1).stringValue = diff;
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
        }
        EditorGUIUtility.labelWidth = 100;

        // Si hay alguna dificultad duplicada se avisa
        if (duplicateDifficulties.Count > 0)
        {
            string labelStart = duplicateDifficulties.Count > 1 ? "The difficulties " : "The difficulty ";
            string labelDiffs = "";
            for (int i = 0; i < duplicateDifficulties.Count; i++)
            {
                labelDiffs += duplicateDifficulties.ElementAt(i);
                if (i < duplicateDifficulties.Count - 2)
                    labelDiffs += ", ";
                else if (duplicateDifficulties.Count != 1 && i < duplicateDifficulties.Count - 1)
                    labelDiffs += " and ";
            }
            string labelEnd = duplicateDifficulties.Count > 1 ? " are duplicate, remove the duplicates " : " is duplicate, remove the duplicate ";
            EditorGUILayout.LabelField(labelStart + labelDiffs + labelEnd + "to avoid implementation issues.");
        }

        // Boton para añadir dificultad
        if (GUILayout.Button("Add difficulty", GUILayout.Width(buttonWidth)))
            diffConfig.InsertArrayElementAtIndex(diffConfig.arraySize);

        // Dificultad por defecto (se elige con popup)
        SerializedProperty startDiff = data.FindPropertyRelative("startDiff");
        // Popup para elegir dificultad inicial
        startDiffIndex = startDiff.intValue;
        startDiffIndex = EditorGUILayout.Popup("Initial difficulty", startDiffIndex, uniqueDificulties.ToArray());
        startDiff.intValue = startDiffIndex;

        // Variables que cambian según la dificultad
        EditorGUILayout.Space();
        SerializedProperty variablesModify = serializedObject.FindProperty("variablesModify");
        // Igualar el tamaño del array de valores de variables al de el array de dificultades
        if (variablesModify.arraySize != uniqueDificulties.Count)
        {
            while (variablesModify.arraySize < uniqueDificulties.Count)
                variablesModify.InsertArrayElementAtIndex(variablesModify.arraySize);
            while (variablesModify.arraySize > uniqueDificulties.Count)
                variablesModify.DeleteArrayElementAtIndex(variablesModify.arraySize - 1);
        }
        EditorGUILayout.LabelField("Variable values for each difficulty", EditorStyles.boldLabel);
        //Entradas para los valores de las variables en cada dificultad
        for (int i = 0; i < variablesModify.arraySize; i++)
            EditorGUILayout.PropertyField(variablesModify.GetArrayElementAtIndex(i), new GUIContent(uniqueDificulties.ElementAt(i)));

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

            // Foldout
            EditorGUILayout.BeginHorizontal();
            variablesFoldouts[i] = EditorGUILayout.Foldout(variablesFoldouts[i], name.stringValue, true);
            // Boton de eliminar
            if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
                eventVariables.DeleteArrayElementAtIndex(i);
            EditorGUILayout.EndHorizontal();
            // Comprobacion de si el foldout está abierto, para mostrar o no el resto de la infos
            if (variablesFoldouts[i])
            {
                // Indentamos para que quede legible
                EditorGUI.indentLevel++;
                // Campo para el nombre del evento
                EditorGUILayout.PropertyField(name);
                // Campo para el peso de la variable en el calculo
                SerializedProperty weight = eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("weight");
                EditorGUILayout.PropertyField(weight);
                if(weight.floatValue <= 0f)
                    EditorGUILayout.LabelField("This variable has a weight of 0 and won't be used for difficulty estimation.");
                    
                // Limites de la variable para cambiar a la siguiente dificultad
                limits = eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("limits");
                // Igualamos el tamaño del array de limites al del de dificultades - 1 (el limite en la mas dificil es el maximo)
                if (limits.arraySize != uniqueDificulties.Count - 1)
                {
                    while (limits.arraySize < uniqueDificulties.Count - 1)
                        limits.InsertArrayElementAtIndex(limits.arraySize);
                    while (limits.arraySize > uniqueDificulties.Count - 1)
                        limits.DeleteArrayElementAtIndex(limits.arraySize - 1);
                }
                EditorGUILayout.LabelField("Limits");
                EditorGUI.indentLevel++;

                float min = eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("minimum").floatValue;
                float max = eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("maximum").floatValue;
                float dir = max - min;
                if (dir == 0)
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Size between minimun an maximum not valid.");
                }
                EditorGUILayout.PropertyField(eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("minimum"));
                // Campo para el valor de cada limite
                for (int j = 0; j < limits.arraySize; j++)
                {
                    EditorGUILayout.LabelField(uniqueDificulties.ElementAt(j));
                    float val = limits.GetArrayElementAtIndex(j).floatValue;
                    float bef, after;
                    if (j == 0)
                        bef = min;
                    else
                        bef = limits.GetArrayElementAtIndex(j - 1).floatValue;
                    if (j == limits.arraySize - 1)
                        after = max;
                    else
                        after = limits.GetArrayElementAtIndex(j + 1).floatValue;

                    if ((dir > 0 && (val <= bef || val >= after)) || (dir < 0 && (val >= bef || val <= after)))
                    {
                        EditorGUILayout.LabelField("Limit not valid. Check the value of the range and the limits next to this.");
                        GUI.color = Color.red;
                    }
                    EditorGUILayout.PropertyField(limits.GetArrayElementAtIndex(j), GUIContent.none);
                    GUI.color = Color.white;
                }
                EditorGUILayout.LabelField(uniqueDificulties.ElementAt(limits.arraySize));
                EditorGUILayout.PropertyField(eventVariables.GetArrayElementAtIndex(i).FindPropertyRelative("maximum"));
                // Campos para los valores minimo y maximo de la variable
                EditorGUI.indentLevel -= 2;
                GUI.color = Color.white;
            }
        }

        EditorGUILayout.Space();
        // Lo anterior se carga los botones mas y menos del array para gestionar las entradas, asi que toca hacer botones personalizados
        // Añadir (al final, el orden da igual)
        if (GUILayout.Button("Add event", GUILayout.Width(buttonWidth)))
            eventVariables.InsertArrayElementAtIndex(eventVariables.arraySize);

        EditorGUILayout.Space();
        // Campo para el evento que provoca el cambio de dificultad
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Event that triggers difficulty change:", EditorStyles.boldLabel);
        SerializedProperty triggerEvent = data.FindPropertyRelative("triggerEvent");
        EditorGUILayout.PropertyField(triggerEvent, GUIContent.none);
        EditorGUILayout.EndHorizontal();
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
