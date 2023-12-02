using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIGamepad : MonoBehaviour
{
    [SerializeField]
    GameObject first;

    private bool beenConnected = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.instance.getConnected())
        {
            EventSystem.current.firstSelectedGameObject = null;
            EventSystem.current.SetSelectedGameObject(null);
            beenConnected = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beenConnected && !GameManager.instance.getConnected())
        {
            EventSystem.current.SetSelectedGameObject(null);
            beenConnected = false;
        }
        else if(!beenConnected && GameManager.instance.getConnected())
        {
            EventSystem.current.SetSelectedGameObject(first);
            beenConnected = true;
        }
        if (GameManager.instance.getConnected() && EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(first);
    }
}
