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
    public Camera cam;

	private float scale;

    [HideInInspector]
    public int curr;

    private float maxHeight;
    private Vector3 bombOffset;
    private Vector3 spitOffset;

    // Use this for initialization
    void Start()
    {
		scale = transform.parent.transform.localScale.x;

		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        curr = maxLevel;
		maxHeight = sprite.bounds.size.y / scale;
		Debug.Log ("maxHeight = " + maxHeight);
        bombOffset = new Vector3(0, (float)bombCost / maxLevel * maxHeight );
        spitOffset = new Vector3(0, (float)spitCost / maxLevel * maxHeight );

        bombMarker.transform.localPosition += new Vector3(0, (maxHeight / 2) - bombOffset.y);
        spitMarker.transform.localPosition += new Vector3(0, (maxHeight / 2) - spitOffset.y);

        // Set goo bar to current position depending on screen size

        float topMargin = 50;
        float sideMargin = 100;
        Debug.Log("Sprite size: " + sprite.bounds.size.x + " x " + sprite.bounds.size.y);

        Vector3 worldOrigin = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 worldDest = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        worldDest.x -= (sprite.bounds.size.x / 2 * 3.5f);
        worldDest.y -= (sprite.bounds.size.y / 2 * 1.3f);
        Vector3 relativeWorldPos = worldDest - worldOrigin;
        /// Vector3 relativeWorldPos = new Vector3(0, 0, 10);
        relativeWorldPos.z = 10;
        transform.parent.transform.localPosition = relativeWorldPos;

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

    public bool RecoverSpit(int quantity)
    {
        return updateGooBar(spitCost * quantity);
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
        if (curr + diff >= 0 && curr + diff <= maxLevel)
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
