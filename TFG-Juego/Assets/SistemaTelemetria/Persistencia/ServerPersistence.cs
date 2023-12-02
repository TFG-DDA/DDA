using FirebaseWebGL.Scripts.FirebaseBridge;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ServerPersistence : IPersistence
{
    List<TrackerEvent> eventsBuff;

    ServerSerializer serializerServerJSON = null;   //JSON
    string path;
    string formPath;
    string id;

    private int counterEvents = 0;
    private int counterLevels = 0;
    private int counterRuns = 0;


    public ServerPersistence()
    {
        eventsBuff = new();
        id = Tracker.Instance.GetSessionId().ToString();

        serializerServerJSON = new ServerSerializer();
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
            if (e.GetEventType() == (typeof(FormDataEvent).Name))
            {
                FirebaseDatabase.UpdateJSON(formPath, serializerServerJSON.Serialize(e), "GameManager", "postJSONcallback", "postJSONfallback");
            }
            else
            {
                FirebaseDatabase.UpdateJSON(path + counterEvents.ToString(), serializerServerJSON.Serialize(e), "GameManager", "postJSONcallback", "postJSONfallback");
                counterEvents++;
            }
        }
    }
}
