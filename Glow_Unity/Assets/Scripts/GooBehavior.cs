using UnityEngine;
using UnityEngine.UI;
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
        RectMask2D test = GetComponent<RectMask2D>();
        test.rectTransform.position = transform.position;
        test.rectTransform.sizeDelta = new Vector2(.01f, 01f);
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
        else
        {
            this.transform.parent = null;
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
        // Undo any changes since the previous frame
        transform.position = impactPos;
        transform.eulerAngles = impactRotation;
        hasSplatted = true;

        RectMask2D mask = GetComponent<RectMask2D>();
        Collider2D platform = collision.collider;
        Vector3 minSP;
        Vector3 maxSP;
        Vector3 minCP;
        Vector3 maxCP;
        if (isVerticalCollision)
        {
            // Use the midpoints of the goo's top and bottom boundaries as sample points.
            // Find the closest points on the platform to those sample points.
            minSP = new Vector3(transform.position.x, goo.bounds.min.y);
            maxSP = new Vector3(transform.position.x, goo.bounds.max.y);
            minCP = platform.bounds.ClosestPoint(minSP);
            maxCP = platform.bounds.ClosestPoint(maxSP);
            Debug.DrawLine(minSP, maxSP, Color.yellow, 60);
          //  Debug.DrawLine(minCP, maxCP, Color.cyan, 60);

            Debug.Log("MinSP = (" + minSP.x + ", " + minSP.y + ")");
            Debug.Log("MaxSP = (" + maxSP.x + ", " + maxSP.y + ")");
            Debug.Log("MinCP = (" + minCP.x + ", " + minCP.y + ")");
            Debug.Log("MaxSP = (" + maxCP.x + ", " + maxCP.y + ")");

            float overlapPercentage = (maxCP.y - minCP.y) / goo.bounds.size.y;
            Debug.Log("width: " + splattedGoo.texture.width);
            Debug.Log("height: " + splattedGoo.texture.height);


            if (overlapPercentage < 1)
            {
                // If there's a projection:
                Sprite croppedSprite = Sprite.Create(splattedGoo.texture, 
                    new Rect(0, 0, splattedGoo.texture.width*overlapPercentage, splattedGoo.texture.height), 
                    new Vector2(.5f, .5f), 200);
                goo.sprite = croppedSprite;

                float projection = goo.bounds.size.x - (maxCP.x - minCP.x);

                // excess is above
                if (minSP.y < minCP.y)  
                {
                    transform.position += new Vector3(0, projection / 2, 0);
                }
                     // excess is below
                else
                {
                    transform.position -= new Vector3(0, projection / 2, 0);
                }
                
            }

        }
        else
        {
            // Do the same with the side boundaries.
            minSP = new Vector3(goo.bounds.min.x, transform.position.x);
            maxSP = new Vector3(goo.bounds.max.x, transform.position.x);
            minCP = platform.bounds.ClosestPoint(minSP);
            maxCP = platform.bounds.ClosestPoint(maxSP);

            float overlapPercentage = (maxCP.x - minCP.x) / goo.bounds.size.x;

            // If there's a projection:
            if (overlapPercentage < 1)
            {
                float projection = goo.bounds.size.x - (maxCP.x - minCP.x);
                float croppedWidth = splattedGoo.texture.width * overlapPercentage;
                Sprite croppedSprite;

                // excess is to the right
                if (maxSP.x > maxCP.x) 
                {
                    // Keep the left side of the goo sprite
                    croppedSprite = Sprite.Create(splattedGoo.texture,
                    new Rect(0, 0, croppedWidth, splattedGoo.texture.height),
                    new Vector2(.5f, .5f), 200);
                    transform.position -= new Vector3(projection / 2, 0, 0);

                }
                // excess is to the left
                else 
                {
                    // Keep the right side of the goo sprite
                    croppedSprite = Sprite.Create(splattedGoo.texture,
                    new Rect(splattedGoo.texture.width-croppedWidth, 0, croppedWidth, splattedGoo.texture.height),
                    new Vector2(.5f, .5f), 200);
                    transform.position += new Vector3(projection / 2, 0, 0);
                }
                goo.sprite = croppedSprite;

            }


            Debug.DrawLine(mask.rectTransform.anchorMin, mask.rectTransform.anchorMax, Color.yellow, 60);
            Debug.DrawLine(minCP, maxCP, Color.cyan, 60);
        }
    }
    
    /*
    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("CollisionStay");

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
    */
}
