using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Salas : MonoBehaviour
{
    void Start()
    {
        string t = "Last Score: " + (GameManager.instance.getLastScore());
        GetComponent<TextMeshProUGUI>().text = t;
    }
}
