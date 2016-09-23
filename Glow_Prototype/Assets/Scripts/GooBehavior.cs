using UnityEngine;
using System.Collections;

public class GooBehavior : MonoBehaviour {

    /* NOTE:
     * This code is predicated on the mask and the goo being the SAME SIZE.
     * If we change the goo, update the mask!
     * I know that's a drag, but for the time being, this is the solution.
     */

    public SpriteRenderer mask;
    private SpriteRenderer goo;
    

	// Use this for initialization
	void Start () {
        goo = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D (Collision2D collision)
    {
        GetComponent<Rigidbody2D>().isKinematic = true;
        ContactPoint2D[] cps = collision.contacts;
        // If part of the sprite is hanging off the platform:
        float overlap = cps[1].point.x - cps[0].point.x;

        if (overlap != goo.bounds.size.x) 
        {
            float protrusion = goo.bounds.size.x - Mathf.Abs(overlap);

            SpriteRenderer newMask = (SpriteRenderer)Instantiate(mask, transform.position, Quaternion.identity);
            newMask.transform.parent = this.transform;
            newMask.transform.localScale = new Vector3(protrusion / goo.bounds.size.x, 1, 1);

            Vector3 offset = new Vector3(overlap / 2, 0, 0);
            newMask.transform.position += offset; // overlap is automatically the right sign for this
            
        }

    }
}
