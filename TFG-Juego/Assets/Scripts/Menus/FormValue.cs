using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormValue : MonoBehaviour
{
    [SerializeField]
    int value;
    public int getValue() { return value; }
}
