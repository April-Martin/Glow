using UnityEngine;
using System.Collections;

public class BombBehavior : MonoBehaviour {

	public AudioClip splat;
	public ParticleSystem explosionPrefab;
	[HideInInspector]
	public AudioSource src;



    void OnCollisionEnter2D()
    {
        ExplodeBomb();
    }

    public void ExplodeBomb()
    {
		ParticleSystem partSys = (ParticleSystem) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		src.PlayOneShot (splat, .2f);
        Destroy(gameObject);
    }
}
