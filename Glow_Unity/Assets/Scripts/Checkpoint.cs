using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public AudioClip chime;

	private SpriteRenderer gooSprite;
	private SpriteRenderer glowSprite;
	private SpriteRenderer lightSprite;
	private AudioSource src;
	private bool isActive = false;

	float glowAlphaDiff;
	Color gooColorDiff;
	Color lightColorDiff;
	Vector3 lightScaleDiff;

	private float interval = 60;

	// player's hex code: 72CBFFFF


	// Use this for initialization
	void Start () {
		gooSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
		glowSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
		lightSprite = transform.GetChild(2).GetComponent<SpriteRenderer>();
		src = GetComponent<AudioSource> ();
	}


	public void ChangeColor()
	{
		if (isActive)
			return;
		/*
        gooSprite.color = new Color32(0xc8, 0xe8, 0xff, 0xff);
        lightSprite.color = new Color32(0x72, 0xCB, 0xff, 0xff);
        lightSprite.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        */
		glowAlphaDiff = 1;
		gooColorDiff = new Color32 (0xc8, 0xe8, 0xff, 0xff);
		gooColorDiff -= gooSprite.color;
		lightColorDiff = new Color32 (0x72, 0xCB, 0xff, 0xff);
		lightColorDiff -= lightSprite.color;
		lightScaleDiff = new Vector3(1.5f, 1.5f, 1) - lightSprite.transform.localScale;

		src.PlayOneShot (chime);
		isActive = true;
		StartCoroutine("GradualChange");

	}

	public IEnumerator GradualChange()
	{

		while (glowSprite.color.a < 1) {

			glowSprite.color += new Color (0, 0, 0, (glowAlphaDiff / interval));
			gooSprite.color += gooColorDiff / interval;
			lightSprite.color += lightColorDiff / interval;
			lightSprite.transform.localScale += lightScaleDiff / interval;

			yield return null;

		}
		yield break;

	}
}
