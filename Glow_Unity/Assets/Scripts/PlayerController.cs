using UnityEngine;
using System;
using System.Collections;
using System.Timers;
using Prime31;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{


    // Dependency variables
    public GameManager gm;
    public GameObject gameCamera;
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

    public float aimingIconInterval = 0.4f;
    public float aimingSensitivity = .1f;

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
    enum animState { idle, hopStart, hopEnd, jumpStart, jumpEnd, death };

    // Misc variables
    private int currHealth;
    private bool isThrowingBomb = false;
    private bool isThrowingSpit = false;
    private bool isPickingUpSpit = false;
    private Vector3 aimingDirection;
    private float aimingIconElapsed = 0;
    private bool isFacingLeft = false;
    private bool isDying = false;

    // Delay variables
    public float delay = 0.1f;
    private bool isDelayed = false;
    private bool DelayShouldFinish = false;
    private Vector3 delayedVelocity;
    private float delayedTime;
    private Timer delayer;

    [HideInInspector]
    public bool isAiming = false;

    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        transform.position = startPos.position;
        animator = GetComponent<Animator>();

        gameCamera.GetComponent<CameraFollow2D>().startCameraFollow(this.gameObject);

        currHealth = maxHealth;
        glow = gameObject.transform.GetChild(0);
        sprite = GetComponent<SpriteRenderer>();
        maxGlowSize = glow.localScale;

        delayer = new Timer(delay);
        delayer.Enabled = false;
        delayer.AutoReset = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Move sprite
        HandleMotion();
        // Execute actions
        HandleActions();
        // Decrease brightness if necessary

        Debug.Log("player velocity.y: " + GetComponent<Rigidbody2D>().velocity.y);

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
        velocity.x = 0;

        if (isThrowingBomb || isThrowingSpit || isDying)
        {
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            _controller.move(velocity * Time.deltaTime);
            return;
        }

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
                isFacingLeft = false;
            }
            else
            {
                velocity.x = walkSpeed * (-1);
                isFacingLeft = true;
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
        // If the user JUST pressed a throwing action button:
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
            isAiming = false;
            isThrowingSpit = false;
            if (gooBar.DepleteGooBar(GooBar.Ammo.Spit))
            {
                Launch(gooPrefab, aimingDirection);
                Debug.Log("Threw spit. New goo level: " + gooBar.curr);
            }
        }

        // If the user just released the bomb button:
        else if (isThrowingBomb && Input.GetKeyUp(KeyCode.Alpha2))
        {
            isAiming = false;
            isThrowingBomb = false;
            if (gooBar.DepleteGooBar(GooBar.Ammo.Bomb))
            {
                Launch(bombPrefab, aimingDirection);
                Debug.Log("Threw bomb. New goo level: " + gooBar.curr);
            }
        }

        if (isThrowingBomb || isThrowingSpit)
        {
            PreviewTrajectory();
        }


        if (Input.GetKeyDown(KeyCode.Alpha3))
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
        GameObject goo = (GameObject)Instantiate(projectile, transform.position, Quaternion.identity);
        if (transform.parent != null)
            goo.transform.parent = transform.parent;
        goo.GetComponent<Rigidbody2D>().velocity = velocity;
    }




    void PreviewTrajectory()
    {
        isAiming = true;

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


    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDying)
            return;

        if (col.tag == "Killer")
        {
            KillPlayer();
        }

        if (col.tag == "PlayerDamager")
        {
            SetHealth(currHealth - 1);
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
            isDying = true;
            SetAnimationState(animState.death);
            StartCoroutine("delayedDeath");
        }

    }
    IEnumerator delayedDeath()
    {
        yield return new WaitForSeconds(1);
        Launch(gooPrefab, Vector3.zero);
        KillPlayer();
        isDying = false;
        yield break;
    }


    void KillPlayer()
    {
        transform.position = startPos.position;
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
        }
    }


}
