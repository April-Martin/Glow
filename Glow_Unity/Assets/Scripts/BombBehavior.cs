using UnityEngine;
using System.Collections;

public class BombBehavior : MonoBehaviour {

	public AudioClip splat;
	public ParticleSystem explosionPrefab;
    public GameObject gooPrefab;
	[HideInInspector]
	public AudioSource src;



    void OnCollisionEnter2D()
    {
        ExplodeBomb();
        float gooHeight = GetComponent<SpriteRenderer>().bounds.size.y;
 //       Pickup pickup = ((GameObject)Instantiate(gooPrefab, transform.position - new Vector3 (0, gooHeight/2, 0), Quaternion.identity)).GetComponent<Pickup>();
 //       pickup.StartCoroutine("GrowPickup");
    }

    public void ExplodeBomb()
    {
		ParticleSystem partSys = (ParticleSystem) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		src.PlayOneShot (splat, .2f);
        
 //        Vector3 offset = new Vector3(gooPrefab.GetComponent<SpriteRenderer>().bounds.size.x, 0);
/*
         Instantiate(gooPrefab, transform.position + offset, Quaternion.identity);
        Instantiate(gooPrefab, transform.position - offset, Quaternion.identity);
         Instantiate(gooPrefab, transform.position, Quaternion.identity);
*/

        Destroy(gameObject);
    }
}
