using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityTracker.instance.SetCanvasCamera(Camera.main);

        GameManager.instance.TeleportPlayer(transform);
        UIManager.instance.gameObject.SetActive(true);
        UIManager.instance.ResetFade();
        PlayerInstance.instance.ToggleMovement(true);
        PlayerInstance.instance.gameObject.SetActive(true);
        Camera.main.GetComponent<CameraFollow>().ResetPos();
    }
}
