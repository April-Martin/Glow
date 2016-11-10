using UnityEngine;
using System.Collections;

public class ComplexMovingPlatform : MonoBehaviour {

    public float speed = 1;

    private Vector3[] waypoints;
    private int prevIndex = 0;
    private int nextIndex = 0;
    private Vector3 currVelocity;

    private float distSqrToPrev
    {
        get { return (waypoints[prevIndex] - transform.position).sqrMagnitude; }
    }
    private float distSqrToNext
    {
        get { return (waypoints[nextIndex] - transform.position).sqrMagnitude; }
    }


	void Start () {

        // Initialize waypoints
        Transform path = transform.parent.FindChild("Path");
        waypoints = new Vector3[path.childCount];
        for (int i = 0; i < path.childCount; i++)
        {
            waypoints[i] = path.GetChild(i).transform.position;
            Debug.Log("Waypoint @ (" + waypoints[i].x + ", " + waypoints[i].y + ")");
        }
        transform.position = waypoints[0];

	
	}
	
	void Update () {
        
		// If we are at the target waypoint, move on to the next
		float fudgeRoom = .1f;
		if ((transform.position.x > waypoints[nextIndex].x - fudgeRoom && transform.position.x < waypoints[nextIndex].x + fudgeRoom)
			&& (transform.position.y > waypoints[nextIndex].y - fudgeRoom && transform.position.y < waypoints[nextIndex].y + fudgeRoom))
		{
			// But we don't want to get stuck at one waypoint.
			if (distSqrToNext <= distSqrToPrev)
			{
				prevIndex = nextIndex;
				nextIndex = (nextIndex + 1) % waypoints.Length;
				//    SetPathToNext();
			}
		}

		// Calculate direction to next waypoint
		SetPathToNext();

        // Apply velocity
        transform.position += (currVelocity * Time.deltaTime);

	}


    private void SetPathToNext()
    {
        currVelocity = (waypoints[nextIndex] - transform.position);
        currVelocity.Normalize();
        currVelocity *= speed;
    }

}
