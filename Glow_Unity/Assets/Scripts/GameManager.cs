using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public GameObject gameOverPanel;

	[HideInInspector] 
	public bool isPaused = false;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
		{
			Time.timeScale = 0;
			gameOverPanel.SetActive(true);
			isPaused = true;
		}
	}

	public void RestartLevel(){
		Time.timeScale = 1;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void ExitLevel(){
		SceneManager.LoadScene ("mainMenu");
	}

	public void ControlsScreen(){
		SceneManager.LoadScene (6);
	}

	public void ContinueLevel(){
		Time.timeScale = 1;
		gameOverPanel.SetActive (false);
		isPaused = false;
	}

	public void Play(){
		Time.timeScale = 1;
		SceneManager.LoadScene (1);
	}

	public void Level01(){
		Time.timeScale = 1;
		SceneManager.LoadScene (2);
	}

	public void Level02 (){
		Time.timeScale = 1;
		SceneManager.LoadScene (3);
	}

	public void Level03(){
		Time.timeScale = 1;
		SceneManager.LoadScene (4);
	}

	public void Level04(){
		Time.timeScale = 1;
		SceneManager.LoadScene (5);
	}

	public void levelSelect(){
		Time.timeScale = 1;
		SceneManager.LoadScene (8);
	}

	public void ExitGame(){
		Application.Quit();
	}


}
