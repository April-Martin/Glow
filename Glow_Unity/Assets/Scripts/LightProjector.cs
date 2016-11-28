using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightProjector : MonoBehaviour {

    public Camera lightCam;
    public RawImage renderImage;
    public Material mat1;
    public Material mat2;
    [HideInInspector]
    public int screenWidth;
    [HideInInspector]
    public int screenHeight;

    private RenderTexture rt_light;

	// Use this for initialization
	void Awake () {

		screenWidth = Screen.width;
		screenHeight = Screen.height;

		rt_light = new RenderTexture(screenWidth, screenHeight, 16);
        lightCam.targetTexture = rt_light;

        mat1.SetTexture("_LightTex", rt_light);
		mat1.SetFloat("_ScreenWidth", screenWidth);
        mat1.SetFloat("_ScreenHeight", screenHeight);

        mat2.SetTexture("_LightTex", rt_light);
        mat2.SetFloat("_ScreenWidth", screenWidth);
        mat2.SetFloat("_ScreenHeight", screenHeight);

	}


	// Update is called once per frame
	void Start () {

	}

}
