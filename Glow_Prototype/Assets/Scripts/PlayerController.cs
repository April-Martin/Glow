using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerController : MonoBehaviour {

	public GameObject gameCamera;

    public float jumpHeight = 2;
    public float walkSpeed = 1;
    public float jumpsAllowed = 2;
	public int totalHealth = 2;

    public float gravity = -35;
	public Transform startPos;
	public GameObject gooPrefab;

    private CharacterController2D _controller;
	private Transform glow;

    private int jumpCounter = 0;
    private bool isHoldingDownJump = false;
	private int currHealth;

	// Use this for initialization
	void Start () {
        _controller = gameObject.GetComponent<CharacterController2D>();
		transform.position = startPos.position;

		gameCamera.GetComponent<CameraFollow2D> ().startCameraFollow (this.gameObject);

		currHealth = totalHealth;
		glow = this.gameObject.GetComponentInChildren<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 velocity = CalculateVelocity();
        velocity.y += gravity * Time.deltaTime;
        _controller.move(velocity * Time.deltaTime);

        if (_controller.isGrounded)
            jumpCounter = 0;

	}

    Vector3 CalculateVelocity()
    {
        Vector3 velocity = _controller.velocity;
        velocity.x = 0;

        if (Input.GetKeyDown(KeyCode.K))
        {
            DropGoo();
            KillPlayer();
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            velocity.x = walkSpeed * (-1);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            velocity.x = walkSpeed;
        }

        if (Input.GetAxis("Jump") > 0 && !isHoldingDownJump && (jumpCounter < jumpsAllowed))
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            jumpCounter++;
            isHoldingDownJump = true;

        }

        if (Input.GetAxis("Jump") <= 0)
        {
            isHoldingDownJump = false;
        }

        return velocity;
    }

    void DropGoo()
    {
        Instantiate(gooPrefab, transform.position, Quaternion.identity);
    }
		
	public void DamagePlayer(int dmg)
	{
		Debug.Log ("Damaging player");

		currHealth -= dmg;
		glow.localScale = new Vector3 (currHealth/totalHealth, currHealth/totalHealth, 1);

		if (currHealth <= 0) {
			DropGoo ();
			KillPlayer ();
		}

	}

	public void KillPlayer()
	{
		Debug.Log ("Killing player");
		transform.position = startPos.position;
	}
		
}
