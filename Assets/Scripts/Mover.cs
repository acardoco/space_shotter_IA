using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//movimiento de los disparos y asteroides
//Los objetos en Unity de "Disparos" y "Asteroides" se ha movido también a la carpeta "Prefabs" para poder copiarlo 
//(se disparará muchas veces y se spawnearán varios asteroides ).

public class Mover : MonoBehaviour {

    public float speed;

    void Start()
    {
        //"forward" quiere decir que se mueva por el eje de las Z
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
