using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//spawn asteroids
public class GameController : MonoBehaviour
{

    public GameObject hazard;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public GUIText scoreText;
    public GUIText restartText;
    public GUIText gameOverText;

    private bool gameOver;
    private bool restart;
    private int score;

    private string learningString;

    void Start()
    {
        gameOver = false;
        restart = false;
        learningString = "";

        restartText.text = "";
        gameOverText.text = "";
        score = 100;
        UpdateScore();
        StartCoroutine(SpawnWaves());
        StartCoroutine(WriteFileFire());
    }

    //aqui se recarga la escena cuando se pulsa la tecla 'R'
    void Update()
    {
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Main");
            }
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);

                //.identity hace que no tengan rotación alguna los hazards. 
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);

            //se sale del bucle cuando se acaba la oleada
            if (gameOver)
            {
                restartText.text = "Press 'R' for Restart";
                restart = true;
                break;
            }
        }
    }

    IEnumerator WriteFileAvoid()
    {
        yield return new WaitForSeconds(0);

        Vector3 currentPos;
        GameObject[] asteroids;
        int fIzq, fDer, fBaj, fAI, fAD;
        double dBaj;
        double defaultDist = 1.5;

        while (true)
        {

            currentPos = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
            asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
            asteroids = asteroids.OrderBy(ast => Vector3.Distance(ast.transform.position, currentPos)).ToArray();

            fIzq = 0; fDer = 0; fBaj = 0;
            fAI = 0; fAD = 0;
            dBaj = 100;

            foreach (GameObject ast in asteroids)
            {

                if (fBaj == 0 && Math.Abs(ast.transform.position.x - currentPos.x) <= defaultDist
                    && Math.Abs(ast.transform.position.z - currentPos.z) <= 5
                    && ast.transform.position.z > currentPos.z)
                {

                    fBaj = 1;
                    dBaj = Math.Round(Vector3.Distance(ast.transform.position, currentPos), 1);
                    continue;
                }

                else if (fIzq == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                    && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                    && ast.transform.position.x < currentPos.x
                    && Math.Abs(ast.transform.position.z - currentPos.z) <= defaultDist)
                {

                    fIzq = 1;
                    continue;
                }

                else if (fDer == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                 && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                 && ast.transform.position.x > currentPos.x
                 && Math.Abs(ast.transform.position.z - currentPos.z) <= defaultDist)
                {

                    fDer = 1;
                    continue;
                }

                else if (fAI == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                 && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                 && ast.transform.position.x < currentPos.x
                 && Math.Abs(ast.transform.position.z - currentPos.z) > defaultDist
                 && Math.Abs(ast.transform.position.z - currentPos.z) <= 5)
                {

                    fAI = 1;
                    continue;
                }

                else if (fAD == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                && ast.transform.position.x > currentPos.x
                && Math.Abs(ast.transform.position.z - currentPos.z) > defaultDist
                && Math.Abs(ast.transform.position.z - currentPos.z) <= 5)
                {
                    fAD = 1;
                    continue;
                }

            }

            if (currentPos.x > 5) fDer = 1;
            else if (currentPos.x < -5) fIzq = 1;

            learningString = learningString + fBaj + "\t" + dBaj + "\t" + fIzq + "\t" + fDer + "\t" + fAI + "\t" + fAD + "\t";

            if (Input.GetAxis("Horizontal") > 0) learningString = learningString + "der";
            else if (Input.GetAxis("Horizontal") < 0) learningString = learningString + "izq";
            else learningString = learningString + "nad";

            learningString = learningString + "\r\n";

            yield return new WaitForSeconds(0.05F);

            if (gameOver)
            {
                System.IO.File.WriteAllText("C:/Users/Raul/Desktop/SpaceDataSet.txt", learningString);
                break;
            }
        }
    }

    IEnumerator WriteFileFire()
    {
        yield return new WaitForSeconds(0);

        Vector3 currentPos;
        GameObject[] asteroids;
        int fIzq, fDer, fBaj, dBaj, dMed, dAlt, fAI, fAD, cScore;
        double defaultDist = 1.5;

        while (true)
        {
            currentPos = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
            asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
            asteroids = asteroids.OrderBy(ast => Vector3.Distance(ast.transform.position, currentPos)).ToArray();
            cScore = GetScore();

            fIzq = 0; fDer = 0; fBaj = 0;
            dBaj = 0; dMed = 0; dAlt = 0;
            fAI = 0; fAD = 0;

            foreach (GameObject ast in asteroids)
            {

                if (fBaj == 0 && Math.Abs(ast.transform.position.x - currentPos.x) <= defaultDist
                    && Math.Abs(ast.transform.position.z - currentPos.z) <= 5
                    && ast.transform.position.z > currentPos.z)
                {

                    fBaj = 1;

                    if (Math.Abs(ast.transform.position.x - currentPos.x) <= 0.5) dBaj = 1;

                    continue;
                }


                else if (dMed == 0 && Math.Abs(ast.transform.position.x - currentPos.x) <= 0.5
                    && Math.Abs(ast.transform.position.z - currentPos.z) <= 10
                    && Math.Abs(ast.transform.position.z - currentPos.z) > 5
                    && ast.transform.position.z > currentPos.z)
                {

                    dMed = 1;
                    continue;
                }

                else if (dAlt == 0 && Math.Abs(ast.transform.position.x - currentPos.x) <= 0.5
                    && Math.Abs(ast.transform.position.z - currentPos.z) > 10
                    && ast.transform.position.z > currentPos.z)
                {

                    dAlt = 1;
                    continue;
                }


                else if (fIzq == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                    && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                    && ast.transform.position.x < currentPos.x
                    && Math.Abs(ast.transform.position.z - currentPos.z) <= defaultDist)
                {

                    fIzq = 1;
                    continue;
                }

                else if (fDer == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                 && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                 && ast.transform.position.x > currentPos.x
                 && Math.Abs(ast.transform.position.z - currentPos.z) <= defaultDist)
                {

                    fDer = 1;
                    continue;
                }

                else if (fAI == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                 && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                 && ast.transform.position.x < currentPos.x
                 && Math.Abs(ast.transform.position.z - currentPos.z) > defaultDist
                 && Math.Abs(ast.transform.position.z - currentPos.z) <= 5)
                {

                    fAI = 1;
                    continue;
                }

                else if (fAD == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                && ast.transform.position.x > currentPos.x
                && Math.Abs(ast.transform.position.z - currentPos.z) > defaultDist
                && Math.Abs(ast.transform.position.z - currentPos.z) <= 5)
                {
                    fAD = 1;
                    continue;
                }

            }

            if (currentPos.x > 5) fDer = 1;
            else if (currentPos.x < -5) fIzq = 1;

            learningString = learningString + fBaj + "\t" + dBaj + "\t" + dMed + "\t" + dAlt + "\t" +
                fIzq + "\t" + fDer + "\t" + fAI + "\t" + fAD + "\t" + cScore + "\t";

            if (Input.GetButton("Fire1")) learningString = learningString + "dis";
            else if (Input.GetAxis("Horizontal") > 0) learningString = learningString + "der";
            else if (Input.GetAxis("Horizontal") < 0) learningString = learningString + "izq";
            else if (Input.GetAxis("Horizontal") == 0) learningString = learningString + "nad";

            learningString = learningString + "\r\n";

            yield return new WaitForSeconds(0.05F);

            if (gameOver)
            {
                System.IO.File.WriteAllText("C:/Users/Raul/Desktop/SpaceDataSet02.txt", learningString);
                break;
            }
        }
    }

    public string RunClassifierAvoid()
    {

        int fIzq, fDer, fBaj, fAI, fAD;
        double dBaj;
        double defaultDist = 1.5;

        Vector3 currentPos = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        asteroids = asteroids.OrderBy(ast => Vector3.Distance(ast.transform.position, currentPos)).ToArray();

        fIzq = 0; fDer = 0; fBaj = 0;
        fAI = 0; fAD = 0;
        dBaj = 100;

        foreach (GameObject ast in asteroids)
        {

            if (fBaj == 0 && Math.Abs(ast.transform.position.x - currentPos.x) <= defaultDist
                && Math.Abs(ast.transform.position.z - currentPos.z) <= 5
                && ast.transform.position.z > currentPos.z)
            {

                fBaj = 1;
                dBaj = Math.Round(Vector3.Distance(ast.transform.position, currentPos), 1);
                continue;
            }

            else if (fIzq == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                && ast.transform.position.x < currentPos.x
                && Math.Abs(ast.transform.position.z - currentPos.z) <= defaultDist)
            {

                fIzq = 1;
                continue;
            }

            else if (fDer == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
             && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
             && ast.transform.position.x > currentPos.x
             && Math.Abs(ast.transform.position.z - currentPos.z) <= defaultDist)
            {

                fDer = 1;
                continue;
            }

            else if (fAI == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
             && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
             && ast.transform.position.x < currentPos.x
             && Math.Abs(ast.transform.position.z - currentPos.z) > defaultDist
             && Math.Abs(ast.transform.position.z - currentPos.z) <= 5)
            {

                fAI = 1;
                continue;
            }

            else if (fAD == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
            && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
            && ast.transform.position.x > currentPos.x
            && Math.Abs(ast.transform.position.z - currentPos.z) > defaultDist
            && Math.Abs(ast.transform.position.z - currentPos.z) <= 5)
            {
                fAD = 1;
                continue;
            }

        }

        if (currentPos.x > 4.5) fDer = 1;
        else if (currentPos.x < -4.5) fIzq = 1;

        return ClassificationModelRulesAvoid(fBaj, fIzq, fDer, fAI, fAD, dBaj);
    }


    public string RunClassifierFire()
    {

        int fIzq, fDer, fBaj, dBaj, dMed, dAlt, fAI, fAD, cScore;
        double defaultDist = 1.5;

        Vector3 currentPos = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        asteroids = asteroids.OrderBy(ast => Vector3.Distance(ast.transform.position, currentPos)).ToArray();
        cScore = GetScore();

        fIzq = 0; fDer = 0; fBaj = 0;
        dBaj = 0; dMed = 0; dAlt = 0;
        fAI = 0; fAD = 0;

        foreach (GameObject ast in asteroids)
        {

            if (fBaj == 0 && Math.Abs(ast.transform.position.x - currentPos.x) <= defaultDist
                && Math.Abs(ast.transform.position.z - currentPos.z) <= 5
                && ast.transform.position.z > currentPos.z)
            {

                fBaj = 1;

                if (Math.Abs(ast.transform.position.x - currentPos.x) <= 0.5) dBaj = 1;

                continue;
            }


            else if (dMed == 0 && Math.Abs(ast.transform.position.x - currentPos.x) <= 0.5
                && Math.Abs(ast.transform.position.z - currentPos.z) <= 10
                && Math.Abs(ast.transform.position.z - currentPos.z) > 5
                && ast.transform.position.z > currentPos.z)
            {

                dMed = 1;
                continue;
            }

            else if (dAlt == 0 && Math.Abs(ast.transform.position.x - currentPos.x) <= 0.5
                && Math.Abs(ast.transform.position.z - currentPos.z) > 10
                && ast.transform.position.z > currentPos.z)
            {

                dAlt = 1;
                continue;
            }


            else if (fIzq == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
                && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
                && ast.transform.position.x < currentPos.x
                && Math.Abs(ast.transform.position.z - currentPos.z) <= defaultDist)
            {

                fIzq = 1;
                continue;
            }

            else if (fDer == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
             && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
             && ast.transform.position.x > currentPos.x
             && Math.Abs(ast.transform.position.z - currentPos.z) <= defaultDist)
            {

                fDer = 1;
                continue;
            }

            else if (fAI == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
             && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
             && ast.transform.position.x < currentPos.x
             && Math.Abs(ast.transform.position.z - currentPos.z) > defaultDist
             && Math.Abs(ast.transform.position.z - currentPos.z) <= 5)
            {

                fAI = 1;
                continue;
            }

            else if (fAD == 0 && Math.Abs(ast.transform.position.x - currentPos.x) > defaultDist
            && Math.Abs(ast.transform.position.x - currentPos.x) <= 3.5
            && ast.transform.position.x > currentPos.x
            && Math.Abs(ast.transform.position.z - currentPos.z) > defaultDist
            && Math.Abs(ast.transform.position.z - currentPos.z) <= 5)
            {
                fAD = 1;
                continue;
            }

        }

        if (currentPos.x > 5) fDer = 1;
        else if (currentPos.x < -5) fIzq = 1;

        return ClassificationModel48Fire(fBaj, dBaj, dMed, dAlt, fIzq, fDer, fAI, fAD, score);

    }

    private string ClassificationModelRulesAvoid(int fBaj, int fIzq, int fDer, int fAI, int fAD, double dBaj)
    {

        if (dBaj > 3.850 && fAI == 0 && fIzq == 0) return "nad";
        if (fDer == 0 && dBaj <= 3.850) return "der";
        if (dBaj > 3.950 && fAD == 1 && fDer == 0) return "nad";
        if (dBaj > 3.750 & fBaj == 1 && dBaj > 4.35) return "nad";
        if (fDer == 0 && fIzq == 0) return "der";
        if (dBaj <= 3.750 && fAI == 1) return "izq";
        if (dBaj > 3.950 && fAI == 1 && fAD == 0) return "nad";
        if (dBaj > 2.650 && fAD == 0 && dBaj <= 3.350) return "izq";
        if (fDer == 0 && fBaj == 0) return "der";
        if (dBaj <= 2.150 && fAD == 1) return "izq";
        if (dBaj >= 2.150 && dBaj <= 2.550 && fAD == 1) return "izq";
        if (dBaj <= 3.550 && fAD == 0) return "izq";
        if (dBaj <= 2.750) return "der";
        if (dBaj > 3.750 && fAD == 1 && fBaj == 1) return "der";
        if (fAD == 0 && fIzq == 0) return "nad";
        if (dBaj > 3.950 && dBaj <= 4.050 && fDer == 0) return "der";
        if (dBaj > 3.850 && dBaj <= 4.150 && fDer == 0 && fBaj == 1) return "der";
        if (dBaj <= 3.950 && dBaj >= 3.250 && fIzq == 0) return "izq";
        if (fBaj == 0 && fAD == 0) return "der";
        if (fAI == 0 && fBaj == 0) return "izq";
        if (fDer == 0 && fBaj == 1) return "nad";
        if (dBaj > 4.100 && fDer == 1) return "der";

        return "izq";

    }

    private string ClassificationModelTreeAvoid(int fBaj, int fIzq, int fDer, int fAI, int fAD, double dBaj)
    {
        if (dBaj > 3.750) return "nad";
        else
        {
            if (dBaj < 1.350) return "nad";

            else
            {
                if (dBaj < 1.450) return "nad";

                else
                {
                    if (fIzq == 0) return "izq";
                    else
                    {
                        if (fDer == 0) return "der";

                        else
                        {
                            if (dBaj > 3.6) return "izq";
                            else return "nad";
                        }

                    }

                }
            }

        }

    }

    private string ClassificationModelJ48Avoid(int fBaj, int fIzq, int fDer, int fAI, int fAD, double dBaj)
    {
        String der = "der";
        String izq = "izq";
        String nad = "nad";
        if (dBaj <= 3.7)
        {
            if (fIzq == 0)
            {
                if (fDer == 0)
                {
                    if (dBaj <=1.7)
                    {
                        if (fAI == 0)
                        {
                            return nad;
                        }else
                        {
                            return izq;
                        }
                    }else
                    {
                        if (dBaj <= 3.2)
                        {
                            return izq;
                        }else
                        {
                            if (fAD == 0)
                            {
                                return izq;
                            }else
                            {
                                if (dBaj <= 3.3)
                                {
                                    if (fAI == 0)
                                    {
                                        return izq;
                                    }else
                                    {
                                        return der;
                                    }

                                }else
                                {
                                    return der;
                                }
                            }
                        }
                    }
                }
                if (fDer == 1)
                {
                    if (fAI == 0)
                    {
                        if (dBaj <= 2.9)
                        {
                            return izq;
                        }
                        else
                        {
                            if (fAD == 0)
                            {
                                return izq;
                            }
                            else
                            {
                                if (dBaj <= 3.2)
                                {
                                    return nad;
                                }
                                else
                                {
                                    return izq;
                                }
                            }
                        }
                    }else
                    {
                        return izq;
                    }
                }
            }else if (fIzq==1)
            {
                if (fDer == 1)
                    return izq;
                if (fDer == 0)
                    return der;
            }
        }else if (dBaj>3.7)
        {
            if (fDer == 0 && fAD == 0 && fIzq == 0 && fAI == 0) return nad;
            if (fDer == 0 && fAD == 0 && fIzq == 0 && fAI == 1)return der;
            if (fDer == 0 && fAD == 0 && fIzq == 1 && fAI == 0 && fBaj == 0) return der;
            if (fDer == 0 && fAD == 0 && fIzq == 1 && fAI == 0 && fBaj == 1 && dBaj <= 4) return der;
            if (fDer == 0 && fAD == 0 && fIzq == 1 && fAI == 0 && fBaj == 1 && dBaj > 4) return nad;
            if (fDer == 0 && fAD == 0 && fIzq == 1 && fAI == 1) return nad;
            if (fDer == 0 && fAD == 1 && fIzq == 0 && fBaj == 0) return nad;
            if (fDer == 0 && fAD == 1 && fIzq == 0 && fBaj == 1 && fAI == 0) return nad;
            if (fDer == 0 && fAD == 1 && fIzq == 0 && fBaj == 1 && fAI == 1) return der;
            if (fDer == 0 && fAD == 1 && fIzq == 1) return nad;
            if (fDer == 1 && fIzq == 0) return nad;
            if (fDer == 1 && fIzq == 1 && fAD == 0) return nad;
            if (fDer == 1 && fIzq == 1 && fAD == 1 && fBaj == 0) return izq;
            if (fDer == 1 && fIzq == 1 && fAD == 1 && fBaj == 1) return der;

        }

        return nad;
    }

    private string ClassificationModelRulesFire(int fBaj, int dBaj, int dMed, int dAlt, int fIzq, int fDer, int fAI, int fAD, int score)
    {

        if (dBaj == 0 && dMed == 0) return "izq";
        if (dBaj == 1 && fDer == 1) return "dis";
        if (dBaj == 1 && dMed == 1) return "dis";
        if (dBaj == 1 && fAI == 1) return "dis";
        if (dBaj == 1 && dAlt == 1) return "dis";
        if (dBaj == 1 && fAD == 0) return "dis";
        if (fDer == 1 && dAlt == 0) return "der";
        if (fBaj == 0 && fIzq == 0) return "dis";
        if (fAD == 0 && fAI == 1) return "izq";
        if (dAlt == 1 && dBaj == 1) return "der";
        if (fAD == 1 && dBaj == 0) return "der";
        if (fIzq == 0 && dBaj == 0) return "der";
        if (fBaj == 0 && dBaj == 0) return "der";
        if (dBaj == 1 && fBaj == 1) return "dis";
        else return "izq";
    }


    private String ClassificationModel48Fire(int fBaj, int dBaj, int dMed, int dAlt, int fIzq, int fDer, int fAI, int fAD, int score)
    {


        if (dBaj == 0)
        {
            if (dMed == 0)
            {
                if (fAD == 0)
                {
                    if (fBaj == 0)
                    {
                        if (fDer == 0)
                        {
                            if (fAI == 0)
                            {
                                if (fIzq == 0)
                                {
                                    if (dAlt == 0) return "der";
                                    else return "izq";
                                }
                                else return "nad";

                            }
                            else
                            {
                                if (dAlt == 0) return "izq";
                                else return "nad";
                            }
                        }
                        else
                        {
                            if (fIzq == 0)
                            {
                                if (dAlt == 0) return "izq";
                                else return "nad";
                            }
                            else return "nad";

                        }
                    }
                    else
                    {
                        if (fIzq == 0)
                        {

                            if (fDer == 0) return "der";
                            else
                            {
                                if (dAlt == 0) return "nad";
                                else return "izq";
                            }

                        }
                        else return "der";

                    }

                }
                else return "der";

            }
            else
            {

                if (fAI == 0)
                {
                    if (fBaj == 0)
                    {
                        if (fAD == 0) return "nad";
                        else
                        {
                            if (dAlt == 0) return "der";
                            else return "nad";
                        }

                    }
                    else
                    {

                        if (fIzq == 0) return "der";
                        else return "izq";
                    }
                }
                else
                {

                    if (fBaj == 0)
                    {
                        if (fIzq == 0) return "nad";
                        else
                        {
                            if (dAlt == 0) return "izq";
                            else return "dis";
                        }

                    }
                    else return "izq";

                }
            }

        }
        else return "dis";
    }

    private String ClassificationModelTreeFire(int fBaj, int dBaj, int dMed, int dAlt, int fIzq, int fDer, int fAI, int fAD, int score)
    {
        if (dBaj == 0)
            return "izq";
        if (dBaj == 1)
            return "dis";
        return "nad";
    }


    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public int GetScore()
    {
        return score;
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        gameOver = true;
    }


}