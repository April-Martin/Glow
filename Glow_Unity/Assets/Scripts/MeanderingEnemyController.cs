using UnityEngine;
using System.Collections;
using System.Timers;

public class MeanderingEnemyController : EnemyController {

	public float startSpeed = 1;
	public float minSpeed;
	public float maxSpeed;
	public int minChangeInterval;
	public int maxChangeInterval;

	public int percent_directionChange;
	public float percent_speedChange;
	public float percent_speedAndDirectionChange;
	public float percent_stop;

	private bool isFacingRight;
	private float currSpeed; 
	private Timer changeInterval;
	private System.Random rnd;

	protected override void Start ()
	{
		base.Start ();

		currSpeed = startSpeed;
		if (transform.localScale.x > 0)
			isFacingRight = true;
		else
			isFacingRight = false;

		rnd = new System.Random ();
		float interval = (float) rnd.Next(minChangeInterval*1000, maxChangeInterval*1000);
		Timer changeInterval = new Timer (interval);
		changeInterval.Elapsed += ChangeMotion;
		changeInterval.Start ();
		Debug.Log ("interval = " + interval);
	}


	private void ChangeMotion(object o, ElapsedEventArgs e)
	{
		Debug.Log ("Changing motion");

		// Change direction and/or speed
		int changeType = rnd.Next(100);
		// direction
		//if (changeType <
		if (changeType < percent_directionChange) 
		{
			isFacingRight = !isFacingRight;
		}
		else if (changeType < percent_directionChange + percent_speedChange) 
		{
			RandomizeSpeed ();
		} 
		else if (changeType < percent_directionChange + percent_speedChange + percent_speedAndDirectionChange) 
		{
			isFacingRight = !isFacingRight;
			RandomizeSpeed ();
		}
		else {
			currSpeed = 0;
		}

		// Set new time
		float interval = (rnd.Next(minChangeInterval*100, maxChangeInterval*100))/100;
		changeInterval.Interval = interval;
		Debug.Log ("New interval = " + interval);
	}

	private void RandomizeSpeed()
	{
		int adjustedMin = (int)(minSpeed * 100);
		int adjustedMax = (int)(maxSpeed * 100);
		float newSpeed = (rnd.Next (adjustedMin, adjustedMax)) / 100;
		currSpeed = newSpeed;
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
			velocity.x = currSpeed;
		else
			velocity.x = (-1) * currSpeed;

		// Switch direction if it's at the edge of a platform or if it hit an obstacle
		if (_controller.isOnEdgeOfPlatform || _controller.isAgainstWall) {
			isFacingRight = !isFacingRight;
			velocity.x *= (-1);
		}

		// Move
		_controller.move (velocity * Time.deltaTime);

		// Make sure enemy is facing correct way
		if (velocity.x > 0)
			transform.localRotation = Quaternion.Euler(0, 0, 0);
		else if (velocity.x < 0)
			transform.localRotation = Quaternion.Euler(0, 180, 0);

	}
}
