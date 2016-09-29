using UnityEngine;
using System.Collections;
using Prime31;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public GameObject gameCamera;
	public GameObject gameOverPanel;

    public float jumpHeight = 2;
    public float walkSpeed = 1;
    public float jumpsAllowed = 2;
	public int maxHealth = 2;
    public bool canStartJumpInMidair;

	public float gravity = -35;
	public Transform startPos;
	public GameObject gooPrefab;

    private CharacterController2D _controller;

	private Transform glow;
    private SpriteRenderer sprite;
	private Vector3 maxGlowSize;
    private Vector3 currGlowSize;
    private Vector4 currSpriteBrightness;
    private bool glowDecreasing = false;

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
        sprite = GetComponent<SpriteRenderer>();
		maxGlowSize = glow.localScale;
	}
	
	// Update is called once per frame
	void Update () {

        // Move sprite
        Vector3 velocity = CalculateVelocity();
        velocity.y += gravity * Time.deltaTime;
        _controller.move(velocity * Time.deltaTime);

        // Decrease brightness if necessary
        if (glowDecreasing)
        {
            DecreaseGlow();
        }

        // Handle double jumps
        if (_controller.isGrounded)
            jumpCounter = 0;

        // Handle moving platforms
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

	    if (col.tag == "endLevel") {
			PlayerEndLevel ();
		}
	}


	void SetHealth(int newHealth)
	{
		// Set health
		currHealth = newHealth;

		// Adjust glow
		float healthPercent = (float) currHealth / maxHealth;
		currGlowSize = maxGlowSize;
		currGlowSize.Scale(new Vector3(healthPercent, healthPercent, 1));
        glowDecreasing = true;

		// Adjust sprite brightness
		currSpriteBrightness = sprite.color;
		currSpriteBrightness -= new Vector4 (.2f, .2f, .2f, 0);
			
		// Kill if necessary
		if (currHealth <= 0) {
			Debug.Log ("Health <= 0");
			DropGoo ();
			KillPlayer ();
		}

	}

    void DecreaseGlow()
    {
        if (glow.localScale.x >= currGlowSize.x)
        {
            Vector3 newGlowSize = currGlowSize - new Vector3(.01f, .01f, .01f);
            glow.localScale = newGlowSize;
        }
        if (sprite.color.r >= currSpriteBrightness[0])
        {
            Vector4 newSpriteBrightness = sprite.color;
            newSpriteBrightness -= new Vector4(.01f, .01f, .01f, 0);
            sprite.color = newSpriteBrightness;
        }
    }

	void KillPlayer()
	{
		transform.position = startPos.position;
		SetHealth (maxHealth);
		GetComponent<SpriteRenderer> ().color = Color.white;
        gameOverPanel.SetActive(true);
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
            if (canStartJumpInMidair || _controller.isGrounded || (jumpCounter > 0))
            {
		    	velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
		    	jumpCounter++;
		    	isHoldingDownJump = true;
            }
		}

		if (Input.GetAxis("Jump") <= 0)
		{
			isHoldingDownJump = false;
		}

		return velocity;
	}

	private void PlayerEndLevel () {
		Application.LoadLevel ("mainMenu");
	}

}		
