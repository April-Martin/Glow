using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GooBehavior : MonoBehaviour
{

    public Sprite splattedGoo;
	public AudioSource src;
    public int platformLayerNumber = 9;

    private SpriteRenderer goo;
    private Rigidbody2D rb;
    private Vector3 impactVelocity;
    private Vector3 impactPos;
    private Vector3 impactRotation;
    private enum collisionType { vert_right, vert_left, horiz_bottom, horiz_top };
    private collisionType colType;
    private bool hasCollided = false;
    private bool hasSplatted = false;


    // Use this for initialization
    void Start()
    {
        goo = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
		src = GetComponent<AudioSource> ();
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
        rb.isKinematic = true;

        // Handle moving platforms
        if (collision.collider.tag == "MovingPlatform")
        {
            this.transform.SetParent(collision.collider.transform);
        }
        else
        {
            this.transform.SetParent(null);
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
            Vector3 offset = new Vector3((oldGooWidth + goo.bounds.size.y) / 2, 0, 0);
            if (impactVelocity.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 90);
                transform.position += offset;
                colType = collisionType.vert_right;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, -90);
                transform.position -= offset;
                colType = collisionType.vert_left;
            }
        }
        else if (cps[0].point.y < cps[1].point.y + fudgeRoom && cps[0].point.y > cps[1].point.y - fudgeRoom)
        {
            Vector3 offset = new Vector3(0, (oldGooHeight + goo.bounds.size.y) / 2, 0);
            if (impactVelocity.y > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 180);
                transform.position += offset;
                colType = collisionType.horiz_top;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.position -= offset;
                colType = collisionType.horiz_bottom;
            }
        }
        else
            Debug.Log("Can't figure out the direction of the collision.");


        hasCollided = true;
        impactRotation = transform.eulerAngles;
        impactPos = transform.position;

		// Play splat sound
		src.Play();

        CropExcess();
    }
    

    void CropExcess()
    {
        // Set up variables for projection calculation
        RectMask2D mask = GetComponent<RectMask2D>();
        Sprite croppedSprite;
        Vector3 minSP, maxSP, minCP, maxCP;

        // --- Vertical collisions ---
        if (colType == collisionType.vert_left || colType == collisionType.vert_right)
        {
            // Use the midpoints of the goo's top and bottom boundaries as sample points.
            // Ray cast from each sample point to find the contact platform(s) 
            Vector3 rayDirection;
            if (colType == collisionType.vert_left)
                rayDirection = Vector3.left;
            else
                rayDirection = Vector3.right;
            Collider2D botPlatform = Physics2D.Raycast(new Vector2(transform.position.x, goo.bounds.min.y), 
                rayDirection, goo.bounds.size.x, 1 << platformLayerNumber).collider;
            Collider2D topPlatform = Physics2D.Raycast(new Vector2(transform.position.x, goo.bounds.max.y),
                rayDirection, goo.bounds.size.x, 1 << platformLayerNumber).collider;

            // If either is null, then just use the non-null result.
            if (botPlatform == null)
                botPlatform = topPlatform;
            else if (topPlatform == null)
                topPlatform = botPlatform;

            // Find the closest points on the platform to those sample points.
            minSP = new Vector3(transform.position.x, goo.bounds.min.y);
            maxSP = new Vector3(transform.position.x, goo.bounds.max.y);
            minCP = botPlatform.bounds.ClosestPoint(minSP);
            maxCP = topPlatform.bounds.ClosestPoint(maxSP);

            float overlapPercentage = (maxCP.y - minCP.y) / goo.bounds.size.y;

            if (overlapPercentage < 1)
            {
                // If there's a projection:
                croppedSprite = Sprite.Create(splattedGoo.texture,
                    new Rect(0, 0, splattedGoo.texture.width * overlapPercentage, splattedGoo.texture.height),
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
        // --- Horizontal collisions ---
        else
        {
            // Do the same with the side boundaries.

            Vector3 rayDirection;
            if (colType == collisionType.horiz_bottom)
                rayDirection = Vector3.down;
            else
                rayDirection = Vector3.up;
			
			Collider2D leftPlatform = Physics2D.Raycast(new Vector2(goo.bounds.min.x, transform.position.y),
                rayDirection, goo.bounds.size.y, 1 << platformLayerNumber).collider;
			Collider2D rightPlatform = Physics2D.Raycast(new Vector2(goo.bounds.max.x, transform.position.y),
                rayDirection, goo.bounds.size.y, 1 << platformLayerNumber).collider;



			if (leftPlatform == null) {
				leftPlatform = rightPlatform;
				Debug.Log ("left is null");
			}
			if (rightPlatform == null) {
				rightPlatform = leftPlatform;
				Debug.Log ("right is null");

			}

            minSP = new Vector3(goo.bounds.min.x, transform.position.y);
            maxSP = new Vector3(goo.bounds.max.x, transform.position.y);
            minCP = leftPlatform.bounds.ClosestPoint(minSP);
            maxCP = rightPlatform.bounds.ClosestPoint(maxSP);

            float overlapPercentage = (maxCP.x - minCP.x) / goo.bounds.size.x;

            // If there's a projection:
            if (overlapPercentage < 1)
            {
                float projection = goo.bounds.size.x - (maxCP.x - minCP.x);
                float croppedWidth = splattedGoo.texture.width * overlapPercentage;

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
                    new Rect(splattedGoo.texture.width - croppedWidth, 0, croppedWidth, splattedGoo.texture.height),
                    new Vector2(.5f, .5f), 200);
                    transform.position += new Vector3(projection / 2, 0, 0);
                }
                goo.sprite = croppedSprite;

            }
        }

    }
    /*
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (hasSplatted) return;

        // Undo any changes since the previous frame
        rb.isKinematic = true;
        transform.position = impactPos;
        transform.eulerAngles = impactRotation;
        hasSplatted = true;

        // Set up variables for projection calculation
        RectMask2D mask = GetComponent<RectMask2D>();
        Collider2D platform = collision.collider;
        Sprite croppedSprite;
        Vector3 minSP, maxSP, minCP, maxCP;

        // --- Vertical collisions ---
        if (isVerticalCollision)
        {
            // Use the midpoints of the goo's top and bottom boundaries as sample points.
            // Find the closest points on the platform to those sample points.
            minSP = new Vector3(transform.position.x, goo.bounds.min.y);
            maxSP = new Vector3(transform.position.x, goo.bounds.max.y);
            minCP = platform.bounds.ClosestPoint(minSP);
            maxCP = platform.bounds.ClosestPoint(maxSP);

            float overlapPercentage = (maxCP.y - minCP.y) / goo.bounds.size.y;

            if (overlapPercentage < 1)
            {
                // If there's a projection:
                croppedSprite = Sprite.Create(splattedGoo.texture, 
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
        // --- Horizontal collisions ---
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
        }
     * 
    }*/
}
