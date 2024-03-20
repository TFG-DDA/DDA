using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Tracker
{
    private static Tracker instance = null;
    long sessionId;

    //Espaciado entre posts
    float timeBetweenPosts;
    float tSinceLastPost = 0;

    FilePersistence filePersistence;
    bool filePers = false;
    ServerPersistence serverPersistence;
    bool serverPers = false;
#if !UNITY_WEBGL
    ServerPersistenceDesktop serverPersistenceDesktop;
#endif
    bool serverPersDesktop = false;
    GraphPersistence graphPersistence;
    bool graphPers = true;
    // Diccionario para comprobar rápidamente si debe trackearse un evento durante ejecución
    Dictionary<string, bool> eventsTracked = new Dictionary<string, bool>();

    private long timeLastUpdate;
    private Tracker()
    {
    }

    public static Tracker Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Tracker();
            }
            return instance;
        }
    }

    public void Init(long id)
    {
        sessionId = id;
        if(filePers) filePersistence = new FilePersistence(true, true, true);
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            serverPersDesktop = false;
        }
        else
        {
            serverPers = false;
        }

        if(serverPers) serverPersistence = new ServerPersistence();
#if !UNITY_WEBGL
        if(serverPersDesktop) serverPersistenceDesktop = new ServerPersistenceDesktop();
#endif

        

        AddTrackableEvent<InicioEvent>(true);
        AddTrackableEvent<FinEvent>(true);
        AddEvent(new InicioEvent());
    }

    public void InitGraphs(GraphConfig[] graphs)
    {
        if(graphPers) graphPersistence = new GraphPersistence(graphs);
    }

    public void Update()
    {
        long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (tSinceLastPost > timeBetweenPosts)
        {
            //if (filePers) filePersistence.Flush();
            //if (serverPers) serverPersistence.Flush();
            tSinceLastPost = 0;
        }
        else
            tSinceLastPost += (time - timeLastUpdate);
        timeLastUpdate = time;
    }

    public void Release()
    {
        if (instance == this)
        {
            AddEvent(new FinEvent());
            if (filePers) filePersistence.Release();
            if (serverPers) serverPersistence.Release();
#if !UNITY_WEBGL
            if (serverPersDesktop) serverPersistenceDesktop.Release();
#endif
        }
    }

    public void AddEvent(TrackerEvent e)
    {
        if (graphPers && graphPersistence != null) graphPersistence.Send(e);
        DDA.Instance.Send(e);
#if !UNITY_EDITOR
        if (eventsTracked[e.GetType().Name])
        {
            if (filePers) filePersistence.Send(e);
            if (serverPers) serverPersistence.Send(e);
#if !UNITY_WEBGL
            if (serverPersDesktop) serverPersistenceDesktop.Send(e);
#endif
        }
#endif
    }

    public long GetSessionId()
    {
        return sessionId;
    }

    public void AddTrackableEvent<T>(bool track)
    {
        if (!typeof(T).IsSubclassOf(typeof(TrackerEvent)))
            return;

        Type type = typeof(T);
        if (!eventsTracked.ContainsKey(type.Name))
            eventsTracked.Add(type.Name, track);
    }

    public List<string> GetEventNames()
    {
        List<string> names = new List<string>();
        foreach(string e in eventsTracked.Keys)
        {
            names.Add(e);
        }
        return names;
    }

    public void ChangeTrackableState<T>(bool track)
    {
        if (!typeof(T).IsSubclassOf(typeof(TrackerEvent)))
            return;

        Type type = typeof(T);
        if (eventsTracked.ContainsKey(type.Name))
            eventsTracked[type.Name] = track;
    }

    public void Flush()
    {
        if(graphPers && graphPersistence != null) graphPersistence.Flush();
#if !UNITY_EDITOR
        if(filePers) filePersistence.Flush();
        if(serverPers) serverPersistence.Flush();
#if !UNITY_WEBGL
        if (serverPersDesktop) serverPersistenceDesktop.Flush();
#endif
#endif
    }
}
