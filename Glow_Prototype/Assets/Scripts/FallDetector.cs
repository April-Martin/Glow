using UnityEngine;
using System.Collections;

public class FallDetector : MonoBehaviour {

	public PlayerController player;

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.GetComponent<PlayerController> () == player) {
			player.KillPlayer ();
		}
	}


}
