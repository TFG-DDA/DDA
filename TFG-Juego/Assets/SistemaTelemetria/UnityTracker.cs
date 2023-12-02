using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class UnityTracker : MonoBehaviour
{
    public static UnityTracker instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Tracker.Instance.Init(AnalyticsSessionInfo.sessionId);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Eventos correspondientes al juego
        Tracker.Instance.AddTrackableEvent<PosicionJugadorEvent>(false);
        Tracker.Instance.AddTrackableEvent<MuerteJugadorEvent>(true);
        Tracker.Instance.AddTrackableEvent<MuerteEnemigoEvent>(true);
        Tracker.Instance.AddTrackableEvent<InicioNivelEvent>(true);
        Tracker.Instance.AddTrackableEvent<FinNivelEvent>(true);
        Tracker.Instance.AddTrackableEvent<FormDataEvent>(true);
        Tracker.Instance.AddTrackableEvent<DashEvent>(true);
        Tracker.Instance.AddTrackableEvent<InicioPausaEvent>(true);
        Tracker.Instance.AddTrackableEvent<FinPausaEvent>(true);
        Tracker.Instance.AddTrackableEvent<RecibirDanoEvent>(true);
        Tracker.Instance.AddTrackableEvent<LevelDifficultyEvent>(true);
        Tracker.Instance.AddTrackableEvent<DisparoEvent>(true);
        Tracker.Instance.AddTrackableEvent<BulletDodgedEvent>(true);
        Tracker.Instance.AddTrackableEvent<DanoEnemigoEvent>(true);
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
}
