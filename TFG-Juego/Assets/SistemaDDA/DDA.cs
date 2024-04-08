using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

/*
* DifficultyModifierTypes define las diferentes formas de modificar la dificultad en el DDA,
* se pueden utilizar varias a la vez
* 
* ENEMIES: Modifica las características de los enemigos
* 
* PLAYER: Modifica las características del player
* 
* ENVIROMENT: Elige entre las escenas que van a aparecer
* 
*/


[Flags]
public enum DifficultyModifierTypes { ENEMIES = 1, PLAYER = 2, ENVIROMENT = 4 }

//TODO: Que lo pueda cambiar el diseñador
//[Serializable]
//public struct PlayerDifficulty
//{
//    // Nombre de la dificultad para diferenciarla
//    public string difficultyName;
//    // Posición de la dificultad (la idea es que cuanto más pequeño más facil)
//    public uint difficultyLevel;
//}

public enum PlayerDifficulty { EASY, MID, HARD }
public class DDA : MonoBehaviour
{
    private static DDA instance = null;
    [HideInInspector]
    public DDAConfig config;
    [HideInInspector]
    public DDAData configData;
    public PlayerDifficulty currentPlayerDifficulty;
    private PlayerDifficulty nextDifficulty;
    // Diccionario utilizado por el diseñador para instrumentalizar su código
    public Dictionary<string, float> instVariables;
    public Dictionary<string, DDAInstVariables> instPrivateVariables;

    private Dictionary<string, DDAVariableData> eventVariables;
    private Dictionary<DDAVariableData, PlayerDifficulty> currentDifficultyValues;

    // Variable usada para implementar el DDA en el código del juego
    public DifficultyModifierTypes modifierType;

    private float totalWeight = 0;
    // Rango entre 0 y 2 que determinará la dificultad
    private float difficultyRange = 0;

    public static DDA Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DDA();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        config = GetComponent<DDAConfig>();
        DDAInstrumentalization ins = GetComponent<DDAInstrumentalization>();
        if (config == null || ins == null)
        {
            Debug.LogError("El objeto que contiene el DDA no tiene DDAConfig o DDAInstrumentalization");
        }
        configData = config.data;

        eventVariables = new Dictionary<string, DDAVariableData>();
        totalWeight = 0;
        currentPlayerDifficulty = configData.startDifficulty;
        nextDifficulty = currentPlayerDifficulty;
        //currentPlayerDifficulty = configData.difficultiesConfig[configData.defaultDifficultyLevel];
        // Creamos un mapa para comprobar rápidamente si un evento influye en el DDA
        for (int i = 0; i < configData.variables.Length; i++)
        {
            // El totalweight se utilizará para determinar cuanto influye cada variable en el resultado final
            if (configData.variables[i].weight > 0)
            {
                // TODO: Avisar si ha dejado un weight a 0, ya que no se va a usar para calcular la dificultad
                totalWeight += configData.variables[i].weight;
                eventVariables.Add(configData.variables[i].eventName, configData.variables[i]);
                //currentDifficultyValues.Add(configData.variables[i], configData.defaultDifficulty);
            }
        }

        // modifierType se utiliza como FLAGS
        if (configData.EnemiesModifierType) modifierType = modifierType | DifficultyModifierTypes.ENEMIES;
        if (configData.PlayerModifierType) modifierType = modifierType | DifficultyModifierTypes.PLAYER;
        if (configData.EnviromentModifierType) modifierType = modifierType | DifficultyModifierTypes.ENVIROMENT;
        UpdateDifficulty();


        // Añadimos las variables de instrumentalización que contengan las flags al diccionario que se usará desde las distintas partes del código
        for (int i = 0; i < ins.instVariables.Length; i++)
        {
            if (modifierType.HasFlag(ins.instVariables[i].modifierType))
            {
                //instPrivateVariables.Add(ins.instVariables[i].variableName, ins.instVariables[i]);
                //switch (configData.defaultDifficulty)
                //{
                //    case PlayerDifficulty.EASY:
                //        instVariables.Add(ins.instVariables[i].variableName, ins.instVariables[i].easyValue);
                //        break;
                //    case PlayerDifficulty.MID:
                //        instVariables.Add(ins.instVariables[i].variableName, ins.instVariables[i].midValue);
                //        break;
                //    case PlayerDifficulty.HARD:
                //        instVariables.Add(ins.instVariables[i].variableName, ins.instVariables[i].hardValue);
                //        break;
                //}
            }
        }

    }
    public void Update()
    {

    }
    public void Release()
    {

    }
    // Recibe todos los eventos del Tracker
    public void Send(DDAEvent e)
    {
        string eventType = e.GetEventType();

        // Se reciben el resto de eventos y se calcula la destreza del jugador
        if (eventVariables.ContainsKey(eventType))
        {
            DDAVariableData aux = eventVariables[eventType];

            float eventValue = e.value;

            if ()
            {
                nextDifficulty = PlayerDifficulty.EASY;
            }
            else if ()
            {
                nextDifficulty = PlayerDifficulty.MID;
                //Cambio a medio
            }
            else if ()
            {
                nextDifficulty = PlayerDifficulty.HARD;
                //Cambio a díficil
            }
        }
        // Se lanza la atualización de dificultad cuando llega el evento de trigger dado por el diseñador
        if (eventType == configData.triggerEvent)
        {
            UpdateDifficulty();
        }
    }

    public float getInstVariable(string s)
    {
        if (instVariables.ContainsKey(s))
            return instVariables[s];
        // Printear error de que no se ha encontrado la variable de instrumentalización y devuelve -1
        return -1.0f;
    }

    private void UpdateDifficulty()
    {
        currentPlayerDifficulty = nextDifficulty;
        // Se actualizan tantas variables como flags estén activas en el modifierType
        if (modifierType.HasFlag(DifficultyModifierTypes.ENEMIES))
        {
            UpdateEnemiesDifficulty();
        }
        if (modifierType.HasFlag(DifficultyModifierTypes.PLAYER))
        {
            UpdatePlayerDifficulty();
        }
        if (modifierType.HasFlag(DifficultyModifierTypes.ENVIROMENT))
        {
            UpdateEnviromentDifficulty();
        }
    }
    private void UpdateEnemiesDifficulty()
    {
        switch (currentPlayerDifficulty)
        {
            case PlayerDifficulty.EASY:
                config.enemyDamage = 0.1f;
                config.enemyHealth = 0.5f;
                config.enemySpeed = 0.25f;
                config.enemyCadence = 10.0f;
                config.enemyDrops = 10.0f;
                break;
            case PlayerDifficulty.MID:
                config.enemyDamage = 1.0f;
                config.enemyHealth = 1.0f;
                config.enemySpeed = 1.0f;
                config.enemyCadence = 1.0f;
                config.enemyDrops = 1.0f;
                break;
            case PlayerDifficulty.HARD:
                config.enemyDamage = 5.0f;
                config.enemyHealth = 5.0f;
                config.enemySpeed = 5.0f;
                config.enemyCadence = 0.1f;
                config.enemyDrops = 0f;
                break;
        }
    }
    private void UpdatePlayerDifficulty()
    {
        switch (currentPlayerDifficulty)
        {
            //case PlayerDifficulty.EASY:

            //    break;
            //case PlayerDifficulty.MID:

            //    break;
            //case PlayerDifficulty.HARD:

            //    break;
        }
    }
    private void UpdateEnviromentDifficulty()
    {
        switch (currentPlayerDifficulty)
        {
            //case PlayerDifficulty.EASY:

            //    break;
            //case PlayerDifficulty.MID:

            //    break;
            //case PlayerDifficulty.HARD:

            //    break;
        }
    }
}
