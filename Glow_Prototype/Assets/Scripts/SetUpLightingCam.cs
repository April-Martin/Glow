using UnityEngine;
using System.Collections;

public class SetUpLightingCam : MonoBehaviour {

    public Shader lightingShader;

	void Start () {
        Camera cam = GetComponent<Camera>();
        cam.SetReplacementShader(lightingShader, "");
	}
	
}
