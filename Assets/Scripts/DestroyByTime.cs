using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//para eliminar objetos (asteroides) que han quedado "explotados"
public class DestroyByTime : MonoBehaviour {

    public float lifetime;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
