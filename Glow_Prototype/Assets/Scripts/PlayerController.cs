using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerController : MonoBehaviour {

    public float gravity = -35;
    public float jumpHeight = 2;
    public float walkSpeed = 1;
	public Transform startPos;
	public GameObject gooPrefab;

    private CharacterController2D _controller;


	// Use this for initialization
	void Start () {
        _controller = gameObject.GetComponent<CharacterController2D>();
		transform.position = startPos.position;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 velocity = _controller.velocity;
        velocity.x = 0;

        if (Input.GetAxis("Horizontal") < 0)
        {
            velocity.x = walkSpeed * (-1);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            velocity.x = walkSpeed;
        }

        if (Input.GetAxis("Jump") > 0 && _controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        _controller.move(velocity * Time.deltaTime);

	}

	public void KillPlayer()
	{
		Instantiate (gooPrefab, transform.position, Quaternion.identity);
		transform.position = startPos.position;
	}
}
