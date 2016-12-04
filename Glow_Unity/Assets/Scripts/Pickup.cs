using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

    public int amount;
    public pickupType type;
    private float hangOutTime = .3f;
    private Vector3 dest;

    public enum pickupType { goo, health };

    public IEnumerator GrowPickup()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 1);

        while (transform.localScale.x < 1)
        {
            transform.localScale += new Vector3(0.02f, 0.02f);
            transform.position += new Vector3(0, 0.008f);
            yield return null;
        }

        yield break;
    }


    public void PickupAnimation()
    {
		// Kill the particle effect

		ParticleSystem ps = GetComponent<ParticleSystem> ();
		ps.Stop ();
		ps.Clear ();

		// Set the destination it's flying toward
        Camera cam = FindObjectOfType<Camera>();

        if (type == pickupType.goo)
        {
            dest = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        }
        else
        {
            dest = cam.ScreenToWorldPoint(new Vector3(0, Screen.height));
        }

        GetComponent<SpriteRenderer>().sortingOrder = 100;

		// Send it off on its merry way
        IEnumerator coroutine = ShrinkAndMove(Time.time);
        StartCoroutine(coroutine);

    }


    private IEnumerator ShrinkAndMove(float startTime)
    {
        // Blow up size of pickup before sending it off
        while (Time.time - startTime <= hangOutTime)
        {
            transform.localScale += new Vector3(.08f, .08f, 1);
            yield return null;
        }

        while ((transform.position-dest).sqrMagnitude > .25 && transform.localScale.x > .001)
        {
            Vector3 increment = (dest - transform.position);
            increment.Normalize();
            increment *= .3f;

            transform.position += increment;
            transform.localScale -= new Vector3(.05f, .05f, 1);
            yield return null;
        }

        Destroy(this.gameObject);
        yield break;
    }
}
