using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    // Esto es codigo duplicado del MenuManager perdon
    public void hoverSound()
    {
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().UI_CHANGE);
    }
    public void pressSound()
    {
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().UI_ACCEPT);
    }
}
