using UnityEngine;
using System.Collections;

public class PatrollerController : EnemyController {

    public int speed = 1;
    public bool isFacingRight = true;

	private float floatSpeed;

	protected override void Start ()
	{
		base.Start ();
		floatSpeed = (float) speed / 2;
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
			velocity.x = floatSpeed;
        else
            velocity.x = (-1) * floatSpeed;

		// Switch direction if it's at the edge of a platform
		if (_controller.isOnEdgeOfPlatform || _controller.isAgainstWall) {
			isFacingRight = !isFacingRight;
			velocity.x *= (-1);
		}

		// Move
		_controller.move (velocity * Time.deltaTime);

    }
}
