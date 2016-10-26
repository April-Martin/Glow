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
    private Vector3 bombOffset;
    private Vector3 spitOffset;

    // Use this for initialization
    void Start()
    {
        curr = maxLevel;
        maxHeight = GetComponent<SpriteRenderer>().bounds.size.y;

        bombOffset = new Vector3(0, (float)bombCost / maxLevel * maxHeight);
        spitOffset = new Vector3(0, (float)spitCost / maxLevel * maxHeight);

        bombMarker.transform.localPosition += new Vector3(0, (maxHeight / 2) - bombOffset.y);
        spitMarker.transform.localPosition += new Vector3(0, (maxHeight / 2) - spitOffset.y);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool updateGooBar(Ammo ammo)
    {
        if (updateData(ammo))
        {
            updateGUI(ammo);
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


    private void updateGUI(Ammo ammo)
    {
        // Change level of goo bar
        float currPercent = (float)curr / maxLevel;
        transform.localScale = new Vector3(1, currPercent, 1);
//        float currHeight = currPercent * maxHeight;
 //       Vector3 offset = new Vector3(0, -(maxHeight - currHeight) / 2, 0);
 //       transform.localPosition = offset;

        if (ammo == Ammo.Bomb)
        {
            transform.localPosition -= (bombOffset / 2);
            bombMarker.transform.localPosition -= (bombOffset);
            spitMarker.transform.localPosition -= (bombOffset); 
        }
        else if (ammo == Ammo.Spit)
        {
            transform.localPosition -= (spitOffset / 2);
            bombMarker.transform.localPosition -= (spitOffset);
            spitMarker.transform.localPosition -= (spitOffset);
        }

        HideMarkers();
        // Change position of indicators
        //bombMarker.localPosition 
    }


    void HideMarkers()
    {
        if (bombMarker.transform.localPosition.y < -(maxHeight / 2))
        {
            bombMarker.gameObject.SetActive(false);
        }
        if (spitMarker.transform.localPosition.y < -(maxHeight / 2))
        {
            spitMarker.gameObject.SetActive(false);
        }
    }
}
