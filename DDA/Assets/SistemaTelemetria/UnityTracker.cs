using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using static GraphConfig;

public enum Constraints { FREE_CONFIG, LEFT_TOP, LEFT_BOTTOM, LEFT_VERTICAL, RIGHT_VERTICAL }
public class UnityTracker : MonoBehaviour
{
    [HideInInspector]
    public Resolution resolution;
    public Vector2 dimension;
    public Constraints constraintsGraphs;
    public float preset_Scale = 1f;
    public int max_charts_per_row = 4;
    public int max_charts_per_col = 3;

    public static UnityTracker instance;

    GameObject canvasObject;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Tracker.Instance.Init(AnalyticsSessionInfo.sessionId);
            if(GetComponent<GraphConfig>() != null)
                InitGraphs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitGraphs()
    {
        // Crear un nuevo objeto Canvas
        canvasObject = new GameObject("GraphCanvas");
        canvasObject.AddComponent<CanvasScaler>();              // Agregar el componente grafico CanvasScaler al objeto Canvas
        canvasObject.AddComponent<GraphicRaycaster>();          // Agregar el componente grafico GraphicRaycaster al objeto Canvas
        canvasObject.transform.SetParent(transform, false);     // Hacer que el objeto Canvas sea hijo del objeto padre
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        canvasObject.GetComponent<Canvas>().worldCamera = Camera.main;
        canvasObject.GetComponent<Canvas>().scaleFactor = 1f;

        // Resolucion
        CanvasScaler cScaler = canvasObject.GetComponent<CanvasScaler>();
        cScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        resolution = Screen.currentResolution;
        cScaler.referenceResolution = new Vector2(resolution.width, resolution.height);
        //Tamano
        dimension = new Vector2(Screen.width, Screen.height);

        Tracker.Instance.InitGraphs(GetComponents<GraphConfig>(), max_charts_per_row, max_charts_per_col);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Eventos correspondientes al juego
        Tracker.Instance.AddTrackableEvent<InicioNivelEvent>(true);
        Tracker.Instance.AddTrackableEvent<FinNivelEvent>(true);
    }

    // Update is called once per frame
    void Update()
    {
        Tracker.Instance.Update();
    }

    private void OnDestroy()
    {
        if(instance == this)
            Tracker.Instance.Release();
    }

    // Ajusta la posicion y escala de la Grafica
    public void SetGraphInWindow(ref GameObject chart, int index, int rowIndex, int colIndex, GraphData config)
    {
        RectTransform rectChart = chart.GetComponent<RectTransform>();
        rectChart.localScale = new Vector3(preset_Scale / max_charts_per_row, preset_Scale / max_charts_per_row, preset_Scale / max_charts_per_row);

        float offsetX = 0;
        float offsetY = 0;
        int row = 0;
        int col = 0;

        switch (constraintsGraphs)
        {
            // HORIZONTAL ABAJO
            case Constraints.LEFT_BOTTOM:
                offsetX = (resolution.width / max_charts_per_row) * rowIndex;
                row = index / max_charts_per_row;
                offsetY = rectChart.anchoredPosition.y + row * (rectChart.rect.height / max_charts_per_row); // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constraints.LEFT_TOP:
                offsetX = (resolution.width / max_charts_per_row) * rowIndex;
                row = index / max_charts_per_row;
                offsetY = resolution.height - ((row + 1) * (rectChart.rect.height / max_charts_per_row)); // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constraints.LEFT_VERTICAL:
                col = index / max_charts_per_col;
                offsetX = (resolution.width / max_charts_per_col) * col;
                offsetY = resolution.height - (1080 / max_charts_per_col) - (resolution.height / max_charts_per_col) * colIndex; // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constraints.RIGHT_VERTICAL:
                col = index / max_charts_per_col;
                offsetX = resolution.width - (rectChart.rect.width / max_charts_per_col) * (col + 1);
                offsetY = resolution.height - (1080 / max_charts_per_col) - (1080 / max_charts_per_col) * colIndex; // el height es el original por eso hay que reescalarlo para abajo
                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constraints.FREE_CONFIG:
                rectChart.anchoredPosition = new Vector2(config.graph_X, config.graph_Y);
                rectChart.localScale = new Vector3(preset_Scale * config.scale, preset_Scale * config.scale, preset_Scale * config.scale);
                break;
        }
    }

    public GameObject GetGraphCanvas()
    {
        return canvasObject;
    }
}
