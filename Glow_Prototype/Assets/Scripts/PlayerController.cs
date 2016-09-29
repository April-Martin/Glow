using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerController : MonoBehaviour {

	public GameObject gameCamera;
	public GameObject gameOverPanel;

    public float jumpHeight = 2;
    public float walkSpeed = 1;
    public float jumpsAllowed = 2;
	public int maxHealth = 2;

	public float gravity = -35;
	public Transform startPos;
	public GameObject gooPrefab;

    private CharacterController2D _controller;
	private Transform glow;

	private Vector3 maxLight;

    private int jumpCounter = 0;
    private bool isHoldingDownJump = false;
	private int currHealth;

	// Use this for initialization
	void Start () {
        _controller = gameObject.GetComponent<CharacterController2D>();
		transform.position = startPos.position;

		gameCamera.GetComponent<CameraFollow2D> ().startCameraFollow (this.gameObject);

		currHealth = maxHealth;
		glow = gameObject.transform.GetChild (0);
		maxLight = glow.localScale;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 velocity = CalculateVelocity();
        velocity.y += gravity * Time.deltaTime;
        _controller.move(velocity * Time.deltaTime);

        if (_controller.isGrounded)
            jumpCounter = 0;

		if (_controller.isGrounded && _controller.ground != null && _controller.ground.tag == "MovingPlatform") {
			this.transform.parent = _controller.ground.transform;
		} else {
			if (this.transform.parent != null)
				transform.parent = null;
		}
	}


	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Killer") {
			KillPlayer ();
		}

		if (col.tag == "Damager") {
			SetHealth (currHealth - 1);
		}
	}


	void SetHealth(int newHealth)
	{
		// Set health
		currHealth = newHealth;

		// Adjust glow
		float healthPercent = (float) currHealth / maxHealth;
		Vector3 newLight = maxLight;
		newLight.Scale(new Vector3(healthPercent, healthPercent, 1));
		glow.localScale = newLight;

		// Adjust sprite brightness
		Vector4 newTint = GetComponent<SpriteRenderer>().color;
		newTint -= new Vector4 (.2f, .2f, .2f, 0);
		GetComponent<SpriteRenderer> ().color = newTint;
			
		// Kill if necessary
		if (currHealth <= 0) {
			Debug.Log ("Health <= 0");
			DropGoo ();
			KillPlayer ();
		}

	}

	void KillPlayer()
	{
		transform.position = startPos.position;
		SetHealth (maxHealth);
		GetComponent<SpriteRenderer> ().color = Color.white;
	}



    void DropGoo()
    {
        Instantiate(gooPrefab, transform.position, Quaternion.identity);
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

		
}