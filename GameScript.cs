using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameScript : MonoBehaviour {

    public GameObject player;
    public GameObject map;
    
    //floor background related
    public GameObject rockPattern1;
    public GameObject rockPattern2;
    public GameObject rockPattern3;
    public GameObject portalRockPrefab;
    public GameObject portalPrefab;

    //enemies to spawn
    public GameObject scarecrow;
    public GameObject championScarecrow;
    public GameObject mage;
    public GameObject championMage;

    private int floor = 1;
    private int enemyCount = 0;
    private Vector3 playerSpawn;

    private GameObject rocks;
    private GameObject portalRock;
    private GameObject portal;

    private int maxEnemyLevel = 2;
    private Bounds mapBounds;

    private PlayerScript playerScript;

	// Use this for initialization
	void Start () {
        //create portal rock and portal
        portalRock = (GameObject)Instantiate(portalRockPrefab, Vector3.zero, Quaternion.identity);
        portal = (GameObject)Instantiate(portalPrefab, Vector3.zero, Quaternion.identity);
        mapBounds = map.GetComponent<Renderer>().bounds;

        playerSpawn = new Vector3(0, -4, 0);
        playerScript = player.GetComponent<PlayerScript>();

        GenerateFloor();

        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	    
        if(playerScript.GetIsDead())
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }
        }
	}

    private void GenerateFloor()
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.GetComponent<Text>().text = "Floor " + floor;

        //block portal and disable teleport
        portalRock.SetActive(true);
        portal.SetActive(false);

        //Pick a rock pattern and create it
        int rockPattern = Random.Range(0, 4);

        switch (rockPattern) 
        {
            case 1:
                rocks = (GameObject)Instantiate(rockPattern1, Vector3.zero, Quaternion.identity);
                break;
            case 2:
                rocks = (GameObject)Instantiate(rockPattern2, Vector3.zero, Quaternion.identity);
                break;
            case 3:
                rocks = (GameObject)Instantiate(rockPattern3, Vector3.zero, Quaternion.identity);
                break;
            default:
                break;
        }

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        int levels = floor; //total amount of levels to distribute to enemies. Equal to current floor for progressive difficulty

        while (levels > 0)
        {
            int enemyLevel = Random.Range(1, levels);

            //clamp enemyLevel to available enemy types
            if (enemyLevel > maxEnemyLevel)
            {
                enemyLevel = Random.Range(1, maxEnemyLevel);
            }

            levels -= enemyLevel;

            Vector3 spawnLoc = GenerateSpawnLoc();

            switch(enemyLevel)
            {
                case 1:
                    SpawnEnemyLevel1(spawnLoc);
                    break;
                case 2:
                    SpawnEnemyLevel2(spawnLoc);
                    break;
            }

            enemyCount++;
        }
    }

    private Vector3 GenerateSpawnLoc()
    {
        //spawn within map bounds
        float mapHeight = mapBounds.size.y;
        float mapWidth = mapBounds.size.x;
        //offset for arena
        float offsetX = 4f;
        float offsetY = 3f;

        //no spawns below player
        float minY = player.transform.position.y + 1f;

        //TODO: not have enemies spawn on top of rocks

        float x = Random.Range((-mapWidth / 2f) + offsetX, (mapWidth / 2f) - offsetX);
        float y = Random.Range(minY, (mapHeight / 2f) - offsetY);

        return new Vector3(x, y, 0);
    }

    private void FloorCleared()
    {
        portalRock.SetActive(false);
        portal.SetActive(true);
    }

    private void CleanUpFloor()
    {
        //remove rocks
        Destroy(rocks);

        //remove existing enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        //remove existing enemy attacks
        GameObject[] enemyAttacks = GameObject.FindGameObjectsWithTag("EnemyAttack");

        foreach (GameObject attack in enemyAttacks)
        {
            Destroy(attack);
        }

    }

    public void EnemyKilled()
    {
        enemyCount--;

        if(enemyCount == 0)
        {
            FloorCleared();
        }
    }

    public void NextFloor()
    {
        CleanUpFloor();
        floor++;
        player.transform.position = playerSpawn;
        GenerateFloor();
    }

    private void SpawnEnemyLevel1(Vector3 spawnLoc)
    {
        int type = Random.Range(1, 3);

        switch(type)
        {
            case 1:
                Instantiate(scarecrow, spawnLoc, Quaternion.identity);
                break;
            case 2:
                Instantiate(mage, spawnLoc, Quaternion.identity);
                break;
        }
    }

    private void SpawnEnemyLevel2(Vector3 spawnLoc)
    {
        int type = Random.Range(1, 3);

        switch (type)
        {
            case 1:
                Instantiate(championScarecrow, spawnLoc, Quaternion.identity);
                break;
            case 2:
                Instantiate(championMage, spawnLoc, Quaternion.identity);
                break;
        }
    }

    private void Reset()
    {
        floor = 0;
        enemyCount = 0;
        NextFloor();
        playerScript.Reset();
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
    }
}
