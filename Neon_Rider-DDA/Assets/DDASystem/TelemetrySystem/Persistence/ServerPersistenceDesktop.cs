#if !UNITY_WEBGL
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

//using Firebase;
//using Firebase.Database;

public class ServerPersistenceDesktop : IPersistence
{
    List<TrackerEvent> eventsBuff;

    ServerSerializer serializerServerJSON = null;   //JSON
    string path;
    string formPath;
    string id;

    private int counterEvents = 0;

    //DatabaseReference dbRef;


    public ServerPersistenceDesktop()
    {
        //eventsBuff = new();
        //id = Tracker.Instance.GetSessionId().ToString();

        //serializerServerJSON = new ServerSerializer();

        //AppOptions options = new AppOptions
        //{
        //    // Rellenar con información del servidor de firebase
        //    ApiKey = "ApiKey",
        //    AppId = "AppId",
        //    DatabaseUrl = new System.Uri("DatabaseUrl"),
        //    MessageSenderId = "MessageSenderId",
        //    ProjectId = "ProjectId",
        //    StorageBucket = "StorageBucket"
        //};
        //FirebaseApp app = FirebaseApp.Create(options);
        //dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public override void Release()
    {
        Flush();
    }

    public override void Send(TrackerEvent e)
    {
        eventsBuff.Add(e);
    }

    public override void Flush()
    {
        List<TrackerEvent> events = new List<TrackerEvent>(eventsBuff);
        eventsBuff.Clear();
        Write(events);
    }

    private void Write(List<TrackerEvent> events)
    {
        path = "SessionIDs/" + id /* + ruta específica del juego (opcional)*/;
        formPath = "SessionIDs/" + id /* + ruta específica del juego (opcional)*/;
        foreach (TrackerEvent e in events)
        {
            //dbRef.Child(path + counterEvents.ToString()).SetRawJsonValueAsync(serializerServerJSON.Serialize(e));
            counterEvents++;
        }
    }
}
#endif