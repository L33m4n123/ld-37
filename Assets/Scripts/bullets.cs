using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullets : MonoBehaviour {
    
    public float dmg = 1f;
    public float speed = 3f;
    public float maxLifeSpan = 3f;

    private float timeAlive = 0f;
    private GameController gc;

    void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    void OnEnable()
    {
        if (gc == null)
            gc = FindObjectOfType<GameController>();
        timeAlive = 0f;
        speed = gc.GetBulletSpeed();
    }

	// Update is called once per frame
	void Update () {
        timeAlive += Time.deltaTime;
        if (timeAlive > maxLifeSpan)
            SimplePool.Despawn(gameObject);
        gameObject.transform.Translate(speed * Time.deltaTime, 0f, 0f);
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 11)
        {
            col.gameObject.GetComponent<Enemy>().Damage(dmg);
        }
        SimplePool.Despawn(gameObject);
    }
}
