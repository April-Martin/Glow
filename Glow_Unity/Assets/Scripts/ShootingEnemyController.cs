using UnityEngine;
using System.Collections;
using System.Timers;

public class ShootingEnemyController : MonoBehaviour {

    public float firingInterval;
    public Vector3 firingRange;
    public float projectileSpeed;

    public GameObject projectilePrefab;
    private Animator anim;

	void Start () 
    {
        StartCoroutine("FireProjectile");
	}


    IEnumerator FireProjectile()
    {
        while (true)
        {
            Instantiate (projectilePrefab, transform.position, Quaternion.identity, transform);
            yield return new WaitForSeconds(firingInterval);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + firingRange);
    }

}
