using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class GraphPersistence : IPersistence
{
    //Lista con los gráficos añadidos desde editor
    public GraphConfig[] graphs;

    private string baseSaveRoute = "Trazas\\Graphs\\";
    private Dictionary<string, StreamWriter> graphWriters;

    public GraphPersistence(GraphConfig[] g)
    {
        graphs = g;

        graphWriters = new Dictionary<string, StreamWriter>();
        // Crear la carpeta donde se guardaran los archivos que contienen los datos con los puntos de la grafica
        string id = Tracker.Instance.GetSessionId().ToString();
        string fullRoute = baseSaveRoute + id + "\\";
        Directory.CreateDirectory(fullRoute);

        Tuple<int, int>[] offset = new Tuple<int, int>[graphs.Count()];
        for (int i = 0; i < graphs.Count(); ++i)
        {
            graphs[i].Init(i, offset);
            offset[i] = Tuple.Create((int)(graphs[i].data.graph_Width * graphs[i].data.scale), (int)(graphs[i].data.graph_Height * graphs[i].data.scale));
            // Crear el archivo en el que se guardaran los puntos en formato de texto
            graphWriters.Add(graphs[i].data.name, new StreamWriter(fullRoute + graphs[i].data.name + ".csv"));
            graphWriters[graphs[i].data.name].WriteLine(graphs[i].data.eventX + "," + graphs[i].data.eventY);            
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
                graphWriters[graphs[i].data.name].WriteLine(pos.x + "," + pos.y + "," + graphs[i].shownGraph.getLatestObjectivePoint());    
            }
        }
    }

    public override void Flush()
    {
        for (int i = 0; i < graphs.Length; ++i)
        {
            graphWriters[graphs[i].data.name].Flush();
        }
    }

    public override void Release()
    {

    }
}
