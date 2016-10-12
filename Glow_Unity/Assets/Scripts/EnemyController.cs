using UnityEngine;
using System.Collections;
using Prime31;

public class EnemyController : MonoBehaviour {

	public float gravity = -35;
    public int health = 1;

	private CharacterController2D _controller;
	private PlayerController player;
    private int maxHealth;
    private int currHealth;

	// Use this for initialization
	void Start () {
		_controller = GetComponent<CharacterController2D> ();
		player = FindObjectOfType<PlayerController> ();
        maxHealth = health;
        currHealth = health;
	}
	
	// Update is called once per frame
	void Update () {

		// Handle gravity
		Vector3 velocity = _controller.velocity;
		velocity.y += gravity * Time.deltaTime;

		// Handle moving platforms
		if (_controller.isGrounded && _controller.ground != null && _controller.ground.tag == "MovingPlatform") {
			this.transform.parent = _controller.ground.transform;
		} else {
			if (this.transform.parent != null)
				transform.parent = null;
		}

		// Apply velocity
		_controller.move (velocity * Time.deltaTime);


	}

    void OnTriggerEnter2D(Collider2D other)
    {
       if (other.tag == "Damager")
       {
           DamageEnemy(1);
       }

    }

    void DamageEnemy(int dmg)
    {
        if (currHealth >= dmg)
            KillEnemy();
        else
            currHealth -= dmg;

    }

    void KillEnemy()
    {
        Destroy(gameObject);
    }

}
