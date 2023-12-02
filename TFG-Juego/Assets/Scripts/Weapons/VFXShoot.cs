using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXShoot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("destruir", 1.0f);
    }

    void destruir()
    {
        Destroy(gameObject);
    }

}
