using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlooodStainScript : MonoBehaviour {
    void OnEnable()
    {
        Invoke("Despawn", 5f);
    }

    void Despawn()
    {
        SimplePool.Despawn(gameObject);
    }
}
