using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GooBehavior2 : MonoBehaviour
{
    [HideInInspector]
    public AudioSource src;
    public int platformLayerNumber = 9;

    private SpriteRenderer goo;
    private Rigidbody2D rb;
    private ParticleSystem partSys;
    private Vector3 impactVelocity;

    private bool hasCollided = false;
    private bool hasSplatted = false;

    // Use this for initialization
    void Start()
    {
        goo = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        src = GetComponent<AudioSource>();
        partSys = GetComponent<ParticleSystem>();
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

    void LateUpdate()
    {
        if (!hasSplatted && hasCollided)
        {
            hasSplatted = true;
            float coneSize = 2f;

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[partSys.particleCount];
            partSys.GetParticles(particles);
            for (int i = 0; i < partSys.particleCount; i++)
            {
                particles[i].velocity += impactVelocity;
                particles[i].velocity += new Vector3(Random.Range(-coneSize, coneSize), Random.Range(-coneSize, coneSize), 0);
            }

            partSys.SetParticles(particles, partSys.particleCount);
        }
    }

    void OnParticleCollision(GameObject collider)
    {
        Debug.Log("hey");
        ParticleCollisionEvent[] collisions = new ParticleCollisionEvent[ParticlePhysicsExtensions.GetSafeCollisionEventSize(partSys)];
        ParticlePhysicsExtensions.GetCollisionEvents(partSys, collider, collisions);
        for (int i = 0; i < collisions.Length; i++)
        {
            Debug.Log("normal = " + collisions[i].normal);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        hasCollided = true;

        partSys.Play();
        rb.isKinematic = true;
        goo.enabled = false;

        // Handle moving platforms
        if (collision.collider.tag == "MovingPlatform")
        {
            this.transform.SetParent(collision.collider.transform);
        }
        else
        {
            this.transform.SetParent(null);
        }





        /*
        // Change to splatted form
        float oldGooWidth = goo.bounds.size.x;
        float oldGooHeight = goo.bounds.size.y;
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


        */
        hasCollided = true;

        // Play splat sound
        src.Play();

    }


}

