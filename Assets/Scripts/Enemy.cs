using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Stats")]
    public float maxHealth = 5f;
    public float speed = 2f;
    public bool stopWalking;

    private float currHealth;
    private float sinceInterrupt;

    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void OnEnable()
    {
        currHealth = maxHealth;
    }
    	
	// Update is called once per frame
	void Update ()
    {
        #region AI
        if (!stopWalking)
        {
            transform.position =
            Vector3.MoveTowards(transform.position,
            gameController.Player.transform.position,
            speed * Time.deltaTime);
        }

        if (stopWalking)
        {
            Invoke("keepWalking", 1f);
        }
        #endregion
    }

    void keepWalking()
    {
        stopWalking = false;
    }

    public void Damage(float ammount)
    {
        currHealth -= ammount;
        if (currHealth <= 0f)
            Death();
    }

    public void Death()
    {
        gameController.GetMoney();

        //TODO: blood?
        gameController.DeathSoundEnemy.Play();

        SimplePool.Spawn(gameController.bloodStain, transform.position, transform.rotation);

        SimplePool.Despawn(gameObject.transform.parent.gameObject);
    }

    void OnTriggerEnter (Collider col)
    {
        if (col.gameObject.transform.name == "player_graphics")
        {
            stopWalking = true;
            gameController.TakeDamage();
        }
    }
}
