using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleWeapon : MonoBehaviour
{
    GameObject weapon1, weapon2;

    [SerializeField]
    bool hasSecondWeapon;

    // Start is called before the first frame update
    void Start()
    {
        weapon1 = transform.GetChild(1).gameObject;
        weapon2 = transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //if (hasSecondWeapon && Input.mouseScrollDelta.y != 0.0f)
        //{
        //    if (weapon1.activeSelf)
        //    {
        //        weapon1.SetActive(false);
        //        weapon2.SetActive(true);
        //        weapon2.GetComponent<PlayerWeapon>().DoubleWeaponChange();
        //    }
        //    else
        //    {
        //        weapon1.SetActive(true);
        //        weapon2.SetActive(false);
        //        weapon1.GetComponent<PlayerWeapon>().DoubleWeaponChange();
        //    }
        //}
    }

    public void Swap()
    {
        weapon1.SetActive(true);
        weapon2.SetActive(false);
        weapon1.GetComponent<PlayerWeapon>().DoubleWeaponChange();
    }

    public void Infinite()
    {
        weapon1.SetActive(false);
        weapon2.SetActive(true);
        weapon2.GetComponent<PlayerWeapon>().DoubleWeaponChange();
    }
    public GameObject GetFirst()
    {
        return weapon1;
    }
}
