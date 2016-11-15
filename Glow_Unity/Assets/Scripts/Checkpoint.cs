using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    private SpriteRenderer gooSprite;
    private SpriteRenderer glowSprite;
    private SpriteRenderer lightSprite;

    float glowAlphaDiff;
    Color gooColorDiff;
    Color lightColorDiff;
    Vector3 lightScaleDiff;

    private float interval = 1;
  
    // player's hex code: 72CBFFFF


	// Use this for initialization
	void Start () {
        gooSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        glowSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        lightSprite = transform.GetChild(2).GetComponent<SpriteRenderer>();
	}
	

    public void ChangeColor()
    {
/*
        gooSprite.color = new Color32(0xc8, 0xe8, 0xff, 0xff);
        lightSprite.color = new Color32(0x72, 0xCB, 0xff, 0xff);
        lightSprite.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        */
        glowAlphaDiff = 0xff;
        gooColorDiff = new Color32(0xc8, 0xe8, 0xff, 0xff) - gooSprite.color;
        lightColorDiff = new Color32(0x72, 0xCB, 0xff, 0xff) - lightSprite.color;
        lightScaleDiff = new Vector3(1.5f, 1.5f, 1) - lightSprite.transform.localScale;

        StartCoroutine("GradualChange");

    }

    public IEnumerator GradualChange()
    {
        if (glowSprite.color.a == 0xff)
        {
            yield break;
        }

        glowSprite.color += new Color32(0x00, 0x00, 0x00, (byte) (glowAlphaDiff / interval));
        gooSprite.color += gooColorDiff / interval;
        lightSprite.color += lightColorDiff / interval;
        lightSprite.transform.localScale += lightScaleDiff / interval;

        yield return null;

    }
}
