using UnityEngine;
using System.Collections;


public class Credits : MonoBehaviour {

    private GameObject[] assets;
    private Vector3[] destinations;
    private bool[] isMoving;
    private AudioSource src;

    public Vector3 offScreenPos;
    public float speed;
    public AudioClip quack;

	void Start () {

        src = GetComponent<AudioSource>();

        assets = new GameObject[transform.childCount];
        destinations = new Vector3[transform.childCount];
        isMoving = new bool[transform.childCount];

        for (int i=0; i<assets.Length; i++)
        {
            assets[i] = transform.GetChild(i).gameObject;
            destinations[i] = assets[i].transform.position;
            isMoving[i] = false;

            assets[i].transform.position = new Vector3(destinations[i].x, offScreenPos.y);
        }

        StartCoroutine("rollCredits");
	}

    void OnDrawGizmos()
    {
        Debug.DrawLine(offScreenPos + Vector3.left * 1, offScreenPos + Vector3.right * 1);
        Debug.DrawLine(offScreenPos + Vector3.up * 1, offScreenPos + Vector3.down * 1);
    }
	
    IEnumerator rollCredits()
    {
        yield return new WaitForSeconds(1);
        isMoving[0] = true;
        yield return new WaitForSeconds(1.5f);
        isMoving[1] = true;
        yield return new WaitForSeconds(1.5f);
        isMoving[2] = true;
        yield return new WaitForSeconds(2);

        for (int i = 0; i < 3; i++)
            assets[i].transform.position = offScreenPos;

        assets[3].transform.position = destinations[3];
        yield return new WaitForSeconds(.5f);
        isMoving[4] = true;
        yield return new WaitForSeconds(.5f);
        isMoving[5] = true;
        isMoving[6] = true;
        yield return new WaitForSeconds(.5f);
        src.PlayOneShot(quack);
    }

	void Update () 
    {
        for (int i = 0; i < assets.Length; i++)
        {
            if (isMoving[i])
            {
                if (assets[i].transform.position.y < destinations[i].y)
                    assets[i].transform.position += Vector3.up * speed;
                else
                    isMoving[i] = false;
            }
        }
	}
}
