using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//clase para que el asteroide rote sobre sus ejes y no parezca algo estático
public class RandomRotation : MonoBehaviour {

    public float tumble;

    void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
    }
}
