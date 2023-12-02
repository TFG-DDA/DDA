using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletColliderTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tracker.Instance.AddEvent(new BulletDodgedEvent());
    }
}
