using UnityEngine;
using System.Collections;

public class BombBehavior : MonoBehaviour {

	public AudioSource src;
	public AudioClip splat;


    void OnCollisionEnter2D()
    {
        ExplodeBomb();
    }

    public void ExplodeBomb()
    {
		Debug.Log ("should play");
		src.PlayOneShot (splat, .2f);
        Destroy(gameObject);
    }
}
