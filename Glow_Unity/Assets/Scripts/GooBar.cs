using UnityEngine;
using System.Collections;

public class GooBar : MonoBehaviour
{

    public int maxLevel = 15;
    public int spitCost = 1;
    public int bombCost = 3;
    public enum Ammo { Spit, Bomb };

    public Transform bombMarker;
    public Transform spitMarker;

    [HideInInspector]
    public int curr;
    private float maxHeight;

    // Use this for initialization
    void Start()
    {
        curr = maxLevel;
        maxHeight = GetComponent<SpriteRenderer>().bounds.size.y;

        float bombPercent = (float)bombCost / maxLevel;
        float spitPercent = (float)spitCost / maxLevel;
        bombMarker.localPosition += new Vector3(0, -maxHeight / 2 + bombPercent * maxHeight);
        spitMarker.localPosition += new Vector3(0, -maxHeight / 2 + spitPercent * maxHeight);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool updateGooBar(Ammo ammo)
    {
        if (updateData(ammo))
        {
            updateGUI();
            return true;
        }
        else
            return false;
    }

    private bool updateData(Ammo ammo)
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


    private void updateGUI()
    {
        float currPercent = (float)curr / maxLevel;
        transform.localScale = new Vector3(1, currPercent, 1);
        float currHeight = currPercent * maxHeight;
        Vector3 offset = new Vector3(0, -(maxHeight - currHeight) / 2, 0);
        //transform.position = offset;
        transform.localPosition = offset;
    }

}
