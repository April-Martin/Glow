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

    public bool DepleteGooBar(Ammo ammo)
    {
        if (ammo.Equals(Ammo.Bomb))
        {
            return updateGooBar(-bombCost);
        }
        else if (ammo.Equals(Ammo.Spit))
        {
            return updateGooBar(-spitCost);
        }
        else
            return false;
    }

    public void RecoverSpit()
    {
        updateGooBar(spitCost);
    }

    private bool updateGooBar(int diff)
    {
        if (updateData(diff))
        {
            updateGUI(diff);
            return true;
        }
        else
            return false;
    }

    private bool updateData(int diff)
    {
        if (curr+diff >= 0 && curr+diff <= maxLevel)
        {
            curr += diff;
            return true;
        }
        else
        {
            Debug.Log("Ran off edge of bar");
            return false;
        }
    }


    private void updateGUI(int diff)
    {
        // Change level of goo bar
        float currPercent = (float)curr / maxLevel;
        transform.localScale = new Vector3(1, currPercent, 1);
        Vector3 offset = new Vector3(0, (float)diff / maxLevel * maxHeight);
        transform.localPosition += offset / 2;

        // Adjust markers
        bombMarker.transform.localPosition += offset;
        spitMarker.transform.localPosition += offset;
        HideMarkers();
    }


    void HideMarkers()
    {
        if (bombMarker.transform.localPosition.y < -(maxHeight / 2))
        {
            bombMarker.gameObject.SetActive(false);
        }
        else
            bombMarker.gameObject.SetActive(true);

        if (spitMarker.transform.localPosition.y < -(maxHeight / 2))
        {
            spitMarker.gameObject.SetActive(false);
        }
        else
            spitMarker.gameObject.SetActive(true);

    }
}
