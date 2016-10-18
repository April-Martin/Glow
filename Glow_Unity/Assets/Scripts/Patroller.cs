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
        float offset = GetComponent<SpriteRenderer>().bounds.size.x;
        if (isFacingRight)
            velocity.x = speed;
        else
        {
            velocity.x = (-1) * speed;
            offset *= (-1);
        }

        // Move in current direction to test spot, to check if we've found a cliff.
        // If we have, switch direction.
        _controller.move(velocity * Time.deltaTime + new Vector3(offset, 0));
        if (!_controller.isGrounded)
        {
            isFacingRight = !isFacingRight;
            velocity.x = (-1) * velocity.x;
        }
        // Move back to correct position (original position + velocity*dt)
        _controller.move(new Vector3(-offset, 0) );
    }
}
