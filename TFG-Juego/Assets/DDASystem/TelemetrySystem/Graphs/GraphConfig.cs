using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
* GraphTypes define las diferentes formas de contar los eventos
* 
* ACCUMULATED: Se acumula la cantidad de veces que aparece el evento Y desde el inicio hasta el numero de veces que haya aparecido X
* 
* NOTACCUMULATED: Cada vez que aparece un evento X, se reinicia la cantidad de veces que ha aparecido el evento Y
* 
* AVERAGE: Cada vez que aparece el evento X se hace una media de las veces que ha aparecido el evento Y desde el inicio
* 
*/
public enum GraphTypes { ACCUMULATED, NOTACCUMULATED, AVERAGE }

/*
 * Scaling define las diferentes formas en las que se escalan las lineas dentro de los charts
 * 
 * X_SCALING_START: La linea ocupa siempre todo el chart en X y con cada evento nuevo se estrecha para insertar el nuevo punto
 * 
 * X_SCALING_OFFSET: La linea empieza de la izquierda y avanza x_segments veces para llegar a ocupar todos los valores de X,
 *      luego se estrecha para insertar nuevos puntos
 *      
 * ONLY_Y: La linea empieza de la izquierda y avanza x_segments veces para llegar a ocupar todos los valores de X,
 *      luego se mueven todos los puntos a la izquierda cuando es necesario insertar uno nuevo,
 *      las distancias entre los puntos son siempre las mismas.
 * 
 */
public enum Scaling { X_SCALING_START, X_SCALING_OFFSET, ONLY_Y }

/*
 * Constraints define de que manera se van a colocar los charts dentro de la pantalla
 * 
 * FREE_CONFIG: Se deja al diseñador que mueva cada chart individualmente y lo escale como quiera,
 *      con las variables graph_X, graph_Y y scale
 *      
 * LEFT_TOP: El primer chart se coloca arriba a la izquierda y los siguientes se situan directamente a la derecha,
 *      cuando no haya espacio en la pantalla para mas se empezara debajo del primero y siguiendo a la derecha
 *      
 * LEFT_BOTTOM: El primer chart se coloca abajo a la izquierda y los siguientes a la derecha, la siguiente fila empieza encima del primero
 * 
 * LEFT_VERTICAL: El primer chart se coloca arriba a la izquierda y los siguientes se situan directamente abajo,
 *      cuando no haya espacio en la pantalla para mas se empezara a la derecha del primero y siguiendo abajo
 *      
 * RIGHT_VERTICAL: El primer chart se coloca arriba a la derecha y los siguientes abajo, la siguiente columna empieza a la izquierda del primero
 * 
 */
[Serializable]
public struct GraphData
{
    [HideInInspector]
    public string name;
    [HideInInspector]
    public bool ddaGraph;
    [HideInInspector]
    public string eventX;
    [HideInInspector]
    public string eventY;
    [HideInInspector]
    public int pointsNumber;
    [HideInInspector]
    public AnimationCurve myCurve;
    [HideInInspector]
    public GameObject window_graph;
    //Config del window_graph
    [HideInInspector]
    public float graph_Height;
    [HideInInspector]
    public float graph_Width;
    //Posicion en X e Y
    [HideInInspector]
    public int graph_X;
    [HideInInspector]
    public int graph_Y;
    [HideInInspector]
    public float scale;
    [HideInInspector]
    public int x_segments; // numero de separaciones que tiene el Eje X (Ademas es el numero de puntos que se representan en la grafica a la vez)
    [HideInInspector]
    public int y_segments; // numero de separaciones que tiene el Eje Y
    [HideInInspector]
    public Scaling scaling;
    [HideInInspector]
    [Range(0.5f, 10.0f)]
    public float line_Width;
    [HideInInspector]
    [Range(0.0f, 1.0f)]
    public float point_Size;
    [HideInInspector]
    public GraphTypes graphType;
    [HideInInspector]
    public Color designerGraphCol;
    [HideInInspector]
    public Color actualGraphCol;
}

public class GraphConfig : MonoBehaviour
{
    GameObject graphObject;

    [HideInInspector]
    public Window_Graph shownGraph;

    [HideInInspector]
    public GraphData data;

    public void Init(int index, Tuple<int, int>[] offset)
    {
        if (data.ddaGraph)
        {
            if (DDA.instance == null) { Debug.LogError("No puedes crear una gráfica de DDA sin una instancia de DDA"); return; };

            data.myCurve = new AnimationCurve();
            data.pointsNumber = 1;
            data.graphType = GraphTypes.NOTACCUMULATED;
            data.scaling = Scaling.X_SCALING_OFFSET;
            data.eventX = "DDAGraphActEvent";
            data.eventY = "GraphDifficEvent";
            data.x_segments = 6;
            data.y_segments = DDA.instance.config.data.difficultiesConfig.Count - 1;
        }

        // Creamos el objeto grafica
        graphObject = transform.GetComponent<UnityTracker>().graphObject;
        GameObject aux = Instantiate(graphObject, parent: UnityTracker.instance.GetGraphCanvas().transform);

        // Rescalamos y posicionamos 
        shownGraph = aux.GetComponent<Window_Graph>();
        shownGraph.name = data.name;
        shownGraph.SetConfig(ref data);
        UnityTracker.instance.SetGraphInWindow(ref aux, index, data, offset);
        if (aux == null) shownGraph = null;        
    }


    private void FixedUpdate()
    {
        if (shownGraph != null)
            shownGraph.RefreshChart();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GraphConfig))]
public class GraphConfigEditor : Editor
{
    SerializedProperty grPers;

    void OnEnable()
    {
        grPers = serializedObject.FindProperty("data");
    }

    void OnValidate()
    {

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(grPers);

        GraphConfig graphPersistence = (GraphConfig)target;

        //Metodo de checkeo de cambios en el editor
        EditorGUI.BeginChangeCheck();

        GraphData actGraphConf = graphPersistence.data;
        actGraphConf.name = EditorGUILayout.TextField("GraphName", actGraphConf.name);
        if (actGraphConf.name == "")
            actGraphConf.name = "NewGraph";

        actGraphConf.ddaGraph = EditorGUILayout.Toggle("Is DDA Graph", actGraphConf.ddaGraph);

        if (!actGraphConf.ddaGraph)
        {
            actGraphConf.pointsNumber = EditorGUILayout.IntField("NumberPoints", actGraphConf.pointsNumber);
            if (actGraphConf.pointsNumber < 1)
                actGraphConf.pointsNumber = 1;

            actGraphConf.myCurve = EditorGUILayout.CurveField("Curve", actGraphConf.myCurve);
            if (actGraphConf.myCurve == null)
            {
                actGraphConf.myCurve = new AnimationCurve();
            }
            EditorGUILayout.Space(5);

            //Variables de escala
            actGraphConf.graphType = (GraphTypes)EditorGUILayout.EnumPopup("GraphType", actGraphConf.graphType);
            actGraphConf.scaling = (Scaling)EditorGUILayout.EnumPopup("Scaling", actGraphConf.scaling);

            // Crea cuadros de texto para los nombres de los eventos
            actGraphConf.eventX = EditorGUILayout.TextField("Event X", actGraphConf.eventX);
            if (actGraphConf.eventX == "")
                actGraphConf.eventX = "Inicio";

            actGraphConf.eventY = EditorGUILayout.TextField("Event Y", actGraphConf.eventY);
            if (actGraphConf.eventY == "")
                actGraphConf.eventY = "Fin";
        }

        //El resto de configuracion
        actGraphConf.line_Width = EditorGUILayout.Slider("Line Width", actGraphConf.line_Width, 0.5f, 10f);
        actGraphConf.point_Size = EditorGUILayout.Slider("Point Size", actGraphConf.point_Size, 0.01f, 1.0f);

        if (graphPersistence.gameObject.GetComponent<UnityTracker>().constraintsGraphs == Constraints.FREE_CONFIG)
        {
            actGraphConf.graph_X = EditorGUILayout.IntSlider("X Pos", actGraphConf.graph_X, 0, Screen.currentResolution.width);
            actGraphConf.graph_Y = EditorGUILayout.IntSlider("Y Pos", actGraphConf.graph_Y, 0, Screen.currentResolution.height);
        }
        actGraphConf.scale = EditorGUILayout.Slider("Scale", actGraphConf.scale, 0.01f, 2.0f);

        if (!actGraphConf.ddaGraph)
        {
            actGraphConf.x_segments = EditorGUILayout.IntField("X segments", actGraphConf.x_segments);
            if (actGraphConf.x_segments < 2)
                actGraphConf.x_segments = 2;
            actGraphConf.y_segments = EditorGUILayout.IntField("Y segments", actGraphConf.y_segments);
            if (actGraphConf.y_segments < 2)
                actGraphConf.y_segments = 2;

            actGraphConf.designerGraphCol = EditorGUILayout.ColorField("DesignerGraph", actGraphConf.designerGraphCol);
            actGraphConf.designerGraphCol.a = 1;
        }

        actGraphConf.actualGraphCol = EditorGUILayout.ColorField("ActualGraph", actGraphConf.actualGraphCol);
        actGraphConf.actualGraphCol.a = 1;

        EditorGUILayout.Space(20);
        graphPersistence.data = actGraphConf;

        //Si ha habido cambios utilizamos setDirty para que unity no cambie los valores de editor y se mantengan para ejecucion
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);

        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
