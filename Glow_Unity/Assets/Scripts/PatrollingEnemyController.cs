using UnityEngine;
using System.Collections;

public class PatrollingEnemyController : EnemyController {

    public int speed = 1;
    
	private bool isFacingRight;
	private float floatSpeed;

	protected override void Start ()
	{
		base.Start ();
		floatSpeed = (float) speed / 2;
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
			velocity.x = floatSpeed;
        else
            velocity.x = (-1) * floatSpeed;

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
}
