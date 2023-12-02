using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public static PlayerInstance instance;

    private bool controller;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeWeapon(int index)
    {
        transform.GetChild(1).GetComponent<PlayerWeapon>().ChangeWeapon(index);
    }

    public void setBulletCollider(bool active) {
        transform.GetChild(3).GetComponent<BoxCollider2D>().isTrigger = !active;
    }

    public void changeControls(bool gamepad)
    {
        controller = gamepad;
        transform.GetChild(0).gameObject.SetActive(!gamepad);
        transform.GetChild(6).gameObject.SetActive(gamepad);
    }

    public bool usingController() { return controller; }

    public WeaponScriptable GetWeapon()
    {
        return GetComponentInChildren<PlayerWeapon>().GetWeapon();
    }

    public string GetWeaponName()
    {
        return transform.GetChild(1).GetComponent<PlayerWeapon>().GetWeapon().name;
    }

    public FireWeapon GetFireWeapon()
    {
        return transform.GetChild(1).GetComponentInChildren<FireWeapon>();
    }
    public void DestroyThyself()
    {
        Destroy(gameObject);
        instance = null;    // because destroy doesn't happen until end of frame
    }

    public void ToggleMovement(bool moves)
    {
        GetComponent<PlayerMovement>().enabled = moves;
        changeControls(GameManager.instance.getConnected());
    }

    public void ToggleArrow(bool shown)
    {
        transform.GetChild(5).gameObject.SetActive(shown);
    }

    public bool ShowingArrow() { return transform.GetChild(5).gameObject.activeSelf; }

    public void NextTargetArrow() { transform.GetChild(5).gameObject.GetComponent<ArrowController>().NextTarget(); }
}
