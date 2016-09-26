using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightProjector : MonoBehaviour {

    public Camera lightCam;
    public RawImage renderImage;
    public Material mat;
    public int screenWidth;
    public int screenHeight;

    private RenderTexture rt_light;

	// Use this for initialization
	void Awake () {

		screenWidth = Screen.width;
		screenHeight = Screen.height;

		rt_light = new RenderTexture(screenWidth, screenHeight, 16);
        lightCam.targetTexture = rt_light;
        mat.SetTexture("_LightTex", rt_light);

		mat.SetFloat("_ScreenWidth", screenWidth);
        mat.SetFloat("_ScreenHeight", screenHeight);

	}


	// Update is called once per frame
	void Start () {

	}

}
