using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseScreen : UFEScreen {
	public virtual void GoToMainMenu(){
		Debug.Log("GoToMainMenu pause");
		ResumeGame();
		UFE.DanMainScreen();
		
	}

	public virtual void ResumeGame(){
		UFE.PauseGame(false);
	}
}
