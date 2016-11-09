using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Prime31;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Dependency variables
    public GameManager gm;
    public Camera gameCamera;
    public Transform startPos;
    public GooBar gooBar;
    public GameObject gooPrefab;
    public GameObject bombPrefab;
    public GameObject aimingIconPrefab;
    public int gooLayerNumber = 11;

    // Gameplay tuners
    public float walkSpeed = 1;
    public float hopHeight = 0.5f;
    public float jumpHeight = 2;
    public float jumpsAllowed = 2;
    public int maxHealth = 2;
    public bool canStartJumpInMidair;
    public float gravity = -35;
    public float maxThrow = 6;
    public float glowPenalty = .1f;
    public float invulnerabilityTime = 1.5f;


    // General variables
    private CharacterController2D _controller;
    private SpriteRenderer sprite;

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
    private bool isJumping = false;

    // Animation variables
    Animator animator;
    enum animState { idle, hopStart, hopEnd, jumpStart, jumpEnd, death, spitStart, spitEnd };


    // Throwing + aiming variables
	private bool bombMode = false;
	private bool spitMode = false;
    private Vector3 aimingDirection;
    public float aimingIconInterval = 0.4f;
    public float aimingScaler = .1f;
    [HideInInspector]
    public bool isThrowing = false;
    private LinkedList<GameObject> aimingIcons;


    // Misc variables
    private int currHealth;
    private Vector3 respawnPoint;
    private bool isPickingUpSpit = false;
    private bool isInvulnerable = false;
    private bool isLocked = false;




    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        transform.position = startPos.position;
        respawnPoint = startPos.position;
        animator = GetComponent<Animator>();

        gameCamera.GetComponent<CameraFollow2D>().startCameraFollow(this.gameObject);

        currHealth = maxHealth;
        glow = gameObject.transform.GetChild(0);
        sprite = GetComponent<SpriteRenderer>();
        maxGlowSize = glow.localScale;
        aimingIcons = new LinkedList<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move sprite
        HandleMotion();
        // Execute actions
        HandleActions();
        // Decrease brightness if necessary

        // Handle moving platforms
        if ((_controller.isGrounded || isHopping) && _controller.ground != null && _controller.ground.tag == "MovingPlatform")
        {
            this.transform.parent = _controller.ground.transform;
        }
        else
        {
            if (this.transform.parent != null)
                transform.parent = null;
        }
    }

    void HandleMotion()
    {
        Vector3 velocity = _controller.velocity;

        if (isLocked)
        {
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            _controller.move(velocity * Time.deltaTime);
            return;
        }

        // Note: by putting this here, the recoil and the jump both work properly.
        velocity.x = 0;


        // Handle horizontal input
        if (Input.GetAxis("Horizontal") != 0)
        {
            bool firstHop = false;

            if (!isJumping && !isHopping)
            {
                isHopping = true;
                firstHop = true;
                animator.SetBool("loopHop", true);
                SetAnimationState(animState.hopStart);
            }

            if (!isJumping && _controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(2f * hopHeight * -gravity);
                if (!firstHop)
                    SetAnimationState(animState.hopEnd);
            }
            else if (!isJumping)
            {
                animator.SetBool("loopHop", true);
            }


            if (Input.GetAxis("Horizontal") > 0)
            {
                velocity.x = walkSpeed;
            }
            else
            {
                velocity.x = walkSpeed * (-1);
            }
        }

        // Check if we just released the arrow key
        else if (isHopping)
        {
            // If the player's still in the air:
            if (!_controller.isGrounded)
            {
                animator.SetBool("loopHop", false);
            }
            // If the player just hit the ground after ending a hop sequence:
            else
            {
                isHopping = false;
                SetAnimationState(animState.hopEnd);
            }
        }

        // Check if the player just hit the ground after a jump
        if (_controller.isGrounded && isJumping)
        {
            jumpCounter = 0;
            isJumping = false;
            SetAnimationState(animState.jumpEnd);
        }


        // If the user just pushed jump
        if (Input.GetAxis("Jump") > 0 && !isHoldingDownJump && (jumpCounter < jumpsAllowed))
        {
            if (canStartJumpInMidair || _controller.isGrounded || isHopping || (jumpCounter > 0))
            {
                velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                jumpCounter++;
                isHoldingDownJump = true;
                isJumping = true;
                isHopping = false;
                SetAnimationState(animState.jumpStart);
            }
        }

        if (Input.GetAxis("Jump") <= 0)
        {
            isHoldingDownJump = false;
        }


        // Make sure sprite is facing right direction for motion
        if (velocity.x > 0)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (velocity.x < 0)
            transform.localRotation = Quaternion.Euler(0, 180, 0);

        velocity.y += gravity * Time.deltaTime;

        _controller.move(velocity * Time.deltaTime);
        return;

    }


    void HandleActions()
    {
		// PROCESS KEYBOARD INPUT

		// If the user just entered bomb mode or spit mode:
		if (!(isThrowing) && Input.GetKeyDown (KeyCode.Q) || Input.GetKeyDown (KeyCode.E)) {
			if (Input.GetKeyDown (KeyCode.Q) && gooBar.curr >= gooBar.spitCost) {
				spitMode = true;
				bombMode = false;
			} else if (Input.GetKeyDown (KeyCode.E) && gooBar.curr >= gooBar.bombCost) {
				bombMode = true;
				spitMode = false;
			} else
				return;
		} 

		// If the user just left bomb mode or spit mode:
		else if (Input.GetKeyUp (KeyCode.Q)) {
			spitMode = false;
			isThrowing = false;
		} 
		else if (Input.GetKeyUp (KeyCode.E)) {
			bombMode = false;
			isThrowing = false;
		}


		// If the user just clicked the mouse:]
		if (Input.GetMouseButtonDown (0)) {
			
			// Exit if they're not in bomb mode or spit mode
			if (!(spitMode || bombMode))
				return;
			
			isThrowing = true;
            isLocked = true;
            PreviewTrajectory();
            SetAnimationState(animState.spitStart);
		} 

		// If the user just released it:
		else if (Input.GetMouseButtonUp (0)) {
			
			// Exit if they're not in bomb mode or spit mode
			if (!(spitMode || bombMode))
				return;
			
			isThrowing = false;
            isLocked = false;
			if (spitMode) {
				if (gooBar.DepleteGooBar (GooBar.Ammo.Spit))
					Launch (gooPrefab, aimingDirection);
			} 
			else {
				if (gooBar.DepleteGooBar (GooBar.Ammo.Bomb))
					Launch (bombPrefab, aimingDirection);
			}
            SetAnimationState(animState.spitEnd);
		}



		// IMPLEMENT ACTIONS
		if (isThrowing && ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))) {
			PreviewTrajectory ();
		}


        if (Input.GetKey(KeyCode.S))
            isPickingUpSpit = true;
        else
            isPickingUpSpit = false;

        if (isPickingUpSpit)
        {
            Collider2D spit = FindSpit();
            if (spit != null)
            {
                gooBar.RecoverSpit();
                Destroy(spit.gameObject);
            }
        }
           
    }


    Collider2D FindSpit()
    {
        Debug.Log("trying to find spit");
        bool found = false;

        // Set up two ray tracing points
        Vector3 rayOrigin1 = new Vector2(transform.position.x - (sprite.bounds.size.x / 6), transform.position.y);
        Vector3 rayOrigin2 = new Vector2(transform.position.x + (sprite.bounds.size.x / 6), transform.position.y);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin1, Vector2.down, 0.7f, 1 << gooLayerNumber);
        if (hit.collider != null)
        {
            found = true;
        }
        else
        {
            hit = Physics2D.Raycast(rayOrigin2, Vector2.down, 0.7f, 1 << gooLayerNumber);
            if (hit.collider != null)
            {
                found = true;
            }
        }

        if (found)
        {
            Debug.Log("Found goo!");
            return hit.collider;
            // Increase goo bar
        }
        return null;
    }



    void Launch(GameObject projectile, Vector3 velocity)
    {
        GameObject obj = (GameObject)Instantiate(projectile, transform.position, Quaternion.identity);
        if (transform.parent != null)
            obj.transform.parent = transform.parent;
        obj.GetComponent<Rigidbody2D>().velocity = velocity;
    }




    void PreviewTrajectory()
    {
        // Wipe the current list of aiming icons
        while (aimingIcons.Count > 0)
        {
            GameObject curr = aimingIcons.Last.Value;
            aimingIcons.RemoveLast();
            Destroy(curr.gameObject);
        }

        // Calculate new aiming vector
		UpdateAimingDirection ();

		// Make sure sprite is facing right direction for throw
		if (aimingDirection.x > 0)
			transform.localRotation = Quaternion.Euler(0, 0, 0);
		else
			transform.localRotation = Quaternion.Euler(0, 180, 0);


        // Draw a series of dots marking the trajectory

        Vector3 worldDrawLocation = transform.position;
        Vector3 worldScreenLocation = gameCamera.WorldToScreenPoint(worldDrawLocation);
        float timestep = .01f;
        Vector3 currVelocity = aimingDirection;

        while (worldScreenLocation.x >= 0 && worldScreenLocation.x <= Screen.width &&
            worldScreenLocation.y >=0 && worldScreenLocation.y <= Screen.height)
        {
            // Move to next location
            Vector3 prevLocation = worldDrawLocation;
            float elapsed = 0;
            while ((worldDrawLocation-prevLocation).sqrMagnitude < aimingIconInterval)
            {
                currVelocity += Physics.gravity * timestep;
                worldDrawLocation += currVelocity * timestep;
                elapsed += timestep;
            }
            // Draw aiming icon and add it to the list
            GameObject node = (GameObject) Instantiate(aimingIconPrefab, worldDrawLocation, Quaternion.identity);
            aimingIcons.AddLast(node);
            // Update worldScreenLocation
            worldScreenLocation = gameCamera.WorldToScreenPoint(worldDrawLocation);

        }

    }

	void UpdateAimingDirection()
	{
		// Get vector between player and mouse
		Vector3 diff = Input.mousePosition - gameCamera.WorldToScreenPoint(transform.position);

		// Scale by screen size
		diff /= (Screen.width*aimingScaler);

		if (diff.sqrMagnitude > maxThrow * maxThrow) {
			diff.Normalize ();
			diff *= maxThrow;
		}

		aimingDirection = diff;
	}


    void OnTriggerEnter2D(Collider2D col)
    {
        if (isInvulnerable)
            return;

        if (col.tag == "Killer")
        {
            KillPlayer();
        }

        if (col.tag == "PlayerDamager")
        {
            SetHealth(currHealth - 1);
            Recoil(col);
        }

        if (col.tag == "Checkpoint")
        {
            respawnPoint = col.transform.position;
        }

        if (col.tag == "tutorialDoor")
        {
			SceneManager.LoadScene (2);
        }

		if (col.tag == "Level01Door")
		{
			SceneManager.LoadScene (0);
		}
    }

    void Recoil(Collider2D col)
    {
        IEnumerator coroutine = lockMotion(.4f);
        StartCoroutine(coroutine);
        coroutine = makeInvulnerable(invulnerabilityTime);
        StartCoroutine(coroutine);

        int xDirection;
        if (transform.position.x > col.transform.position.x)
            xDirection = 1;
        else
            xDirection = -1;

        Vector3 recoilVelocity = new Vector3 (xDirection*2, Mathf.Sqrt(2f * hopHeight * -gravity), 0);
        _controller.move(recoilVelocity * Time.deltaTime);
        SetAnimationState(animState.idle);

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

        Vector3 diff = maxGlowSize;
        diff.x *= glowPenalty;
        diff.y *= glowPenalty;
        currGlowSize -= diff;
        /*
        // Adjust glow
        float healthPercent = (float)currHealth / maxHealth;
        currGlowSize = maxGlowSize;
        currGlowSize.Scale(new Vector3(healthPercent, healthPercent, 1));
        glow.transform.localScale = currGlowSize;
       */

        // Adjust sprite brightness
        currSpriteBrightness = sprite.color;
        currSpriteBrightness -= new Vector4(.2f, .2f, .2f, 0);
        sprite.color = currSpriteBrightness;


        // Kill if necessary
        if (currHealth <= 0)
        {
            isInvulnerable = true;
            isLocked = true;
            SetAnimationState(animState.death);
            StartCoroutine("delayedDeath");
        }

    }

    IEnumerator lockMotion(float time)
    {
        isLocked = true;
        yield return new WaitForSeconds(time);
        isLocked = false;
        yield break;
    }

    IEnumerator makeInvulnerable(float time)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(time);
        isInvulnerable = false;
        yield break;
    }

    IEnumerator delayedDeath()
    {
        yield return new WaitForSeconds(1);
        Launch(gooPrefab, Vector3.zero);
        KillPlayer();
        isInvulnerable = false;
        isLocked = false;
        yield break;
    }


    void KillPlayer()
    {
        transform.position = respawnPoint;
        SetHealth(maxHealth);
        GetComponent<SpriteRenderer>().color = Color.white;
        SetAnimationState(animState.idle);
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

    void SetAnimationState(animState anim)
    {
        switch (anim)
        {
            case animState.idle:
                {
                    animator.SetTrigger("idle");
                    break;

                }
            case animState.hopStart:
                {
                    animator.SetTrigger("hopStart");
                    break;
                }
            case animState.hopEnd:
                {
                    animator.SetTrigger("hopEnd");
                    break;
                }
            case animState.jumpStart:
                {
                    animator.SetTrigger("jumpStart");

                    break;
                }
            case animState.jumpEnd:
                {
                    animator.SetTrigger("jumpEnd");
                    break;
                }
            case animState.death:
                {
                    animator.SetTrigger("death");
                    break;
                }
            case animState.spitStart:
                {
                    animator.SetTrigger("spitStart");
                    break;
                }
            case animState.spitEnd:
                {
                    animator.SetTrigger("spitEnd");
                    break;
                }
        }
    }


}
