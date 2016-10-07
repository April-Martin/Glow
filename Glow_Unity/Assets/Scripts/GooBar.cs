using UnityEngine;
using System.Collections;

public class GooBar : MonoBehaviour {

    public int max = 15;
    public int spitCost = 1;
    public int bombCost = 3;
    public enum Ammo { Spit, Bomb };

    [HideInInspector]
    public int curr;

	// Use this for initialization
	void Start () {
        curr = max;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool updateGooBar(Ammo ammo)
    {
        if (ammo.Equals(Ammo.Bomb) && curr >= bombCost)
        {
            curr -= bombCost;
            return true;
        }
        else if (ammo.Equals(Ammo.Spit) && curr >= spitCost)
        {
            curr -= spitCost;
            return true;
        }
        else
        {
            Debug.Log("Not enough goo");
            return false;
        }
    }

}
