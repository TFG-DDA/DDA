using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;

public class GraphPersistence : IPersistence
{
    //Lista con los gráficos añadidos desde editor
    public GraphConfig[] graphs;

    private string baseSaveRoute = "Trazas\\Graphs\\";
    private Dictionary<string, StreamWriter> graphWriters;

    public GraphPersistence(GraphConfig[] g, int maxRow, int maxCol)
    {
        graphs = g;

        graphWriters = new Dictionary<string, StreamWriter>();
        // Crear la carpeta donde se guardaran los archivos que contienen los datos con los puntos de la grafica
        string id = Tracker.Instance.GetSessionId().ToString();
        string fullRoute = baseSaveRoute + id + "\\";
        Directory.CreateDirectory(fullRoute);

        int rowIndex = 0;
        int colIndex = 0;
        for (int i = 0; i < graphs.Count(); ++i)
        {
            graphs[i].Init(rowIndex, colIndex, i);
            // Crear el archivo en el que se guardaran los puntos en formato de texto
            graphWriters.Add(graphs[i].data.name, new StreamWriter(fullRoute + graphs[i].data.name + ".csv"));
            graphWriters[graphs[i].data.name].WriteLine(graphs[i].data.eventX + "," + graphs[i].data.eventY);
            if (rowIndex >= maxRow)
                rowIndex = 0;
            colIndex++;
            if (colIndex >= maxCol)
                colIndex = 0;
        }
    }

    public override void Send(TrackerEvent e)
    {
        for (int i = 0; i < graphs.Length; ++i)
        {
            // Comprueba si la grafica tiene el evento y si debe mostrar un nuevo punto
            if (graphs[i].shownGraph.ReceiveEvent(e))
            {
                // Si muestra un nuevo punto lo escribe en archivo para guardarlo
                Vector2 pos = graphs[i].shownGraph.getLatestPoint();
                // Formato: X (de los dos puntos), Y (del punto de la grafica del jugador), Y (del punto de la grafica del disenador)
                graphWriters[graphs[i].name].WriteLine(pos.x + "," + pos.y + "," + graphs[i].shownGraph.getLatestObjectivePoint());
            }
        }
    }

    public override void Flush()
    {
        for (int i = 0; i < graphs.Length; ++i)
        {
            graphWriters[graphs[i].name].Flush();
        }
    }

    public override void Release()
    {

    }
}
