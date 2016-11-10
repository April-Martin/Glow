using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

    public int amount;
    public pickupType type;

    public enum pickupType { goo, health };
}
