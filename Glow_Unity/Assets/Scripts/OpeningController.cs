using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OpeningController : MonoBehaviour
{

    private Image[] images;
    private bool[] isMoving;
    private AudioSource src;

    public GameManager gm;

    void Start()
    {
        gm.pauseDisabled = true;

      //  src = GetComponent<AudioSource>();
        images = new Image[transform.childCount];

        for (int i = 0; i < images.Length; i++)
        {
            images[i] = transform.GetChild(i).GetComponent<Image>();
        }

        images[0].enabled = true;
        images[0].color = new Color(0, 0, 0, 1);
        images[1].enabled = false;

        StartCoroutine("cutscene");
    }


    IEnumerator cutscene()
    {
        while (images[0].color.r < 1)
        {
            images[0].color += new Color(.02f, .02f, .02f, 0);
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);

        while (images[0].color.r > 0)
        {
            images[0].color -= new Color(.1f, .1f, .1f, 0);
            yield return null;
        }

        images[0].GetComponent<Image>().enabled = false;
        images[1].enabled = true;
        images[1].color = new Color(0, 0, 0, 1);

        while (images[1].color.r < 1f)
        {
            images[1].color += new Color(.1f, .1f, .1f, 0);
            yield return null;
        }

        yield return new WaitForSeconds(2.2f);

        while (images[1].color.r > 0f)
        {
            images[1].color -= new Color(.05f, .05f, .05f, 0);
            yield return null;
        }

        gm.Tutorial();

    }

}
