﻿using UnityEngine;
using System.Collections;
using System.Timers;

public class ShootingEnemyController : MonoBehaviour {

    public float firingInterval;
    public Vector3 firingRange;
    public float projectileSpeed;

    public GameObject projectilePrefab;
    private Animator anim;

	private bool test; 

	void Start () 
    {
        anim = GetComponent<Animator>();
        StartCoroutine("FireProjectile");
	}


    IEnumerator FireProjectile()
    {
        // Randomize start time
        yield return new WaitForSeconds(Random.Range(0, firingInterval));
        anim.SetTrigger("shoot");

        while (true)
        {
            Instantiate (projectilePrefab, transform.position, Quaternion.identity, transform);
            anim.SetTrigger("shoot");
            yield return new WaitForSeconds(firingInterval);
        }
		yield break;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + firingRange);
    }

}
