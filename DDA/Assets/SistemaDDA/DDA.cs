using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class DDA : MonoBehaviour
{
    private static DDA instance = null;
    [HideInInspector]
    public DDAConfig config;
    [HideInInspector]
    public DDAData configData;
    public uint currentPlayerDifficult;
    // Diccionario utilizado por el diseñador para instrumentalizar su código
    public Dictionary<string, float> instVariables;

    private Dictionary<string, DDAVariableData> eventVariables;

    float[] rangeLimits;

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
        if (config == null)
        {
            Debug.LogError("El objeto que contiene el DDA no tiene DDAConfig");
        }
        configData = config.data;

        eventVariables = new Dictionary<string, DDAVariableData>();
        currentPlayerDifficult = configData.defaultDifficultyLevel;

        // Creamos un mapa para comprobar rápidamente si un evento influye en el DDA
        for (int i = 0; i < configData.eventVariables.Length; i++)
        {
            // El totalweight se utilizará para determinar cuanto influye cada variable en el resultado final
            if (configData.eventVariables[i].weight > 0)
            {
                // TODO: Avisar si ha dejado un weight a 0, ya que no se va a usar para calcular la dificultad
                eventVariables.Add(configData.eventVariables[i].eventName, configData.eventVariables[i]);
            }
        }

        // Crea los rangos de dificultades
        InitializeRanges();
        // Aplica la dificultad por defecto
        UpdateDifficulty();
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

            float finalValue = (eventValue - aux.minimum) / (aux.maximum - aux.minimum);
            // Se guardan los valores de todos los eventos para luego decdir la dificultad
            difficultyRange += finalValue * aux.weight;
        }

        // Se lanza la atualización de dificultad cuando llega el evento de trigger dado por el diseñador
        if (eventType == configData.triggerEvent)
        {
            if (currentPlayerDifficult > 0 && difficultyRange < rangeLimits[currentPlayerDifficult - 1]) currentPlayerDifficult--;
            else if(currentPlayerDifficult < rangeLimits.Length && difficultyRange > rangeLimits[currentPlayerDifficult]) currentPlayerDifficult++;
            
            difficultyRange = 0;
            UpdateDifficulty();
        }
    }

    public float getInstVariable(string s)
    {
        if (instVariables.ContainsKey(s))
            return instVariables[s];
        // Printear error de que no se ha encontrado la variable de instrumentalización y devuelve -1
        Debug.LogError("Variable " + s + " no encontrada en lista de variables instrumentalizadas.");
        return -1.0f;
    }

    //Método que aplica los cambios a la dificultad.
    //Añadir los distintos métodos según las flags que defina el diseñador.
    public virtual void UpdateDifficulty()
    {
        Tracker.Instance.AddEvent(new DDAGraphActEvent(currentPlayerDifficult));
    }

    private void InitializeRanges()
    {
        // Suma de los límites de los tramos de cada dificultad
        if (eventVariables.Values.Count <= 0)
        {
            Debug.LogError("Mapa sin ningún evento de control de dificultad.");
            return;
        }
        rangeLimits = new float[eventVariables.ElementAt(0).Value.limits.Length];

        foreach (DDAVariableData a in eventVariables.Values)
        {
            for(int i=0; i<rangeLimits.Length; i++)
            {
                rangeLimits[i] += (a.limits[i] - a.minimum) / (a.maximum - a.minimum) * a.weight;
            }
        }
    }
}
