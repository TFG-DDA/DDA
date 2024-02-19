#if !UNITY_WEBGL
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

using Firebase;
using Firebase.Database;

public class ServerPersistenceDesktop : IPersistence
{
    List<TrackerEvent> eventsBuff;

    ServerSerializer serializerServerJSON = null;   //JSON
    string path;
    string formPath;
    string id;

    private int counterEvents = 0;

    DatabaseReference dbRef;


    public ServerPersistenceDesktop()
    {
        eventsBuff = new();
        id = Tracker.Instance.GetSessionId().ToString();

        serializerServerJSON = new ServerSerializer();

        AppOptions options = new AppOptions
        {
            ApiKey = "AIzaSyBM0BocMaLV2Ovc5xYMYEuretiz97szx7M",
            AppId = "1:1056459091112:web:4606e9c5986a206cda669d",
            DatabaseUrl = new System.Uri("https://hellfireponcho-desktop-default-rtdb.europe-west1.firebasedatabase.app"),
            MessageSenderId = "1056459091112",
            ProjectId = "hellfireponcho-desktop",
            StorageBucket = "hellfireponcho-desktop.appspot.com"
        };
        FirebaseApp app = FirebaseApp.Create(options);
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
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
        path = "SessionIDs/" + id + "/Runs/" + GameManager.instance.getPlayedRuns() + "/Levels/" + GameManager.instance.GetPlayedLevels() + "/";
        formPath = "SessionIDs/" + id + "/Forms/" + (GameManager.instance.getPlayedRuns() - 1) + "/";
        foreach (TrackerEvent e in events)
        {
            if (GameManager.instance.GetPlayedLevels() != 1 &&  e.GetEventType() == (typeof(InicioNivelEvent).Name)) counterEvents = 0;

            if (e.GetEventType() == (typeof(FormDataEvent).Name)){
                dbRef.Child(formPath).SetRawJsonValueAsync(serializerServerJSON.Serialize(e));
            }
            else
            {
                dbRef.Child(path + counterEvents.ToString()).SetRawJsonValueAsync(serializerServerJSON.Serialize(e));
                counterEvents++;
            }
        }
    }
}
#endif