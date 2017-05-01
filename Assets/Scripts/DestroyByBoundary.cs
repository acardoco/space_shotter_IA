using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByBoundary : MonoBehaviour {

    //todo aquel que pase los limites del objeto "Boundary" será destruido (para no tener disparos sueltos por el mundo).
    void OnTriggerExit(Collider other)
    {
        Destroy(other.gameObject);
    }
}
