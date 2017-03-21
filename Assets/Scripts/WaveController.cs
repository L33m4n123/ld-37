using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    [Header("Setting")]
    public int difficulty;
    public float spawnDelay;
    public int enemiesPerWave;
    int waveCount;

    bool wave;

    public int[] Waves;

    [Header("Prefab")]
    public GameObject Enemy;
    public GameObject EnemyParent;

    private GameController gc;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        waveCount = 0;
        gc = GetComponent<GameController>();
        wave = false;
    }

	// Update is called once per frame
	void Update ()
    {
        if (wave)
        {
            int count = 0;
            foreach (Transform child in EnemyParent.transform) if (child.gameObject.activeInHierarchy) count++;

            if (count == 0)
            {
                wave = false;
                gc.WaveCleared();
            }
        }
    }

    void SpawnEnemy()
    {
        wave = true;
        Vector3 spawnPos = new Vector3(Random.Range(0, 29), Random.Range(0, 29), 0);
        float distance = Vector3.Distance(spawnPos, gc.Player.transform.position);
        if (distance < 10f)
        { 
            Debug.Log("SpawnPos:" + spawnPos);
            Debug.Log("PlayerPos:" + gc.Player.transform.position);
            Vector3 dir = gc.Player.transform.position - spawnPos;
            Debug.Log("Direction:" + dir);

            if (dir.x > 0 && dir.x < 10)
            {
                dir.x += 10;
            }
            else if (dir.x < 0 && dir.x > -10)
            {
                dir.x -= 10;
            }

            if (dir.y > 0 && dir.y < 10)
            {
                dir.y += 10;
            }
            else if (dir.y < 0 && dir.y > -10)
            {
                dir.y -= 10;
            }

            spawnPos = new Vector3(spawnPos.x - dir.x, spawnPos.y - dir.y, 0);
            Debug.Log("new Spawn Pos:" + spawnPos);
            Debug.Log("Direction:" + dir);
            Debug.Log("********");
        }
        GameObject go = SimplePool.Spawn(Enemy, spawnPos, Quaternion.identity);
        go.GetComponentInChildren<Enemy>().speed = Mathf.Clamp(2 * (waveCount * 0.75f), 2, 5);
        go.transform.SetParent(EnemyParent.transform, true);
    }

    public void SpawnWave()
    {
        if (waveCount >= Waves.Length)
        {
            GetComponent<GameController>().Win();
        }
        else
        {
            for (int i = 0; i < Waves[waveCount]; i++)
            {
                Invoke("SpawnEnemy", 0.5f);
            }
            waveCount++;
        }
    }
}
