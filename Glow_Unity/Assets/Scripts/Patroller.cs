using UnityEngine;
using System.Collections;

public class Patroller : EnemyController {

    public int speed = 1;
    public bool isFacingRight = true;


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

		if (_controller.isOnEdgeOfPlatform) {
			isFacingRight = !isFacingRight;
			velocity.x *= (-1);
		}

		_controller.move (velocity * Time.deltaTime);

    }
}
