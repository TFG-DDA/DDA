using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class DDA : MonoBehaviour
{
    public static DDA instance = null;
    [HideInInspector]
    public DDAConfig config;
    [HideInInspector]
    public DDAData configData;
    public uint currentPlayerDifficult;

    private Dictionary<string, DDAVariableData> eventVariables;

    float[] rangeLimits;

    private float difficultyRange = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        if (!TryGetComponent(out config))
        {
            Debug.LogError("The DDA object does not have a DDAConfig component.");
        }
        configData = config.data;
    }

    public void Start()
    {
        UnityTracker.instance.Init();
        eventVariables = new Dictionary<string, DDAVariableData>();
        currentPlayerDifficult = configData.startDiff;
        config.actVariables = config.variablesModify[currentPlayerDifficult];

        // Creamos un mapa para comprobar rápidamente si un evento influye en el DDA
        for (int i = 0; i < configData.eventVariables.Length; i++)
        {
            // El totalweight se utilizará para determinar cuanto influye cada variable en el resultado final
            if (configData.eventVariables[i].weight > 0)
            {
                eventVariables.Add(configData.eventVariables[i].eventName, configData.eventVariables[i]);
            }
        }

        // Crea los rangos de dificultades
        InitializeRanges();
        // Aplica la dificultad por defecto
        Tracker.Instance.AddEvent(new DDAGraphActEvent(currentPlayerDifficult));
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
            else if (currentPlayerDifficult < rangeLimits.Length && difficultyRange > rangeLimits[currentPlayerDifficult]) currentPlayerDifficult++;

            difficultyRange = 0;
            Tracker.Instance.AddEvent(new DDAGraphActEvent(currentPlayerDifficult));
            UpdateDifficulty();
        }
    }

    //Método que aplica los cambios a la dificultad. 
    public virtual void UpdateDifficulty()
    {
        config.actVariables = config.variablesModify[currentPlayerDifficult];
    }

    private void InitializeRanges()
    {
        // Suma de los límites de los tramos de cada dificultad
        if (eventVariables.Values.Count <= 0)
        {
            Debug.LogError("There's no event to determine the difficulty in config.");
            return;
        }
        rangeLimits = new float[eventVariables.ElementAt(0).Value.limits.Length];

        foreach (DDAVariableData a in eventVariables.Values)
        {
            for (int i = 0; i < rangeLimits.Length; i++)
            {
                rangeLimits[i] += (a.limits[i] - a.minimum) / (a.maximum - a.minimum) * a.weight;
            }
        }
    }
}
