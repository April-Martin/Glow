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
    public HealthUI healthUI;
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
	public float recoilForce = 2;


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

    // Sound variables
    public AudioClip[] landingSounds;
    private AudioSource src;

    // Misc variables
    private int currHealth;
    private Vector3 respawnPoint;
    private bool isPickingUpSpit = false;
	[HideInInspector]
    public bool isInvulnerable = false;
	[HideInInspector]
    public bool isLocked = false;




    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        transform.position = startPos.position;
        respawnPoint = startPos.position;
        animator = GetComponent<Animator>();
        src = GetComponent<AudioSource>();

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
		if (gm.isPaused)
			return;

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
                {
                    SetAnimationState(animState.hopEnd);
                    PlayRandomLandingSound();
                }
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
                PlayRandomLandingSound();
            }
        }

        // Check if the player just hit the ground after a jump
        if (_controller.isGrounded && isJumping)
        {
            jumpCounter = 0;
            isJumping = false;
            SetAnimationState(animState.jumpEnd);
            PlayRandomLandingSound();
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
        if (!(isThrowing) && Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            if (Input.GetKeyDown(KeyCode.Q) && gooBar.curr >= gooBar.spitCost)
            {
                spitMode = true;
                bombMode = false;
            }
            else if (Input.GetKeyDown(KeyCode.E) && gooBar.curr >= gooBar.bombCost)
            {
                bombMode = true;
                spitMode = false;
            }
            else
                return;
        }

        // If the user just left bomb mode or spit mode:
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            spitMode = false;
            isThrowing = false;
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            bombMode = false;
            isThrowing = false;
        }

        // if the user just clicked a mouse button:
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // If they're using the keyboard to determine spit mode / bomb mode, don't change that.
            // But if they're not, infer it from the mouse button.
            if (!(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    spitMode = true;
                    bombMode = false;
                }
                else
                {
                    bombMode = true;
                    spitMode = false;
                }
            }

            isThrowing = true;
            isLocked = true;
			_controller.velocity = Vector3.zero;
            PreviewTrajectory();
            SetAnimationState(animState.spitStart);
        }

        // If the user just released a mouse button
        else if (isThrowing && Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            isThrowing = false;
            isLocked = false;
            if (spitMode)
            {
                if (gooBar.DepleteGooBar(GooBar.Ammo.Spit))
                    Launch(gooPrefab, aimingDirection);
            }
            else
            {
                if (gooBar.DepleteGooBar(GooBar.Ammo.Bomb))
                    Launch(bombPrefab, aimingDirection);
            }
            SetAnimationState(animState.spitEnd);
        }

        // IMPLEMENT ACTIONS
        if (isThrowing)
        {
            PreviewTrajectory();
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
                gooBar.RecoverSpit(1);
                Destroy(spit.gameObject);
            }
        }

    }


    Collider2D FindSpit()
    {
        // Set up ray tracing origins, directions, and lengths
        float[] lengths = new float[] { 0.7f, 0.5f };
        Vector3[] directions = new Vector3[] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
        Vector3[] rayOrigin = new Vector3[4];
        rayOrigin[0] = new Vector2(transform.position.x - (sprite.bounds.size.x / 6), transform.position.y);    // Vertical raycast origins
        rayOrigin[1] = new Vector2(transform.position.x + (sprite.bounds.size.x / 6), transform.position.y);
        rayOrigin[2] = new Vector2(transform.position.x, transform.position.y - (sprite.bounds.size.y / 6));    // Horizontal raycast origins
        rayOrigin[3] = new Vector2(transform.position.x, transform.position.y + (sprite.bounds.size.y / 6));

        Collider2D goo = null;
        for (int i = 0; i < directions.Length; i++)
        {
            if (goo != null) break;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin[i / 2 * 2], directions[i], lengths[i / 2], 1 << gooLayerNumber);

            if (hit.collider != null)
            {
                goo = hit.collider;
            }
            else
            {
                hit = Physics2D.Raycast(rayOrigin[i / 2 * 2 + 1], directions[i], lengths[i / 2], 1 << gooLayerNumber);
                if (hit.collider != null)
                {
                    goo = hit.collider;
                }
            }

        }

        return goo;
    }



    void Launch(GameObject projectile, Vector3 velocity)
    {
        GameObject obj = (GameObject)Instantiate(projectile, transform.position, Quaternion.identity);
        if (transform.parent != null)
            obj.transform.parent = transform.parent;
        obj.GetComponent<Rigidbody2D>().velocity = velocity;

		// Note: if it's a bomb, we need to attach the player's audio source to it.
		if (obj.GetComponent<BombBehavior> () != null) {
			obj.GetComponent<BombBehavior>().src = src;
		}
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
        UpdateAimingDirection();

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
            worldScreenLocation.y >= 0 && worldScreenLocation.y <= Screen.height)
        {
            // Move to next location
            Vector3 prevLocation = worldDrawLocation;
            float elapsed = 0;
            while ((worldDrawLocation - prevLocation).sqrMagnitude < aimingIconInterval)
            {
                currVelocity += Physics.gravity * timestep;
                worldDrawLocation += currVelocity * timestep;
                elapsed += timestep;
            }
            // Draw aiming icon and add it to the list
            GameObject node = (GameObject)Instantiate(aimingIconPrefab, worldDrawLocation, Quaternion.identity);
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
        diff /= (Screen.width * aimingScaler);

        if (diff.sqrMagnitude > maxThrow * maxThrow)
        {
            diff.Normalize();
            diff *= maxThrow;
        }

        aimingDirection = diff;
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Pickup")
        {
            bool used = false;
            Pickup pickup = col.GetComponent<Pickup>();
            if (pickup.type == Pickup.pickupType.goo)
            {
                if (gooBar.curr < gooBar.maxLevel)
                {
                    used = true;
                    gooBar.RecoverSpit(pickup.amount);
                }
            }
            else
            {
                Debug.Log("Hit health pickup");
                if (currHealth < maxHealth)
                {
                    SetHealth(currHealth + pickup.amount);
                    used = true;
                    if (currHealth > maxHealth)
                        SetHealth(maxHealth);
                }
            }
            if (used)
            {
                Destroy(pickup.gameObject);
                pickup.PickupAnimation();
            } 
        }

        if (col.tag == "Killer")
        {
            RespawnPlayer();
        }

        if (col.tag == "PlayerDamager")
        {
			if (isInvulnerable)
				return;
			
            SetHealth(currHealth - 1);
            if (currHealth > 0)
                Recoil(col);

        }

        if (col.tag == "Checkpoint")
        {
            respawnPoint = col.transform.position;
            col.GetComponent<Checkpoint>().ChangeColor();
        }

        if (col.tag == "tutorialDoor")
        {
            SceneManager.LoadScene(2);
        }

        if (col.tag == "Level01Door")
        {
            SceneManager.LoadScene(3);
        }

		if (col.tag == "Level02Door")
		{
			SceneManager.LoadScene(4);
		}

		if (col.tag == "Level03Door")
		{
			SceneManager.LoadScene(5);
		}

		if (col.tag == "Level04Door")
		{
			SceneManager.LoadScene(0);
		}
    }

    void Recoil(Collider2D col)
    {
        // Lock the character
        IEnumerator coroutine = lockMotion(.4f);
        StartCoroutine(coroutine);
        coroutine = makeInvulnerable(invulnerabilityTime);
        StartCoroutine(coroutine);

        // Apply new velocity
        int xDirection;
        if (transform.position.x > col.transform.position.x)
            xDirection = 1;
        else
            xDirection = -1;
        Vector3 recoilVelocity = new Vector3(xDirection * recoilForce, Mathf.Sqrt(2f * hopHeight * -gravity), 0);
        _controller.move(recoilVelocity * Time.deltaTime);

        // Make sure animations are doing right thing
        SetAnimationState(animState.idle);
        isJumping = false;
        isHopping = false;

        // Restore their jumps, so as to err on the side of kindness
        jumpCounter = 0;
    }

    void SetHealth(int newHealth)
    {

        // Set health
        currHealth = newHealth;
        Debug.Log("Curr health = " + currHealth);
        currGlowSize = maxGlowSize - maxGlowSize * ( maxHealth - currHealth) * glowPenalty;
        healthUI.SetHealthTo(currHealth);


        // Adjust sprite brightness
        currSpriteBrightness = sprite.color;
        float dec = (1 / (float) maxHealth) / 2;
        float curr = 1 - (maxHealth - currHealth) * dec;
        Debug.Log("dec = " + dec + ", curr = " + curr);
        currSpriteBrightness = new Vector4(curr, curr, curr, 1);
        sprite.color = currSpriteBrightness;


        // Kill if necessary
        if (currHealth <= 0)
        {
            StartCoroutine("delayedDeath");
        }

    }

    IEnumerator lockMotion(float time)
    {
        isLocked = true;
        yield return new WaitForSeconds(time);
        if (currHealth > 0)     // Don't overwrite death access to locks
            isLocked = false;
        yield break;
    }

    IEnumerator makeInvulnerable(float time)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(time);
        if (currHealth > 0)      // Don't overwrite death access to locks
            isInvulnerable = false;
        yield break;
    }

    IEnumerator delayedDeath()
    {
        isInvulnerable = true;
        isLocked = true;
        _controller.move(Vector3.zero);

        SetAnimationState(animState.death);
        ParticleSystem partSys = GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(.3f);
        partSys.Play();
        yield return new WaitForSeconds(.2f);
        partSys.Stop();
        partSys.Clear();
        Launch(gooPrefab, Vector3.zero);


        yield return new WaitForSeconds(.5f);

        RespawnPlayer();
        isInvulnerable = false;
        isLocked = false;
        yield break;
    }


    void RespawnPlayer()
    {
        transform.position = respawnPoint;
        SetHealth(maxHealth);
        GetComponent<SpriteRenderer>().color = Color.white;
		jumpCounter = 0;
        isHopping = false;
        isJumping = false;
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

    void PlayRandomLandingSound()
    {
        System.Random rn = new System.Random();
        src.PlayOneShot(landingSounds[rn.Next(3)], 1);
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
