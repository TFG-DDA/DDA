using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    GameObject target;
    GameObject container;

    private void OnEnable()
    {
        container = GameObject.Find("EnemyContainer");
        if (container != null && container.transform.childCount > 0)
            target = container.transform.GetChild(0).gameObject;
    }

    public void NextTarget()
    {
        if (container != null && container.transform.childCount > 0)
            target = container.transform.GetChild(0).gameObject;
        else gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector2 lookDir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            NextTarget();
        }
    }
}
