using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public GameObject gameOverPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.R))
		{
			gameOverPanel.SetActive(true);

		}
	}

	public void RestartLevel(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void ExitLevel(){
		SceneManager.LoadScene ("mainMenu");
	}

	public void Play(){
		SceneManager.LoadScene (1);
	}

	public void ExitGame(){
		Application.Quit();
	}


}
