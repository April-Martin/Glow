using UnityEngine;
using System.Collections;
using Prime31;

public class EnemyController : MonoBehaviour {

	public float gravity = -35;

	private CharacterController2D _controller;
	private PlayerController player;

	// Use this for initialization
	void Start () {
		_controller = GetComponent<CharacterController2D> ();
		player = FindObjectOfType<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = _controller.velocity;
		velocity.y += gravity * Time.deltaTime;
		_controller.move (velocity * Time.deltaTime);
	}

}
