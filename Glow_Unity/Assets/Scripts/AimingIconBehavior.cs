using UnityEngine;
using System.Collections;

public class AimingIconBehavior : MonoBehaviour {

    private PlayerController player;
    private Renderer rend;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        player = GameObject.Find("Player 1").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!rend.isVisible || !player.isAiming)
        {
            Destroy(gameObject);
        }
    }

    
}
