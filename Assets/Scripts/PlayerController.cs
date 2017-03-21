using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float Speed = 5f;

    public float firePerSecond = 1/3f;
    private float shot = 0f;

    public GameObject bulletPrefab;

    public Transform bulletParent;
    public GameObject firePos;

    private Vector3 lastFrame;
	
	// Update is called once per frame
	void Update () {
        #region hax
        Vector3 pos = gameObject.transform.position;
        if (pos.x < 0)
            pos.x = 0;
        if (pos.x > 29)
            pos.x = 29;
        if (pos.y < 0)
            pos.y = 0;
        if (pos.y > 29)
            pos.y = 29;

        gameObject.transform.position = pos;

        #endregion

        #region camera
        // look where mouse is at.
        // Raycast Mouse Looking
        bool update = false;
        if (lastFrame != Input.mousePosition)
        {
            lastFrame = Input.mousePosition;
            update = true;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (update && Physics.Raycast(ray, out hit, 100))
        {
            update = false;
            Vector3 hitPoint = hit.point;

            Vector3 targetDir = hitPoint - transform.position;

            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        #endregion
        #region movement
        float x = Input.GetAxisRaw("Horizontal-Movement");
        float y = Input.GetAxisRaw("Vertical-Movement");
        Vector3 moveDirection =
            new Vector3(
                x,
                -y, 0f).normalized * Speed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            transform.Translate(moveDirection, Space.World);
        }
        #endregion

        #region Fire
        if (shot > 0f)
        {
            shot -= Time.deltaTime;
            if (shot < 0f)
                shot = 0f;
        }
        if (Input.GetButton("Fire"))
        {
            if (shot <= 0f)
            {
                shot = firePerSecond;
                GameObject go = SimplePool.Spawn(bulletPrefab,
                    firePos.transform.position,
                    transform.rotation);
                go.transform.parent = bulletParent;
            }
        }
        #endregion
    }
}