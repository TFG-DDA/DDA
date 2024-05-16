using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using static GraphConfig;

public enum Constraints { FREE_CONFIG, LEFT_TOP, LEFT_BOTTOM, LEFT_VERTICAL_TOP, LEFT_VERTICAL_BOTTOM, RIGHT_VERTICAL_TOP, RIGHT_VERTICAL_BOTTOM }
public class UnityTracker : MonoBehaviour
{
    [HideInInspector]
    public Resolution resolution;
    public Constraints constraintsGraphs;
    public float preset_Scale = 1f;
    public int max_charts_per_row = 4;
    public int max_charts_per_col = 3;

    public GameObject graphObject;

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

        Tracker.Instance.InitGraphs(GetComponents<GraphConfig>());
    }

    // Start is called before the first frame update
    void Start()
    {
        // Eventos correspondientes al juego
        Tracker.Instance.AddTrackableEvent<InicioEvent>(true);
        Tracker.Instance.AddTrackableEvent<FinEvent>(true);

        Tracker.Instance.AddEvent(new InicioEvent());
        Tracker.Instance.AddEvent(new FinEvent());
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyUp(KeyCode.Space))
        //{
        //    Tracker.Instance.AddEvent(new FinEvent());
        //    Tracker.Instance.AddEvent(new InicioEvent());
        //}
        Tracker.Instance.Update();
    }

    private void OnDestroy()
    {
        if(instance == this)
            Tracker.Instance.Release();
    }

    // Ajusta la posicion y escala de la Grafica
    public void SetGraphInWindow(ref GameObject chart, int index, GraphData config, Tuple<int, int>[] offset)
    {
        if(index > max_charts_per_col * max_charts_per_row - 1)
        {
            Debug.LogError("Numero de graficas superior al limite");
            Destroy(chart);
            chart = null;
            return;
        }
        RectTransform rectChart = chart.GetComponent<RectTransform>();
        rectChart.localScale = new Vector3(preset_Scale * config.scale, preset_Scale * config.scale, preset_Scale * config.scale);

        float offsetX = 0;
        float offsetY = 0;
        float actDimX = rectChart.rect.width * rectChart.localScale.x;
        float actDimY = rectChart.rect.height * rectChart.localScale.y;
        int row;
        int col;

        switch (constraintsGraphs)
        {
            // HORIZONTAL ABAJO
            case Constraints.LEFT_BOTTOM:
                row = index / max_charts_per_row;
                col = index % max_charts_per_row;
                //rectChart.rect.height* rectChart.localScale.y;
                for (int i=0; i<col; i++)
                {
                    offsetX += offset[row * max_charts_per_row + i].Item1 * preset_Scale;
                }
                for (int i = 0; i < row; i++)
                {
                    offsetY += offset[col + max_charts_per_row*i].Item2 * preset_Scale;
                }

                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constraints.LEFT_TOP:
                row = index / max_charts_per_row;
                col = index % max_charts_per_row;

                for (int i = 0; i < col; i++)
                {
                    offsetX += offset[row * max_charts_per_row + i].Item1 * preset_Scale;
                }
                for (int i = 0; i < row; i++)
                {
                    offsetY += offset[col + max_charts_per_row * i].Item2 * preset_Scale;
                }

                rectChart.anchoredPosition = new Vector2(offsetX, resolution.height - actDimY - offsetY);
                break;

            case Constraints.LEFT_VERTICAL_TOP:
                col = index / max_charts_per_col;
                row = index % max_charts_per_col;

                for (int i = 0; i < col; i++)
                {
                    offsetX += offset[row + max_charts_per_col * i].Item1 * preset_Scale;
                }
                for (int i = 0; i < row; i++)
                {
                    offsetY += offset[col * max_charts_per_col + i].Item2 * preset_Scale;
                }

                rectChart.anchoredPosition = new Vector2(offsetX, resolution.height - actDimY - offsetY);
                break;

            case Constraints.LEFT_VERTICAL_BOTTOM:
                col = index / max_charts_per_col;
                row = index % max_charts_per_col;

                for (int i = 0; i < col; i++)
                {
                    offsetX += offset[row + max_charts_per_col * i].Item1 * preset_Scale;
                }
                for (int i = 0; i < row; i++)
                {
                    offsetY += offset[col * max_charts_per_col + i].Item2 * preset_Scale;
                }

                rectChart.anchoredPosition = new Vector2(offsetX, offsetY);
                break;

            case Constraints.RIGHT_VERTICAL_TOP:
                col = index / max_charts_per_col;
                row = index % max_charts_per_col;

                for (int i = 0; i < col; i++)
                {
                    offsetX += offset[row + max_charts_per_col * i].Item1 * preset_Scale;
                }
                for (int i = 0; i < row; i++)
                {
                    offsetY += offset[col * max_charts_per_col + i].Item2 * preset_Scale;
                }

                rectChart.anchoredPosition = new Vector2(resolution.width - actDimX - offsetX, resolution.height - actDimY - offsetY);
                break;

            case Constraints.RIGHT_VERTICAL_BOTTOM:
                col = index / max_charts_per_col;
                row = index % max_charts_per_col;

                for (int i = 0; i < col; i++)
                {
                    offsetX += offset[row + max_charts_per_col * i].Item1 * preset_Scale;
                }
                for (int i = 0; i < row; i++)
                {
                    offsetY += offset[col * max_charts_per_col + i].Item2 * preset_Scale;
                }

                rectChart.anchoredPosition = new Vector2(resolution.width - actDimX - offsetX, offsetY);
                break;
            case Constraints.FREE_CONFIG:
                rectChart.anchoredPosition = new Vector2(config.graph_X, config.graph_Y);
                break;
        }
    }

    public GameObject GetGraphCanvas()
    {
        return canvasObject;
    }

    public void SetCanvasCamera(Camera c)
    {
        canvasObject.GetComponent<Canvas>().worldCamera = c;
    }
}
