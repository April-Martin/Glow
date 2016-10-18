using UnityEngine;
using System.Collections;
using Prime31;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {


    // Dependency variables
	public GameManager gm;
	public GameObject gameCamera;
    public Transform startPos;
    public GameObject gooPrefab;
    public GameObject bombPrefab;
    public GameObject aimingIconPrefab;

    // Gameplay tuners
    public float walkSpeed = 1;
    public float hopHeight = 0.5f;
    public float jumpHeight = 2;
    public float jumpsAllowed = 2;
	public int maxHealth = 2;
    public bool canStartJumpInMidair;
	public float gravity = -35;
    public float maxThrow = 6;

    public float aimingIconInterval = 0.4f;
    public float aimingSensitivity = .1f;

    // General variables
    private CharacterController2D _controller;
    private SpriteRenderer sprite;
    private GooBar gooBar;

    // Brightness variables
	private Transform glow;
	private Vector3 maxGlowSize;
    private Vector3 currGlowSize;
    private Vector4 currSpriteBrightness;
   // private bool glowDecreasing = false;

    // Jump variables
    private int jumpCounter = 0;
    private bool isHoldingDownJump = false;
    private bool isHopping = false;

    // Misc variables
	private int currHealth;
    private bool isThrowingBomb = false;
    private bool isThrowingSpit = false;
    private Vector3 aimingDirection;
    private float aimingIconElapsed = 0;
    private bool isFacingLeft = false;

	// Use this for initialization
	void Start () {
        _controller = GetComponent<CharacterController2D>();
		transform.position = startPos.position;
        gooBar = GetComponent<GooBar>();

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

        // Execute actions
        HandleActions();

        // Decrease brightness if necessary
		/*
        if (glowDecreasing)
        {
            DecreaseGlow();
        }
*/

        // Handle double jumps
        if (_controller.isGrounded)
            jumpCounter = 0;

        // Handle moving platforms
		if ((_controller.isGrounded || isHopping) && _controller.ground != null && _controller.ground.tag == "MovingPlatform") {
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

		if (col.tag == "PlayerDamager") {
			SetHealth (currHealth - 1);
		}

	    if (col.tag == "endLevel") {
			gm.ExitLevel ();
		}
	}


	void SetHealth(int newHealth)
	{
		/*
		if (newHealth > currHealth) {
			glowDecreasing = true;
		}
		*/

		// Set health
		currHealth = newHealth;

		// Adjust glow
		float healthPercent = (float) currHealth / maxHealth;
		currGlowSize = maxGlowSize;
		currGlowSize.Scale(new Vector3(healthPercent, healthPercent, 1));
		glow.transform.localScale = currGlowSize;

		// Adjust sprite brightness
		currSpriteBrightness = sprite.color;
		currSpriteBrightness -= new Vector4 (.2f, .2f, .2f, 0);
		sprite.color = currSpriteBrightness;
			
		// Kill if necessary
		if (currHealth <= 0) {
			Debug.Log ("Health <= 0");
			Launch (gooPrefab, Vector3.zero);
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
	}

    void Launch (GameObject projectile, Vector3 velocity)
    {
        GameObject goo = (GameObject)Instantiate(projectile, transform.position, Quaternion.identity);
        goo.GetComponent<Rigidbody2D>().velocity = velocity;

    }




	Vector3 CalculateVelocity()
	{
        isHopping = false;
		Vector3 velocity = _controller.velocity;
		velocity.x = 0;

        if (isThrowingBomb || isThrowingSpit)
            return velocity;

		if (Input.GetKeyDown(KeyCode.K))
		{
			Launch(gooPrefab, Vector3.zero);
			KillPlayer();

            //y size:0.385229
            //y offset:0.3195446
		}

		if (Input.GetAxis("Horizontal") != 0)
		{
            if (Input.GetAxis("Horizontal") > 0)
            {
                velocity.x = walkSpeed;
                isFacingLeft = false;
            }
            else
            {
                velocity.x = walkSpeed * (-1);
                isFacingLeft = true;
            }

            if (_controller.isGrounded)
                velocity.y = Mathf.Sqrt(2f * hopHeight * -gravity);

            isHopping = true;
		}


		if (Input.GetAxis("Jump") > 0 && !isHoldingDownJump && (jumpCounter < jumpsAllowed))
		{
            if (canStartJumpInMidair || _controller.isGrounded || isHopping || (jumpCounter > 0))
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



    void PreviewTrajectory()
    {
        // Launch the icon if enough time has elapsed
        aimingIconElapsed += Time.deltaTime;
        if (aimingIconElapsed >= aimingIconInterval)
        {
            aimingIconElapsed = 0;
            Launch(aimingIconPrefab, aimingDirection);
        }

        // Calculate new aiming vector
        if (Input.GetAxis("Horizontal") != 0)
        {
            if (Input.GetAxis("Horizontal") > 0 && aimingDirection.x < maxThrow)
                aimingDirection.x += aimingSensitivity;
            else if (Input.GetAxis("Horizontal") < 0 && aimingDirection.x > (-1) * maxThrow)
                aimingDirection.x -= aimingSensitivity;
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Vertical") > 0 && aimingDirection.y < maxThrow)
                aimingDirection.y += aimingSensitivity;
            else if (Input.GetAxis("Vertical") < 0 && aimingDirection.y > (-1) * maxThrow)
                aimingDirection.y -= aimingSensitivity;
        }
    }
    void HandleActions()
    {
        // If the user JUST pressed an action button:
        if (!(isThrowingBomb || isThrowingSpit) && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2)))
        {
            if (Input.GetKeyDown(KeyCode.Alpha2) && gooBar.curr >= gooBar.bombCost)
                isThrowingBomb = true;
            else if (Input.GetKeyDown(KeyCode.Alpha1) && gooBar.curr >= gooBar.spitCost)
                isThrowingSpit = true;
            else
                return;


            aimingIconElapsed = 0;
            aimingDirection = new Vector3(3, 3, 0);
            if (isFacingLeft) aimingDirection.x *= (-1);
            Launch(aimingIconPrefab, aimingDirection);
        }

        // If the user just released the spit button:
        else if (isThrowingSpit && Input.GetKeyUp(KeyCode.Alpha1))
        {
            isThrowingSpit = false;
            if (gooBar.updateGooBar(GooBar.Ammo.Spit))
            {
                Launch(gooPrefab, aimingDirection);
                Debug.Log("Threw spit. New goo level: " + gooBar.curr);
            }
        }

        // If the user just released the bomb button:
        else if (isThrowingBomb && Input.GetKeyUp(KeyCode.Alpha2))
        {
            isThrowingBomb = false;
            if (gooBar.updateGooBar(GooBar.Ammo.Bomb))
            {
                Launch(bombPrefab, aimingDirection);
                Debug.Log("Threw bomb. New goo level: " + gooBar.curr);
            }
        }

        if (isThrowingBomb || isThrowingSpit)
        {
            PreviewTrajectory();
        }
    }


}		
