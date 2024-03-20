using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class DDA_UnityComponent : MonoBehaviour
{
    public static DDA_UnityComponent instance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DDA.Instance.Init(GetComponent<DDAConfig>());
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
