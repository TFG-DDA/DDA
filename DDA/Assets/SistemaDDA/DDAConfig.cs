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
    //[Tooltip("Max value until it changes it's difficulty to Mid")]
    //public float easyMax;
    //[Tooltip("Max value until it changes it's difficulty to Hard")]
    //public float midMax;
    public float[] limits;
    [Tooltip("MAximum value of the event")]
    public float maximum;
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

    public List<string> difficultiesConfig;

    [HideInInspector]
    public uint defaultDifficultyLevel;
    public string startDiff; 
}

[Serializable]
public struct DDAVariableModificables
{
    public float enemyDamage;
    public float enemyHealth;
    public float enemySpeed;
    public float enemyCadence;
    public float enemyDrops;
} 

public class DDAConfig : MonoBehaviour
{
    
    public DDAVariableModificables[] variablesModify;
    [HideInInspector]
    public DDAVariableModificables actVariables;

    public DDAData data;

    private void Awake()
    {
        for(int i=0; i<data.difficultiesConfig.Count; i++)
        {
            if (data.difficultiesConfig[i] == data.startDiff)
            {
                data.defaultDifficultyLevel = (uint)i;
                return;
            }
        }
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
