using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariable : MonoBehaviour
{
    public static GameObject[] playerBalls = new GameObject[50];
    public static List<GameObject> playerBallsDead = new List<GameObject>();
    public GameObject playerBallDead;

    [Header("Spawning")]
    public bool canSpawn = true;
    public GameObject playerBall;
    public static GameObject location;
    private int counter = 0;
    private int spawnTimer = 0;
    private bool spawnTimerStart = false;

    [Header("KillPlayerBalls")]
    public float killDelay = 3f;
    private bool killSwitch = false;
    private float killTimer;
    public static bool goToSpawner = false;


    private void Start()
    {
        location = GameObject.Find("SpawnPoint");
    }

    // Update is called once per frame
    void Update()
    {
        killPlayerBalls();

        if (Input.GetKeyUp(KeyCode.P))
        {
            wipeBoard();
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            if (playerBalls[0] != null)
            {
                canSpawn = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            if (spawnTimer == 0)
            {
                SpawnPlayerBall();
                spawnTimer++;
            }
            spawnTimerStart = true;
        }
        if (spawnTimerStart)
        {
            spawnTimer++;
            if (spawnTimer > 5)
            {
                spawnTimer = 0;
                spawnTimerStart = false;
            }
        }
    }

    public static void  ConsolidateArray()
    {
        //code inspired from:
        //https://www.dotnetmirror.com/Articles/vbnet/31/remove-null-blank-values-from-an-array-using-csharp.net
        List<GameObject> tempPlayerBalls = new List<GameObject>();
        foreach (GameObject gO in playerBalls)
        {
            if (gO != null)
            {
                tempPlayerBalls.Add(gO);
            }
        }

        playerBalls = new GameObject[50];
        int index = 0;
        foreach (GameObject gO in tempPlayerBalls)
        {
            playerBalls[index] = gO;
            index++;
        }
    }

    private void killPlayerBalls()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            canSpawn = true;
            killTimer = Time.time + killDelay;
            killSwitch = true;

            for (int i = 0; i < playerBalls.Length; i++)
            {
                GameObject playerBall = playerBalls[i];
                if (playerBall != null)
                {
                    GameObject deadball = Instantiate(playerBallDead, new Vector3(playerBall.transform.position.x, playerBall.transform.position.y, playerBall.transform.position.z), Quaternion.identity);
                    playerBallsDead.Add(deadball);
                    Destroy(playerBall);
                }
            }
        }

        if (killSwitch)
        {
            if (Time.time > killTimer)
            {
                killSwitch = false;
                goToSpawner = true;
            }
        }
    }

    private void wipeBoard()
    {
        for (int i = 0; i < playerBalls.Length; i++)
        {
            if (playerBalls[i] != null)
            {
                Destroy(playerBalls[i]);
            }
        }

        if (playerBallsDead != null)
        {
            foreach (GameObject gO in playerBallsDead)
            {
                Debug.Log("Pressed P");
                Destroy(gO);
            }
        }

        playerBallsDead = new List<GameObject>();

        //move camera to spawner after delay...
    }

    private void SpawnPlayerBall()
    {
        if (canSpawn)
        {
            GameObject pb = Instantiate(playerBall, location.transform.position, Quaternion.identity);
            pb.name = "playerBall" + counter++;
        }
    }
}
