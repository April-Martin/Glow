using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RestartLevel(){
		Application.LoadLevel (Application.loadedLevel);

	}

	public void ExitLevel(){
		Application.LoadLevel ("mainMenu");
	}

	public void Play(){
		Application.LoadLevel (1);
	}

	public void ExitGame(){
		Application.Quit();
	}


}
