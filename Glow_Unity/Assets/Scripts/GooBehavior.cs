using UnityEngine;
using System.Collections;

public class GooBehavior : MonoBehaviour
{

    /* NOTE:
     * This code is predicated on the mask and the goo being the SAME SIZE.
     * If we change the goo, update the mask!
     * I know that's a drag, but for the time being, this is the solution.
     */

    public SpriteRenderer mask;
    public Sprite splattedGoo;

    private SpriteRenderer goo;
    private Rigidbody2D rb;
    private Vector3 impactVelocity;
    private Vector3 impactPos;
    private Vector3 impactRotation;
    private bool isVerticalCollision = false;
    private bool hasCollided = false;
    private bool hasSplatted = false;


    // Use this for initialization
    void Start()
    {
        goo = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!(hasCollided))
            impactVelocity = rb.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle moving platforms
        if (collision.collider.tag == "MovingPlatform")
        {
            this.transform.parent = collision.collider.transform;
        }

        // Change to splatted form
        float oldGooWidth = goo.bounds.size.x;
        float oldGooHeight = goo.bounds.size.y;
        goo.sprite = splattedGoo;
        GetComponent<BoxCollider2D>().size = goo.bounds.size;


        // Figure out the direction of the collision, and position+rotate the splash accordingly.
        ContactPoint2D[] cps = collision.contacts;
        impactPos = transform.position;
        float fudgeRoom = 0.005f;

        if (cps[0].point.x < cps[1].point.x + fudgeRoom && cps[0].point.x > cps[1].point.x - fudgeRoom)
        {
            isVerticalCollision = true;
            Vector3 offset = new Vector3((oldGooWidth + goo.bounds.size.y) / 2, 0, 0);
            if (impactVelocity.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 90);
                transform.position += offset;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, -90);
                transform.position -= offset;
            }
        }
        else if (cps[0].point.y < cps[1].point.y + fudgeRoom && cps[0].point.y > cps[1].point.y - fudgeRoom)
        {
            isVerticalCollision = false;
            Vector3 offset = new Vector3(0, (oldGooHeight + goo.bounds.size.y) / 2, 0);
            if (impactVelocity.y > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 180);
                transform.position += offset;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.position -= offset;
            }
        }
        else
            Debug.Log("Can't figure out the direction of the collision.");


        hasCollided = true;
        impactRotation = transform.eulerAngles;
        impactPos = transform.position;

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (hasSplatted) return;

        rb.isKinematic = true;
        transform.position = impactPos;
        transform.eulerAngles = impactRotation;
        hasSplatted = true;

        // Set up variables for projection calculation
        ContactPoint2D[] cps = collision.contacts;
        float gooSize;
        float overlap;
        if (isVerticalCollision)
        {
            gooSize = goo.bounds.size.y;
            overlap = cps[1].point.y - cps[0].point.y;
        }
        else
        {
            gooSize = goo.bounds.size.x;
            overlap = cps[1].point.x - cps[0].point.x;
        }

        // If part of the sprite is hanging off the platform:
        Debug.Log("gooSize: " + gooSize);
        Debug.Log("overlap: " + overlap);
        if (overlap < gooSize)
        {
            float protrusion = gooSize - Mathf.Abs(overlap);
            Debug.Log("protrusion: " + protrusion);
            SpriteRenderer newMask = (SpriteRenderer)Instantiate(mask, transform.position, Quaternion.identity);
            newMask.transform.parent = this.transform;

            Vector3 offset;
            if (isVerticalCollision)
            {
                newMask.transform.localScale = new Vector3(1, protrusion / gooSize, 1);
                offset = new Vector3(0, overlap / 2, 0);
            }
            else
            {
                newMask.transform.localScale = new Vector3(protrusion / gooSize, 1, 1);
                offset = new Vector3(overlap / 2, 0, 0);
            }
            newMask.transform.position += offset;       // overlap is automatically the right sign for this

        }

    }

}
