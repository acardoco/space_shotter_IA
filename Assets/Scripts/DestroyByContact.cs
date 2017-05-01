using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//destruir el objeto Asteroid cuando lo toque un disparo
public class DestroyByContact : MonoBehaviour {

    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;
    private GameController gameController;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //como el asteroide al principio está dentro del objeto "Boundary", este "if" evita que se elimine nada más empezar la partida
        //prueba a quitar este "if", darle a play y ver que pasa (SPOILER: pasa que desaparece el asteroide).
        //Este tag tuvo que ser añadido a mano.
        if (other.tag == "Boundary")
        {
            return;
        }

        //para que cuando explote salga una animación
        Instantiate(explosion, other.transform.position, other.transform.rotation);

        //animación de explosión para la nave (al chocar con el asteroide)
        if (other.tag == "Player")
        {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            gameController.GameOver();
        }

        gameController.AddScore(scoreValue);
        //Debug.Log (other.name)  
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
