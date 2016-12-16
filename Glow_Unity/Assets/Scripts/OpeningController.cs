using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OpeningController : MonoBehaviour
{

    private GameObject[] assets;
    private Vector3[] destinations;
    private bool[] isMoving;
    private AudioSource src;

    public GameManager gm;

    void Start()
    {

      //  src = GetComponent<AudioSource>();
        assets = new GameObject[transform.childCount];

        for (int i = 0; i < assets.Length; i++)
        {
            assets[i] = transform.GetChild(i).gameObject;
        }

        assets[0].GetComponent<Image>().enabled = true;
        assets[1].GetComponent<Image>().enabled = false;

        StartCoroutine("cutscene");
    }


    IEnumerator cutscene()
    {
        yield return new WaitForSeconds(3f);
        assets[0].GetComponent<Image>().enabled = false;
        assets[1].GetComponent<Image>().enabled = true; 
        yield return new WaitForSeconds(1.5f);

        gm.Tutorial();

    }

}
