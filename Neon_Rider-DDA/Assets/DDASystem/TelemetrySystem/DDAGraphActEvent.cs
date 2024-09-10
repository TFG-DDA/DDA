using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDAGraphActEvent : DDAEvent
{
    // Start is called before the first frame update
    public DDAGraphActEvent(uint diffIndx) : base(typeof(DDAGraphActEvent).Name)
    {
        for(int i = 0; i < diffIndx; i++)
        {
            Tracker.Instance.AddEvent(new GraphDifficEvent());
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
