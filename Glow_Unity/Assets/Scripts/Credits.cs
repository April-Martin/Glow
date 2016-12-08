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

    public GameManager gm;

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
        
        yield return new WaitForSeconds(.5f);
        isMoving[0] = true;                                 // Artist
        yield return new WaitForSeconds(1.8f);
        isMoving[1] = true;                                 // Designer
        yield return new WaitForSeconds(1.8f);
        isMoving[2] = true;                                 // Programmer
        yield return new WaitForSeconds(2.2f);
        for (int i = 0; i < 3; i++)
            assets[i].transform.position = offScreenPos;
        

        assets[3].transform.position = destinations[3];     // Special thanks
        yield return new WaitForSeconds(.4f);
        isMoving[4] = true;                                 // Alex
        yield return new WaitForSeconds(2.2f);
        assets[5].transform.position = destinations[5];     // and 
        yield return new WaitForSeconds(.4f);
        isMoving[6] = true;                                 // Indy
        yield return new WaitForSeconds(1.6f);
        assets[7].transform.position = destinations[7];     // Duck
        yield return new WaitForSeconds(.2f);
        src.PlayOneShot(quack, .2f);

        yield return new WaitForSeconds(1.6f);
        gm.ExitLevel();

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
