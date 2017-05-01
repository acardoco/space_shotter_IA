using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// crea esta clase auxiliar por "limpieza" (para que no salgan todas las variables a lo loco en Unity, junto al componente "PlayerController" del Asset de la nave ("Player")
// Si pones las variables públicas, como "speed", te saldrán en el componente "PlayerController"
//la serializamos para que aparezca en Unity de forma jerárquica y más bonita
[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

//Clase del objeto nave (Player) 
public class PlayerController : MonoBehaviour
{
    //componente del Asset "Player". Es como la API entre lo que se codea aquí y la nave de Unity
    private Rigidbody rb;

    //instanciamos la clase de las coordenadas
    public Boundary boundary;

    //en el componente "PlayerController", que es un script de la nave, aparece el valor = 10 
    //(Lo puedes modificar al gusto desde Unity porque es una variable pública).
    public float speed;

    //para rotar un poquito la nave cuando se mueva a los lados (darle más realismo al movimiento)
    public float tilt;

    //variables para disparos
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    private float nextFire;

    //instanciamos este objeto para llamar a un funcion suya y restar puntos cuando se dispare
    private GameController gameController;

    void Update()
    {
        int score = gameController.GetScore();
        //no dejar disparar si la puntuacion es negativa
        if (score > 0)
        {
            //esto evita que salgan todos los disparos seguidos y sin espacio entre unos y otros. 
            FirePlayer(Input.GetButton("Fire1"));
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        //instanciamos este objeto para llamar a un funcion suya y restar puntos cuando se dispare
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

    private void FixedUpdate()
    {
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");


        if (gameController.GetScore() > 0)
        {

            string result = gameController.RunClassifierAvoid();
            string result02 = gameController.RunClassifierFire();

            if (result == "nad") MovePlayer(0, 0);
            else if (result == "der" && result02 == "der") MovePlayer(1, 0);
            else if (result == "izq" || result02 == "izq") MovePlayer(-1, 0);
            else MovePlayer(movimientoHorizontal, movimientoVertical);

            if (result02 == "dis") FirePlayer(true);

        }

        else
        {

            string result = gameController.RunClassifierFire();
            if (result == "dis") FirePlayer(true);
            else if (result == "nad") MovePlayer(0, 0);
            else if (result == "der") MovePlayer(1, 0);
            else if (result == "izq") MovePlayer(-1, 0);
            else MovePlayer(movimientoHorizontal, movimientoVertical);
        }
    }

    private void MovePlayer(float movHor, float movVer)
    {

        //indicamos los ejes por los que se puede mover la nave
        Vector3 movement = new Vector3(movHor, 0.0f, movVer);
        rb.velocity = movement * speed;

        //propiedad que se añade para que la nave no salga del mapa
        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );

        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);
    }

    private void FirePlayer(bool fire)
    {


        if (fire && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GetComponent<AudioSource>().Play();
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            //por cada disparo, se restan 5 puntos en el contador
            gameController.AddScore(-5);
        }
    }


}
