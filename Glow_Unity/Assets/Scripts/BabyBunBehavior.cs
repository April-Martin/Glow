using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class BabyBunBehavior : MonoBehaviour {

    public GameManager gm;
    private SpriteRenderer light;
    private SpriteRenderer bun;
    private Animator anim;

    private float interval = 60;
    private Vector3 lightStartSize = new Vector3(.2f, .2f, 1);
    private Vector3 lightScaleDiff;
    private Color bunStartColor = new Color(.5f, .5f, .5f, 1);
    private Color bunColorDiff;

	// Use this for initialization
	void Start () {
        bun = GetComponent<SpriteRenderer>();
        light = transform.GetChild(0).GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        light.transform.localScale = lightStartSize;
        lightScaleDiff = new Vector3(1, 1, 1) - lightStartSize;

        bun.color = bunStartColor;
        bunColorDiff = new Color(1, 1, 1, 1) - bunStartColor;
	}
	

    public void PlayWinAnimation()
    {
        StartCoroutine("Animation");
        return;
    }

    public IEnumerator Animation()
    {
        bun.transform.Rotate(new Vector3(0, 180, 0));
        anim.SetTrigger("happy");

        yield return new WaitForSeconds(.5f);

        while (bun.color.r < 1)
        {
            bun.color += bunColorDiff / interval;
            light.transform.localScale += lightScaleDiff / interval;
            yield return null;
        }

        yield return new WaitForSeconds(.5f);

        if (gameObject.tag == "tutorialDoor")
        {
            SceneManager.LoadScene(2);
        }

        if (gameObject.tag == "Level01Door")
        {
            SceneManager.LoadScene(3);
        }

        if (gameObject.tag == "Level02Door")
        {
            SceneManager.LoadScene(4);
        }

        if (gameObject.tag == "Level03Door")
        {
            SceneManager.LoadScene(5);
        }

        if (gameObject.tag == "Level04Door")
        {
            SceneManager.LoadScene(7);
        }

        yield break;

    }
}
