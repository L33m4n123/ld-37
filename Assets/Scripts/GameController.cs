using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject groundPrefab;
    public GameObject heart;
    public GameObject heartContainer;
    public GameObject DeathScreen;
    public GameObject WinScreen;
    public GameObject waveClearedBanner;
    public GameObject nextWaveBanner;
    public GameObject bloodStain;

    [Header("Settings")]
    public GameObject Player;
    public GameObject EnemyParent;
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
    public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public GameObject BulletParent;

    private bool damaged = false;
    private float nextWaveCounterFloat = 0f;

    [Header("Size")]
    public int sizeX = 100;
    public int sizeY = 100;

    [Header("UI")]
    public Text TimeAliveText;
    public Text WaveClearedText;
    public Text NextWaveText;
    public Text DeathMessage;
    public Text WinMessage;
    public Text WaveCount;

    [Header("Audio")]
    public AudioSource failure;
    public AudioSource success;
    public AudioSource DamageSound;
    public AudioSource DeathSoundEnemy;


    private float timeAliveSeconds;
    private List<GameObject> hearts;

    [Header("Points")]
    public int points;
    public Text pointText;

    [Header("Stats")]
    public int health;
    public float invuinvulnerability;
    public int damage = 2;
    public int attacksPerSecond = 3;
    public int bulletSpeed = 8;

    private float timeSinceDamage;
    private int wave;


    // Use this for initialization
    void Start () {
        WorldGen();
        StartGame();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        Vector3 spawnPosition = new Vector3(sizeX / 2, sizeY / 2, 0);
        Player.transform.position = spawnPosition;

        timeAliveSeconds = 0f;
        timeSinceDamage = 0f;
        Time.timeScale = 1f;

        hearts = new List<GameObject>();
        hearts.Clear();
        health = 3;
        points = 0;
        pointText.text = "Points: " + points;
        for (int i = 0; i < health; i++)
        {
            AddHealth();
        }

        attacksPerSecond = 3;
        float aps = 1f / attacksPerSecond;
        Player.GetComponent<PlayerController>().firePerSecond = aps;
        NextWave();
    }

    void WorldGen()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                GameObject go = SimplePool.Spawn(groundPrefab, new Vector3(x, y), Quaternion.identity);
                go.transform.parent = gameObject.transform;
            }
        }
    }

    void Update()
    {
        timeAliveSeconds += Time.deltaTime;
        TimeAliveText.text = "Time Alive: " + Mathf.FloorToInt(timeAliveSeconds) + "s";

        timeSinceDamage += Time.deltaTime;

        // If the player has just been damaged...
        if (damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;

    }

    void LateUpdate()
    {

        Camera.main.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10f);
        Camera.main.transform.rotation = Quaternion.identity;
    }

    public void TakeDamage()
    {
        if (timeSinceDamage >= invuinvulnerability)
        {
            damaged = true;
            health--;
            GameObject h = hearts[0];
            SimplePool.Despawn(h);
            hearts.Remove(h);

            /////////////////////

            DamageSound.Play();
               

            /////////////////////

            if (health <= 0)
            {
                Die();
            }
            timeSinceDamage = 0f;
        }
    }

    public void AddHealth()
    {
        GameObject go = SimplePool.Spawn(heart, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(heartContainer.transform, true);
        hearts.Add(go);
    }

    public void GetMoney()
    {
        points += wave;
        pointText.text = "Points: " + points;
    }

    public void LoseMoney(int amt)
    {
        points -= amt;
        pointText.text = "Points: " + points;
    }

    public void Die()
    {
        Time.timeScale = 0f;
        DeathMessage.text = string.Format("You are Dead! \nYou survived {0} Seconds and gained {1} Points", Mathf.FloorToInt(timeAliveSeconds), points);
        DeathScreen.SetActive(true);
    }

    public void Win()
    {
        Time.timeScale = 0f;
        WinMessage.text = string.Format("You Won! \n If you are dissapointed of this end, think of NMS ;)\n You gained {0} points in {1} Seconds",
            points, Mathf.FloorToInt(timeAliveSeconds));
        WinScreen.SetActive(true);
    }

    public void MoveSpeed()
    {
        Player.GetComponent<PlayerController>().Speed++;
        bulletSpeed++;
        WaveClearedOff();
    }

    public void AttackSpeed()
    {
        attacksPerSecond++;
        float aps = 1f / attacksPerSecond;
        Player.GetComponent<PlayerController>().firePerSecond = aps;
        WaveClearedOff();
    }

    public void HealthUp()
    {
        health++;
        AddHealth();
        WaveClearedOff();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void WaveCleared()
    {
        waveClearedBanner.SetActive(true);
    }

    public void WaveClearedOff()
    {
        waveClearedBanner.SetActive(false);
        Invoke("NextWave", 1.5f);
    }

    public void NextWave()
    {
        NextWaveText.text = "Next Wave in 5...";
        nextWaveBanner.SetActive(true);
        StartCoroutine("WaveCounter");
        Invoke("SpawnNextWave", 5f);
    }

    public void NextWaveOff()
    {
        nextWaveBanner.SetActive(false);
    }

    public void SpawnNextWave()
    {
        NextWaveOff();
        GetComponent<WaveController>().SpawnWave();
        wave++;
        WaveCount.text = "Wave: " + wave;
    }

    public int GetBulletSpeed()
    {
        return bulletSpeed;
    }

    IEnumerator WaveCounter()
    {
        for (int i = 5; i >= 0; i--)
        {
            NextWaveText.text = "Next Wave in " + i + "...";
            yield return new WaitForSeconds(1f);
        }
    }
}
