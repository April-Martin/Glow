/*
 * Note: Code pretty much cloned from Prof. Johnstone's "MovingPlatform" class
 * */

using UnityEngine;
using System.Collections;

public class ShooterProjectileBehavior : MonoBehaviour {

    private float speed;
    private Vector3 startPos;
    private Vector3 endPos;
    private ShootingEnemyController shooter;
    private float timer = 0;

	// Use this for initialization
	void Start () {
        shooter = transform.parent.GetComponent<ShootingEnemyController>();
        speed = shooter.projectileSpeed;
        endPos = transform.position + shooter.firingRange;
        startPos = transform.position;

        float dist = Vector3.Distance(startPos, endPos);
        if (dist != 0)
            speed = speed / dist;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime * speed;

        if (timer > 1)
            Destroy(gameObject);

        transform.position = Vector3.Lerp(startPos, endPos, timer);
	}
}
