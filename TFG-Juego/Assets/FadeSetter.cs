using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSetter : MonoBehaviour
{
    void Awake()
    {
        GameManager.instance.SetFadeObject(GetComponent<Image>());
        GameManager.instance.SetTransitionTime(2.0f);
        GameManager.instance.StartTransition(TransitionTypes.TOLEVEL);
    }
}
