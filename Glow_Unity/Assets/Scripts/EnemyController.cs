using UnityEngine;
using System.Collections;
using Prime31;

public class EnemyController : MonoBehaviour {

	public float gravity = -35;
    public int health = 1;
	public Color damageColor;

	protected CharacterController2D _controller;
    protected PlayerController player;
    protected int maxHealth;
    protected int currHealth;

	// Use this for initialization
	protected virtual void Start () {
		_controller = GetComponent<CharacterController2D> ();
		player = FindObjectOfType<PlayerController> ();
        maxHealth = health;
        currHealth = health;
	}
	
	// Update is called once per frame
	void Update () {
        HandleMovement();
	}

    protected virtual void HandleMovement()
    {
        Vector3 velocity = _controller.velocity;

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

        // Handle gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply velocity
        _controller.move(velocity * Time.deltaTime);


    }

    void OnTriggerEnter2D(Collider2D other)
    {
       if (other.tag == "EnemyDamager")
       {
           	DamageEnemy(1);
			other.GetComponent<BombBehavior> ().ExplodeBomb ();
       }

    }

    void DamageEnemy(int dmg)
    {
		if (currHealth <= dmg)
			KillEnemy ();
		else {
			currHealth -= dmg;
			StartCoroutine ("FlashRed");
		}

    }

    protected virtual void KillEnemy()
    {
        Destroy(gameObject);
    }

	IEnumerator FlashRed()
	{
		Debug.Log ("HELLO");
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		Color originalColor = sprite.color;
		sprite.color = damageColor;
		yield return new WaitForSeconds (.2f);
		sprite.color = originalColor;
		yield return new WaitForSeconds (.25f);
		sprite.color = damageColor;
		yield return new WaitForSeconds (.2f);
		sprite.color = originalColor;
	}

}
