using UnityEngine;
using System.Collections;

public class PatrollingEnemyController : EnemyController {

    public float speed = 1;
	public float dropRate;
	private bool isFacingRight;
    public GameObject pickupPrefab;

	protected override void Start ()
	{
		base.Start ();
		if (transform.localScale.x > 0)
			isFacingRight = true;
		else
			isFacingRight = false;

	}

    protected override void HandleMovement()
    {
        // Handle moving platforms
        if (_controller.isGrounded && _controller.ground != null && _controller.ground.tag == "MovingPlatform")
        {
            this.transform.parent = _controller.ground.transform;
        }
        else
        {
            if (this.transform.parent != null)
                transform.parent = null;
        }


        Vector3 velocity = _controller.velocity;
        velocity.y += gravity * Time.deltaTime;

        // Set direction-dependent variables
		if (isFacingRight)
			velocity.x = speed;
        else
            velocity.x = (-1) * speed;

		// Switch direction if it's at the edge of a platform
		if (_controller.isOnEdgeOfPlatform || _controller.isAgainstWall) {
			isFacingRight = !isFacingRight;
			velocity.x *= (-1);
			//transform.localRotation = Quaternion.Euler (0, 180, 0);
			transform.Rotate(new Vector3(0, 180, 0));
		}

		// Move
		_controller.move (velocity * Time.deltaTime);

    }

    protected override void KillEnemy()
    {
        float rnd = Random.Range(0, 1);
        if (rnd < dropRate)
            spawnPickup();
        base.KillEnemy();
    }

    private void spawnPickup()
    {
        Vector3 startPos = new Vector3(transform.position.x, GetComponent<SpriteRenderer>().bounds.min.y);
        GameObject pickup = (GameObject) Instantiate(pickupPrefab, startPos, Quaternion.identity);
        pickup.transform.localScale = new Vector3(0.1f, 0.1f, 1);

        // Make sure it sticks to moving platforms
        if (transform.parent != null)
        {
            pickup.transform.SetParent(transform.parent);
        }

        pickup.GetComponent<Pickup>().StartCoroutine("GrowPickup");
    }
}
