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
		if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
		{
			Time.timeScale = 0;
			gameOverPanel.SetActive(true);

		}
	}

	public void RestartLevel(){
		Time.timeScale = 1;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void ExitLevel(){
		SceneManager.LoadScene ("mainMenu");
	}

	public void ContinueLevel(){
		Time.timeScale = 1;
		gameOverPanel.SetActive (false);
	}

	public void Play(){
		Time.timeScale = 1;
		SceneManager.LoadScene (1);
	}

	public void ExitGame(){
		Application.Quit();
	}


}
